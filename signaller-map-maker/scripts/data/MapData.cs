using System;
using System.Collections.Generic;
using Godot;

namespace signallerMap.Scripts.Data
{
    internal static class MapData
    {
        public static List<MapNode> Nodes = new();
        public static List<MapEdge> Edges = new();
        public static List<MapSignal> Signals = new();
    }
}