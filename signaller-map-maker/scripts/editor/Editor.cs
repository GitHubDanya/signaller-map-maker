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
        [Export] private MapGrapher mapGrapher;
        [Export] private Node2D nodeContainer;
        [Export] private Node2D edgeContainer;
        public MapNode[] SelectedNodes = new MapNode[2];
        public MapEdge SelectedEdge { get; set; }
        public string NodePrefix { get; set; } = "XX";
        private Dictionary<string, int> NodeIds = new();

        public override void _Ready()
        {
            nodeContainer = GetNode<Node2D>("/root/Map/MapGrapher/NodeContainer");
        }

        private void initialize()
        {
            NodeIds = new();
            SelectedNodes = new MapNode[2];
            SelectedEdge = null;
            NodePrefix = "XX";
        }

        public void NodeClickEvent(Vector2 position)
        {
            var childrenNodes = nodeContainer.GetChildren().Where(c => c is Sprite2D).Cast<Sprite2D>();
            MapNode existingNode = MapData.Nodes.FirstOrDefault(n => n.Position.IsEqualApprox(position));
            
            if (existingNode != null) return;
            else CreateNode(position);
        }

        public void CreateNode(MapNode node)
        {
            if (nodeContainer.HasNode(node.FullId)) return;
            
            MapData.Nodes.Add(node);
            mapGrapher.DrawNode(node);
            
            SelectNode(node);
        }
        public void CreateNode(Vector2 position)
        {
            RefreshNodeID();
            if (!NodeIds.ContainsKey(NodePrefix)) NodeIds.Add(NodePrefix, 1);
            
            string id = NodePrefix + NodeIds[NodePrefix]++.ToString();

            MapNode node = new()
            {
                Id = NodeIds[NodePrefix],
                FullId = id,
                Position = position
            };

            CreateNode(node);
        }

        public int RefreshNodeID()
        {
            int latestId = 1;

            if (!NodeIds.ContainsKey(NodePrefix)) return latestId;

            foreach (KeyValuePair<string, int> kvp in NodeIds)
                latestId = Math.Max(kvp.Value, latestId);

            return latestId;
        }

        public void SelectNode(MapNode node)
        {
            if (node == null) return;
            if (SelectedNodes[0] == node) node = SelectedNodes[1];
            mapGrapher.DeselectNode(SelectedNodes[1]);
            SelectedNodes[1] = SelectedNodes[0];
            SelectedNodes[0] = node;
            mapGrapher.SelectNodePair(SelectedNodes);
        }

        public void UpdateNode(string id = "")
        {
            if (string.IsNullOrEmpty(id)) id = SelectedNodes[0].FullId;
        }

        public void DeleteNode(string id = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                if (SelectedNodes[0] == null) return;
                id = SelectedNodes[0].FullId;
            }
            MapNode node = MapData.Nodes.FirstOrDefault(n => n.FullId == id);
            if (node == null) return;

            while (node.IncomingEdges.Count > 0) DeleteEdge(node.IncomingEdges[0].Id);
            while (node.OutgoingEdges.Count > 0) DeleteEdge(node.OutgoingEdges[0].Id);
            
            if (GodotObject.IsInstanceValid(node.Sprite))
                node.Sprite.QueueFree();
            MapData.Nodes.Remove(node);
            node.Dispose();

            SelectedNodes[0] = SelectedNodes[1];
            SelectNode(SelectedNodes[0]);
            SelectedNodes[1] = null;
        }

        public void CreateEdge(MapEdge edge)
        {
            MapData.Edges.Add(edge);
            mapGrapher.DrawEdge(edge);

            edge.From.OutgoingEdges.Add(edge);
            edge.To.IncomingEdges.Add(edge);

            SelectEdge(edge);
        }

        public void CreateEdge(MapNode from, MapNode to, int length, int maxSpeed, bool stumps = false)
        {
            int fromId = from.Id;
            int toId = to.Id;
            string id = NodePrefix + Math.Min(fromId, toId) + Math.Max(fromId, toId);

            MapEdge edge = new MapEdge()
            {
                Id = id,
                From = from,
                To = to,
                Length = length,
                MaxSpeed = maxSpeed
            };

            CreateEdge(edge);
        }

        public void SelectEdge(MapEdge edge)
        {
            if (edge == null) return;
            DeselectCurrentEdge();
            SelectedEdge = edge;
            mapGrapher.SelectEdge(edge);
        }

        public void DeselectCurrentEdge()
        {
            if (SelectedEdge == null) return;
            mapGrapher.DeselectEdge(SelectedEdge);
            SelectedEdge = null;
        }

        public void DeleteEdge(string id = "")
        {
            if (string.IsNullOrEmpty(id) && SelectedEdge != null) id = SelectedEdge.Id;
            MapEdge edge = MapData.Edges.FirstOrDefault(e => e.Id == id);

            if (edge == null) return;

            if (edge == SelectedEdge) DeselectCurrentEdge();

            edge.To?.IncomingEdges?.Remove(edge);
            edge.From?.OutgoingEdges?.Remove(edge);

            if (GodotObject.IsInstanceValid(edge.Sprite))
                edge.Sprite.QueueFree();

            MapData.Edges.Remove(edge);
            edge.Dispose();
        }

        public void CleanMap()
        {
            foreach (MapEdge edge in MapData.Edges.ToList()) { DeleteEdge(edge.Id); }
            foreach (MapNode node in MapData.Nodes.ToList()) { DeleteNode(node.FullId); }
            initialize();
        }
    }
}