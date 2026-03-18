using Godot;
using signallerMap.Scripts.Graphics;
using signallerMap.Scripts.Data;
using System;

namespace signallerMap.Scripts.Editor
{
    internal partial class Editor : CanvasLayer
    {
        [Export] private MapGrapher mapGrapher;
        [Export] private Node2D nodeContainer;
        [Export] private Node2D edgeContainer;
        public MapNode SelectedNode { get; set; }
        public MapEdge SelectedEdge { get; set; }

        public void CreateNode(MapNode node)
        {
            if (nodeContainer.HasNode(node.Id)) return;
            MapData.Nodes.Add(node);
            mapGrapher.DrawNode(node);
        }

        public void CreateEdge(MapNode from, MapNode to)
        {
            
        }
    }
}