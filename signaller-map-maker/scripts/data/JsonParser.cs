using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using signallerMap.Scripts.editor;

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
            return new(node.Id, new[] {node.Position.X, node.Position.Y});
        }
        
        private MapNode JSONToMapNode(JsonMapNode jsonNode)
        {
            string id = jsonNode.id;
            if (jsonNode.id.Length < 2) id = "XX000";
            int serial = int.Parse(id.Substring(2));
            string prefix = id.Substring(0, 2);
            return new()
            {
                Serial = serial,
                Prefix = prefix,
                Id = prefix + serial,
                Position = new Vector2(jsonNode.position[0], jsonNode.position[1]),
                Movements = new()
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
            return new(
                id: edge.Id,
                station_id: edge.StationId ?? string.Empty,
                from: edge.From.Id,
                to: edge.To.Id,
                length: edge.Length,
                max_speed: edge.MaxSpeed
            );
        }

        private MapEdge JSONToMapEdge(JsonMapEdge jsonEdge)
        {
            MapNode from = MapData.Nodes.FirstOrDefault(n => n.Id == jsonEdge.from);
            MapNode to = MapData.Nodes.FirstOrDefault(n => n.Id == jsonEdge.to);
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

        public List<MapMovement> ConvertToToMapMovements(List<JsonMapMovement> movements)
        {
            List<MapMovement> mapMovements = new();
            foreach (JsonMapMovement movement in movements) mapMovements.Add(JSONToMapMovement(movement));
            return mapMovements;
        }

        public List<JsonMapMovement> ConvertToJsonMapMovements(List<MapMovement> movements)
        {
            List<JsonMapMovement> mapMovements = new();
            foreach (MapMovement movement in movements) mapMovements.Add(MapMovementToJSON(movement));
            return mapMovements;
        }

        private MapMovement JSONToMapMovement(JsonMapMovement movement)
        {
            return new()
            {
                from = MapData.Edges.FirstOrDefault(e => e.Id == movement.from),
                to = MapData.Edges.FirstOrDefault(e => e.Id == movement.to)
            };
        }
        
        public JsonMapMovement MapMovementToJSON(MapMovement movement)
        {
            return new(
                from: movement.from.Id,
                to: movement.to.Id
            );
        }

        public List<MapSignal> ConvertToMapSignals(List<JsonMapSignal> signals)
        {
            List<MapSignal> mapSignals = new();
            foreach (JsonMapSignal signal in signals) mapSignals.Add(JsonToMapSignal(signal));
            return mapSignals;
        }

        public List<JsonMapSignal> ConvertToJsonMapSignals(List<MapSignal> signals)
        {
            List<JsonMapSignal> mapSignals = new();
            foreach (MapSignal signal in signals) mapSignals.Add(MapSignalToJSON(signal));
            return mapSignals;
        }

        private MapSignal JsonToMapSignal(JsonMapSignal signal)
        {
            MapNode node = MapData.Nodes.FirstOrDefault(n => n.Id == signal.node);
            return new()
            {
                Id = signal.id,
                Node = node,
                Movement = node.Movements.FirstOrDefault(m => m.from.Id == signal.edge),
                State = signal.state switch
                {
                    "danger" => SignalState.Danger,
                    "caution" => SignalState.Caution,
                    "doubleyellow" => SignalState.DoubleYellow,
                    _ => SignalState.Proceed
                }
            };
        }
        
        public JsonMapSignal MapSignalToJSON(MapSignal signal)
        {
            return new(
                id: signal.Id,
                node: signal.Node.Id,
                edge: signal.Movement.from.Id,
                state: signal.State.ToString().ToLower()
            );
        } 
    }
}