using Godot;
using System;
using System.Collections.Generic;

namespace signallerMap.Scripts.Data
{
    internal class RouteJson
    {
        public string from { get; set; }
        public string to { get; set; }
        public int from_platform { get; set; }
        public int to_platform { get; set; }
        public List<string> edges { get; set; }
    }

    internal class JsonMapStation
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<JsonMapStationPlatform> platforms { get; set; }
    }

    internal class JsonMapStationPlatform
    {
        public int number { get; set; }
        public string edge_id { get; set; }
        public bool draw_under_edge { get; set; }
    }

    internal class JsonMapSignal
    {
        public string id { get; set; }
        public string node { get; set; }
        public string edge { get; set; }
        public string state { get; set; }
    }

    internal class JsonMapNode
    {
        public string id { get; set; }
        public float[] position { get; set; }
    }

    internal class JsonMapMovement
    {
        public string from { get; set; }
        public string to { get; set; }
    }

    internal class JsonMapEdge
    {
        public string id { get; set; }
        public string station_id { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public double  length { get; set; }
        public double max_speed { get; set; }
    }
}