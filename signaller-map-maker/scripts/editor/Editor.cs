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

        private int lastNodeId = 1;

        public override void _Ready()
        {
            nodeContainer = GetNode<Node2D>("/root/Map/MapGrapher/NodeContainer");
        }

        public void NodeClickEvent(Vector2 position)
        {
            var childrenNodes = nodeContainer.GetChildren().Where(c => c is Sprite2D).Cast<Sprite2D>();
            MapNode existingNode = MapData.Nodes.FirstOrDefault(n => n.Position.IsEqualApprox(position));
            
            if (existingNode != null) SelectNode(existingNode);
            else CreateNode(position);
        }

        public void CreateNode(Vector2 position)
        {
            string id = NodePrefix + lastNodeId++.ToString();

            MapNode node = new()
            {
                Id = lastNodeId,
                FullId = id,
                Position = position
            };

            if (nodeContainer.HasNode(node.FullId)) return;
            
            MapData.Nodes.Add(node);
            mapGrapher.DrawNode(node);
            
            SelectNode(node);
        }

        public void SelectNode(MapNode node)
        {
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

            node.Sprite.QueueFree();
            MapData.Nodes.Remove(node);
            node.Dispose();

            SelectedNodes[0] = SelectedNodes[1];
            SelectNode(SelectedNodes[0]);
            SelectedNodes[1] = null;
        }

        public void CreateEdge(MapNode from, MapNode to, int length, int maxSpeed)
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

            MapData.Edges.Add(edge);
            mapGrapher.DrawEdge(edge);

            SelectedEdge = edge;
        }

        public void DeleteEdge(string id = "")
        {
            if (string.IsNullOrEmpty(id)) id = SelectedNodes[0].FullId;
            
        }
    }
}