using System.Collections.Generic;

namespace signallerMap.Scripts.Data
{
    internal static class MapData
    {
        public static HashSet<MapNode> Nodes = new();
        public static HashSet<MapEdge> Edges = new();
        public static HashSet<MapStation> Stations = new();
        public static HashSet<MapPlatform> Platforms = new();
        public static HashSet<MapSignal> Signals = new();
    }
}

