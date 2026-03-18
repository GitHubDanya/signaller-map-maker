using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Text;
using System.Threading.Tasks;

namespace signallerMap.Scripts
{
    internal partial class MapStation : Resource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<MapStationPlatform> Platforms { get; set; }
    }

    internal partial class MapStationPlatform : Resource
    {
        public int Number { get; set; }
        public MapEdge Edge { get; set; }
        public ColorRect Sprite { get; set; }
    }
}
