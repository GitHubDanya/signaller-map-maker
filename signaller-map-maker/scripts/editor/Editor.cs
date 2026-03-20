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
        // This class handles generic editor information and events.
        // Editor actions and properties are located here.
        // User inputted actions CAN NOT be directed here,
        // instead use CommandManager.cs


        [Export] public MapGrapher mapGrapher;
        [Export] public Node2D nodeContainer;
        [Export] public Node2D edgeContainer;
        public MapNode[] SelectedNodes = new MapNode[2];
        public MapEdge[] SelectedEdges = new MapEdge[2];
        public IEditorMode editorMode;

        public Dictionary<string, int> NodeIds = new();
        public string NodePrefix { get; set; } = "XX";

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
            editorMode = mode;
        }

        

        public void LMBClickEvent(Vector2 position)
        {
            editorMode.OnInputEvent(EditorInputEvent.LMBClick,
            new EditorInputMouseClickArgs() { Position = position });
        }

        public void RMBClickEvent(Vector2 position)
        {
            editorMode.OnInputEvent(EditorInputEvent.RMBClick,
            new EditorInputMouseClickArgs() { Position = position });
        }

        public void NodeClick(MapNode node)
        {
            editorMode.OnInputEvent(EditorInputEvent.NodeClick,
            new EditorInputOnNodeArgs() { Node = node } );
        }

        public void EdgeClick(MapEdge edge)
        {
            editorMode.OnInputEvent(EditorInputEvent.EdgeClick,
            new EditorInputOnEdgeArgs() { Edge = edge } );
        }
        
        public void MouseEnterEdgeEvent(MapEdge edge)
        {
            editorMode.OnInputEvent(EditorInputEvent.EdgeHover,
            new EditorInputOnEdgeArgs() { Edge = edge } );
        }

        public void MouseExitEdgeEvent(MapEdge edge)
        {
            editorMode.OnInputEvent(EditorInputEvent.EdgeUnhover,
            new EditorInputOnEdgeArgs() { Edge = edge });
        }

        public void FireUiEvent(EditorUiEvent uiEvent, EditorUiEventArgs args = null) {
            editorMode.OnUiEvent(uiEvent, args);
        }

        public int RefreshCurrentNodeID()
        {
            int latestId = 1;

            if (!NodeIds.ContainsKey(NodePrefix)) return latestId;

            foreach (KeyValuePair<string, int> kvp in NodeIds)
                latestId = Math.Max(kvp.Value, latestId);

            return latestId;
        }

        
        public void CreateNode(MapNode node)
        {
            var command = new CreateNodeCommand(this, node);
            CommandManager.ExecuteCommand(command);
        }

        public void CreateNode(Vector2 position)
        {
            RefreshCurrentNodeID();
            if (!NodeIds.ContainsKey(NodePrefix)) NodeIds.Add(NodePrefix, 1);
            
            string id = NodePrefix + NodeIds[NodePrefix]++.ToString();

            MapNode node = new()
            {
                Serial = NodeIds[NodePrefix],
                Prefix = NodePrefix,
                Id = id,
                Position = position
            };

            CreateNode(node);
        }

        public void DeleteNode(MapNode node)
        {
            if (node == null) return;
            while (node.Edges.Count > 0) DeleteEdge(node.Edges[0]);

            if (GodotObject.IsInstanceValid(node.Sprite))
                node.Sprite.QueueFree();
                
            MapData.Nodes.Remove(node);
            node.Dispose();
        }

        public void CreateEdge(MapEdge edge)
        {
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

            if (GodotObject.IsInstanceValid(edge.Sprite))
                edge.Sprite.QueueFree();

            MapData.Edges.Remove(edge);
            edge.Dispose();
        }

        public void ClearSelection()
        {
            Array.Clear(SelectedEdges);
            Array.Clear(SelectedNodes);
        }

        public void CleanMap()
        {
            foreach (MapEdge edge in MapData.Edges.ToList()) { DeleteEdge(edge); }
            foreach (MapNode node in MapData.Nodes.ToList()) { DeleteNode(node); }
            initialize();
        }
    }
}