using Godot;
using System.Collections.Generic;

namespace signallerMap.Scripts
{
    internal partial class MapEdge : Resource
    {
        public string Id { get; set; }
        public string StationId { get; set; }
        public MapNode From { get; set; }
        public MapNode To { get; set; }
        public List<MapPlatform> Platforms { get; set; } = new();
        public ColorRect Sprite { get; set; }
        public double Length { get; set; }
        public int Zindex { get; set; }
        public double MaxSpeed { get; set; }

        public MapNode GetSharedNode(MapEdge other)
        {
            if (other == null) return null;
            if (other.From == To) return other.From;
            if (From == other.To) return other.To;
            return null;
        }
    }


}
