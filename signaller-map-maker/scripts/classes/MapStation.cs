using System.Collections.Generic;
using Godot;

namespace signallerMap.Scripts
{
    internal partial class MapStation : Resource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<MapPlatform> Platforms { get; set; }
        public Label Sprite { get; set; }
    }

    internal partial class MapPlatform : Resource
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public MapEdge Edge { get; set; }
        public MapStation Station { get; set; }
        public ColorRect Sprite { get; set; }
        public PlatformVerticalAlignment VerticalAlignment { get; set; }
    }
}
