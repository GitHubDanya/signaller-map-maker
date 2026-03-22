using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Text;
using System.Threading.Tasks;
using signallerMap.Scripts.Graphics;

namespace signallerMap.Scripts
{
    internal partial class MapNode : Resource
    {
        public int Serial { get; set; }
        public string Prefix { get; set; }
        public string Id { get; set; }
        public Vector2 Position { get; set; }
        public List<MapEdge> Edges = new();
        public List<MapSignal> Signals = new();
        public List<MapMovement> Movements = new();
        public Sprite2D Sprite { get; set; }
    }

    internal partial class MapMovement
    {
        public MapEdge from;
        public MapEdge to;
        public MapNode GetNode()
        {
            if (from.To == to.From || from.To == to.To)
                return from.To;

            if (from.From == to.From || from.From == to.To)
                return from.From;
            return null;
        }

        public MapNode GetTargetNode()
        {
            MapNode node = GetNode();
            if (node == null) return null;
            if (to.To == node) return to.From;
            if (to.From == node) return to.To;
            return null;
        }

        public MapNode GetSourceNode()
        {
            MapNode node = GetNode();
            if (node == null) return null;
            if (from.To == node) return from.From;
            if (from.From == node) return from.To;
            return null;
        }
    }
}
