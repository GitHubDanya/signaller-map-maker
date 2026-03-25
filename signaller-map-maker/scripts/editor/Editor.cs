using Godot;
using signallerMap.Scripts.Graphics;
using signallerMap.Scripts.Data;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace signallerMap.Scripts.editor
{
    internal partial class Editor : CanvasLayer
    {
        // This is the main Editor class. User input is directed here to be manipulated.
        // Input is never linked to direct methods in this class, instead it is routed through
        // the current EDITOR MODE to handle custom logic for same actions.

        [Export] public MapGrapher mapGrapher;
        [Export] public Node2D nodeContainer;
        [Export] public Node2D edgeContainer;
        [Export] public Node2D signalContainer;
        public IEditorMode editorMode;
        public EditorSelectionManager selectionManager;

        internal Editor()
        {
            selectionManager = new();
        }

        public override void _Ready()
        {
            selectionManager.Initialize(mapGrapher);
            nodeContainer = GetNode<Node2D>("/root/Map/MapGrapher/NodeContainer");
            SetEditorMode(new BuildingMode(this));
        }

        private void initialize()
        {
            SetEditorMode(new BuildingMode(this));
        }

        public void SetEditorMode(IEditorMode mode)
        {
            selectionManager.ClearSelection();
            editorMode = mode;
        }

        public void FireInputEvent(EditorInputEvent e, EditorInputEventArgs args) =>
        editorMode.OnInputEvent(e, args);
        public void FireUiEvent(EditorUiEvent uiEvent, EditorUiEventArgs args = null) =>
        editorMode.OnUiEvent(uiEvent, args);
        
        public void CreateNode(MapNode node)
        {
            if (MapData.Nodes.Contains(node) || nodeContainer.HasNode(node.Id)) return;
            
            MapData.Nodes.Add(node);
            mapGrapher.DrawNode(node);
        } 

        public void DeleteNode(MapNode node)
        {
            if (node == null) return;
            while (node.Edges.Count > 0) DeleteEdge(node.Edges[0]);

            selectionManager.DeselectNode(node);

            if (GodotObject.IsInstanceValid(node.Sprite))
                node.Sprite.QueueFree();
                
            MapData.Nodes.Remove(node);
        }

        public void CreateEdge(MapEdge edge)
        {
            MapEdge existingEdge = MapData.Edges.FirstOrDefault(e => e.Id == edge.Id);
            if (existingEdge != null) DeleteEdge(existingEdge);

            MapData.Edges.Add(edge);
            mapGrapher.DrawEdge(edge);

            edge.From.Edges.Add(edge);
            edge.To.Edges.Add(edge);
        }

        public void DeleteEdge(MapEdge edge)
        {
            if (edge == null) return;

            edge.To?.Edges?.Remove(edge);
            edge.From?.Edges?.Remove(edge);

            selectionManager.DeselectEdge(edge);

            if (GodotObject.IsInstanceValid(edge.Sprite))
                edge.Sprite.QueueFree();

            MapData.Edges.Remove(edge);
        }

        public void CreateMovement(MapMovement movement)
        {
            if (movement == null || movement.from == null || movement.to == null) return;
            if (MapData.Nodes.SelectMany(n => n.Movements).Contains(movement)) return;
            movement.GetNode()?.Movements.Add(movement);
        }

        public void DeleteMovement(MapMovement movement)
        {
            if (movement == null) return;
            movement.GetNode()?.Movements.Remove(movement);
        }

        public void CreateSignal(MapSignal signal)
        {
            if (MapData.Signals.Contains(signal) ||
            signalContainer.GetNodeOrNull<Sprite2D>(signal.Id) != null) return;

            MapData.Signals.Add(signal);
            mapGrapher.DrawSignal(signal);
        }

        public void CycleSignal(MapSignal signal)
        {
            signal.CycleSignal();
            mapGrapher.SetSignalState(signal, signal.State);
        }

        public void DeleteSignal(MapSignal signal)
        {
            if (signal == null) return;

            signal.Node.Signals.Remove(signal);

            if (GodotObject.IsInstanceValid(signal.Sprite))
                signal.Sprite.QueueFree();

            MapData.Signals.Remove(signal);
        }

        public void CleanMap()
        {
            foreach (MapEdge edge in MapData.Edges.ToList()) { DeleteEdge(edge); }
            foreach (MapNode node in MapData.Nodes.ToList()) { DeleteNode(node); }
            initialize();
        }
    }

    internal class EditorSelectionManager
    {
        public SelectionState<MapNode> Nodes { get; private set; }
        public SelectionState<MapEdge> Edges { get; private set; }
        public SelectionState<MapSignal> Signals { get; private set; }
        public event Action SelectionChanged;
        private MapGrapher mapGrapher;

        public void Initialize(MapGrapher grapher)
        {
            mapGrapher = grapher;
            Nodes = new(2, mapGrapher.DeselectNode);
            Edges = new(2, mapGrapher.DeselectEdge);
            Signals = new(1, mapGrapher.DeselectSignal);
        }

        public void SelectNode(MapNode node)
        { Select(node, Nodes, () => mapGrapher.SelectNodePair(Nodes.Items)); }
        public void SelectEdge(MapEdge edge)
        { Select(edge, Edges, () => mapGrapher.SelectEdgePair(Edges.Items)); }
        public void SelectSignal(MapSignal signal)
        { Select(signal, Signals, () => mapGrapher.SelectSignal(Signals.Items[0])); }
        public void DeselectNode(MapNode node)
        { Deselect(node, Nodes); }
        public void DeselectEdge(MapEdge edge)
        { Deselect(edge, Edges); }
        public void DeselectSignal(MapSignal signal)
        { Deselect(signal, Signals); }
        public void SetSelectableNodeCount(int count)
        { SetSelectableCount(count, Nodes); }
        public void SetSelectableEdgeCount(int count)
        { SetSelectableCount(count, Edges); }
        public void SetSelectableSignalCount(int count)
        { SetSelectableCount(count, Signals); }


        private void Select<T>
        (T item, SelectionState<T> ss, Action onUpdate)
        where T : class
        {
            if (item == null) return;

            ss.Items.Insert(0, item);
            ss.Prune();

            onUpdate();
            SelectionChanged?.Invoke();
        }

        private void Deselect<T>
        (T item, SelectionState<T> ss)
        where T : class
        {
            if (item == null) return;
            ss.Remove(item);
        }

        private void SetSelectableCount<T>
        (int targetCount, SelectionState<T> ss)
        where T : class
        {
            ss.Limit = targetCount;
            ss.Prune();
        }
 
        internal class SelectionState<T> where T : class
        {
            public List<T> Items { get; } = new();
            public int Limit { get; set; } = 2;
            private readonly Action<T> _onDeselect;
            public SelectionState(int limit, Action<T> onDeselect)
            {
                Limit = limit;
                _onDeselect = onDeselect;
            }

            public void Prune()
            {
                while (Items.Count > Limit)
                {
                    T removed = Items[^1];
                    _onDeselect(removed);
                    Items.RemoveAt(Items.Count - 1);
                }
            }

            public void Remove(T item)
            {
                if (Items.Remove(item))
                {
                    _onDeselect?.Invoke(item);
                }
            }
        }
        public void DeselectNodes()
        { foreach (MapNode node in Nodes.Items.ToList()) { DeselectNode(node); } }
        public void DeselectEdges()
        { foreach (MapEdge edge in Edges.Items.ToList()) { DeselectEdge(edge); } }
        public void DeselectSignals()
        { foreach (MapSignal signal in Signals.Items.ToList()) { DeselectSignal(signal); } }

        
        public void ClearSelection()
        {
            DeselectNodes();
            DeselectEdges();
            DeselectSignals();
        }
    }
}