using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace signallerMap.Scripts
{
    internal partial class MapEdge : Resource
    {
        public string Id { get; set; }
        public string StationId { get; set; }
        public MapNode From { get; set; }
        public MapNode To { get; set; }
        public MapStationPlatform Platform { get; set; }
        public Line2D Sprite { get; set; }
        public double Length { get; set; }
        public double MaxSpeed { get; set; }
    }

    
}
