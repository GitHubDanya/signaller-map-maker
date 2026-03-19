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
        public List<MapEdge> IncomingEdges = new List<MapEdge>();
        public List<MapEdge> OutgoingEdges = new List<MapEdge>();
        public List<MapSignal> Signals = new List<MapSignal>();
        public Sprite2D Sprite { get; set; }
    }
}
