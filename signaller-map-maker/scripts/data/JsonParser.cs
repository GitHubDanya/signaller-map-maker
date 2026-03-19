using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;

namespace signallerMap.Scripts.Data
{
    internal class JsonParser
    {
        public List<MapNode> ConvertToMapNodes(List<JsonMapNode> jsonNodes)
        {
            List<MapNode> nodes = new();
            foreach (JsonMapNode node in jsonNodes) nodes.Add(JSONToMapNode(node));
            return nodes;
        }
        public List<JsonMapNode> ConvertToJsonMapNodes(List<MapNode> nodes)
        {
            List<JsonMapNode> jsonMapNodes = new();
            foreach (MapNode node in nodes) jsonMapNodes.Add(MapNodeToJSON(node));
            return jsonMapNodes;
        }

        private JsonMapNode MapNodeToJSON(MapNode node)
        {
            return new()
            {
                id = node.FullId,
                pos = new[] { node.Position.X, node.Position.Y }
            };
        }
        
        private MapNode JSONToMapNode(JsonMapNode jsonNode)
        {
            return new()
            {
                FullId = jsonNode.id,
                Position = new Vector2(jsonNode.pos[0], jsonNode.pos[1])
            };
        }

        public List<MapEdge> ConvertToMapEdges(List<JsonMapEdge> jsonEdges)
        {
            List<MapEdge> edges = new();
            foreach (JsonMapEdge jsonEdge in jsonEdges)
            {
                MapEdge edge = JSONToMapEdge(jsonEdge);
                if (edge != null) edges.Add(edge);
            }
            return edges;
        }

        public List<JsonMapEdge> ConvertToJsonMapEdges(List<MapEdge> edges)
        {
            List<JsonMapEdge> jsonMapEdges = new();
            foreach (MapEdge edge in edges) jsonMapEdges.Add(MapEdgeToJSON(edge));
            return jsonMapEdges;
        }

        private JsonMapEdge MapEdgeToJSON(MapEdge edge)
        {
            return new()
            {
                id = edge.Id,
                station_id = edge.StationId ?? string.Empty,
                from = edge.From.FullId,
                to = edge.To.FullId,
                length = edge.Length,
                max_speed = edge.MaxSpeed
            };
        }

        private MapEdge JSONToMapEdge(JsonMapEdge jsonEdge)
        {
            MapNode from = MapData.Nodes.FirstOrDefault(n => n.FullId == jsonEdge.from);
            MapNode to = MapData.Nodes.FirstOrDefault(n => n.FullId == jsonEdge.to);
            if (from == null || to == null) return null;
            return new ()
            {
                Id = jsonEdge.id,
                StationId = jsonEdge.station_id,
                From = from,
                To = to,
                Length = jsonEdge.length,
                MaxSpeed = jsonEdge.max_speed
            };
        }
    }
}