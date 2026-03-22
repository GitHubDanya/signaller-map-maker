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
        public List<MapNode> SelectedNodes = new();
        public List<MapEdge> SelectedEdges = new();
        public int SelectableNodeCount { get; private set; } = 2;
        public int SelectableEdgeCount { get; private set; } = 2;
        public IEditorMode editorMode;

        public Dictionary<string, int> NodeIds = new();
        public Dictionary<string, int> SignalIds = new();
        public string NextNodePrefix { get; set; } = "XX";

        public override void _Ready()
        {
            nodeContainer = GetNode<Node2D>("/root/Map/MapGrapher/NodeContainer");
            SetEditorMode(new BuildingMode(this));
        }

        private void initialize()
        {
            SetEditorMode(new BuildingMode(this));
        }

        public void SetEditorMode(IEditorMode mode)
        {
            ClearSelection();
            editorMode = mode;
        }

        private void FireInputEvent(EditorInputEvent e, EditorInputEventArgs args)
        { editorMode.OnInputEvent(e, args); }

        public void LMBClickEvent(Vector2 position)
        { FireInputEvent(EditorInputEvent.LMBClick, new EditorInputMouseClickArgs { Position = position }); }
        public void RMBClickEvent(Vector2 position)
        { FireInputEvent(EditorInputEvent.RMBClick, new EditorInputMouseClickArgs { Position = position }); }
        public void NodeClick(MapNode node)
        { FireInputEvent(EditorInputEvent.NodeClick, new EditorInputOnNodeArgs { Node = node }); }
        public void EdgeClick(MapEdge edge)
        { FireInputEvent(EditorInputEvent.EdgeClick, new EditorInputOnEdgeArgs { Edge = edge }); }
        public void SignalClick(MapSignal signal)
        { FireInputEvent(EditorInputEvent.SignalClick, new EditorInputOnSignalArgs { Signal = signal}); }
        public void MouseEnterNodeEvent(MapNode node)
        { FireInputEvent(EditorInputEvent.NodeHover, new EditorInputOnNodeArgs { Node = node }); }
        public void MouseExitNodeEvent(MapNode node)
        { FireInputEvent(EditorInputEvent.NodeUnhover, new EditorInputOnNodeArgs { Node = node }); }
        public void MouseEnterEdgeEvent(MapEdge edge)
        { FireInputEvent(EditorInputEvent.EdgeHover, new EditorInputOnEdgeArgs { Edge = edge }); }
         public void MouseExitEdgeEvent(MapEdge edge)
         { FireInputEvent(EditorInputEvent.EdgeUnhover, new EditorInputOnEdgeArgs { Edge = edge });}
        public void FireUiEvent(EditorUiEvent uiEvent, EditorUiEventArgs args = null)
        { editorMode.OnUiEvent(uiEvent, args); }


        public int RefreshCurrentNodeID()
        {
            int latestId = 1;

            if (!NodeIds.ContainsKey(NextNodePrefix)) return latestId;

            foreach (KeyValuePair<string, int> kvp in NodeIds)
                latestId = Math.Max(kvp.Value, latestId);

            return latestId;
        }
        
        public void CreateNode(MapNode node)
        {
            if (MapData.Nodes.Contains(node) || nodeContainer.HasNode(node.Id)) return;
            
            MapData.Nodes.Add(node);
            mapGrapher.DrawNode(node);
            
            if (!NodeIds.ContainsKey(NextNodePrefix)) NodeIds[NextNodePrefix] = 1;
            NodeIds[NextNodePrefix]++;
        }

        public void SelectNode(MapNode node)
        {
            if (node == null) return;
            
            SelectedNodes.Insert(0, node);
            if (SelectedNodes.Count > SelectableNodeCount)
            {
                MapNode removed = SelectedNodes[^1];
                mapGrapher.DeselectNode(removed);
                SelectedNodes.RemoveAt(SelectedNodes.Count - 1);
                
            }

            mapGrapher.SelectNodePair(SelectedNodes);
        }

        public void DeselectNode(MapNode node)
        {
            if (node == null) return;
            if (SelectedNodes.Remove(node))
                mapGrapher.DeselectNode(node);
        }
        
        public void SetSelectableNodeCount(int count)
        {
            SelectableNodeCount = count;

            while (SelectedNodes.Count > SelectableNodeCount)
            {
                MapNode removed = SelectedNodes[^1];
                mapGrapher.DeselectNode(removed);
                SelectedNodes.RemoveAt(SelectedNodes.Count - 1);
            }
        }

        public void DeleteNode(MapNode node)
        {
            if (node == null) return;
            while (node.Edges.Count > 0) DeleteEdge(node.Edges[0]);

            DeselectNode(node);

            if (GodotObject.IsInstanceValid(node.Sprite))
                node.Sprite.QueueFree();
                
            MapData.Nodes.Remove(node);
            node.Dispose();
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

        public void SelectEdge(MapEdge edge)
        {
            if (edge == null) return;
            
            SelectedEdges.Insert(0, edge);
            if (SelectedEdges.Count > SelectableEdgeCount)
            {
                MapEdge removed = SelectedEdges[^1];
                mapGrapher.DeselectEdge(removed);
                SelectedEdges.RemoveAt(SelectedEdges.Count - 1);
            }
            
            mapGrapher.SelectEdgePair(SelectedEdges);
        }

        public void DeselectEdge(MapEdge edge)
        {
            if (edge == null) return;
            if (SelectedEdges.Remove(edge))
                mapGrapher.DeselectEdge(edge);
        }

        public void SetSelectableEdgeCount(int count)
        {
            SelectableEdgeCount = count;

            while (SelectedEdges.Count > SelectableEdgeCount)
            {
                MapEdge removed = SelectedEdges[^1];
                mapGrapher.DeselectEdge(removed);
                SelectedEdges.RemoveAt(SelectedEdges.Count - 1);
            }
        }

        public void DeleteEdge(MapEdge edge)
        {
            if (edge == null) return;

            edge.To?.Edges?.Remove(edge);
            edge.From?.Edges?.Remove(edge);

            DeselectEdge(edge);

            if (GodotObject.IsInstanceValid(edge.Sprite))
                edge.Sprite.QueueFree();

            MapData.Edges.Remove(edge);
            edge.Dispose();
        }

        public void CreateMovement(MapMovement movement)
        {
            if (movement == null || movement.from == null || movement.to == null) return;
            if (MapData.Nodes.SelectMany(n => n.Movements).Contains(movement)) return;
            movement.GetNode()?.Movements.Add(movement);
        }

        public void DeleteNodeMovement(MapMovement movement)
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

            if (!signal.Node.Signals.Contains(signal))
            signal.Node.Signals.Add(signal);

            string prefix = signal.Node.Prefix;
            if (!SignalIds.ContainsKey(prefix)) SignalIds[prefix] = 1;
            SignalIds[prefix]++;
        }

        public void DeleteSignal(MapSignal signal)
        {
            if (signal == null) return;

            signal.Node.Signals.Remove(signal);

            if (GodotObject.IsInstanceValid(signal.Sprite))
                signal.Sprite.QueueFree();

            MapData.Signals.Remove(signal);
            signal.Dispose();
        }

        public void ClearSelection()
        {
            foreach (MapEdge edge in SelectedEdges) { mapGrapher.DeselectEdge(edge); }
            foreach (MapNode node in SelectedNodes) { mapGrapher.DeselectNode(node); }
            SelectedEdges.Clear();
            SelectedNodes.Clear();
        }

        public void CleanMap()
        {
            foreach (MapEdge edge in MapData.Edges.ToList()) { DeleteEdge(edge); }
            foreach (MapNode node in MapData.Nodes.ToList()) { DeleteNode(node); }
            initialize();
        }
    }
}