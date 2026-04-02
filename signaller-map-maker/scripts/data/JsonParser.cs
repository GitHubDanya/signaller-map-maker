using System;
using System.Linq;
using System.Collections.Generic;
using Godot;

namespace signallerMap.Scripts.Data
{
    internal static class MapConverters
    {
        public static MapConverter<MapNode, JsonMapNode> Node = new(new NodeConversionMethod());
        public static MapConverter<MapEdge, JsonMapEdge> Edge = new(new EdgeConversionMethod());
        public static MapConverter<MapStation, JsonMapStation> Station = new(new StationConversionMethod());
        public static MapConverter<MapPlatform, JsonMapPlatform> Platform = new(new PlatformConversionMethod());
        public static MapConverter<MapMovement, JsonMapMovement> Movement = new(new MovementConversionMethod());
        public static MapConverter<MapSignal, JsonMapSignal> Signal = new(new SignalConversionMethod());
    }

    internal class MapConverter<T, TJson>
    {
        private IJsonConversionMethod<T, TJson> conversionMethod;

        internal MapConverter(IJsonConversionMethod<T, TJson> method)
        {
            conversionMethod = method;
        }

        public List<T> ParseFromJson(IEnumerable<TJson> json)
        {
            return json.Where(j => j != null)
             .Select(j => conversionMethod.ParseJson(j))
             .ToList();
        }

        public List<TJson> ParseToJSON(IEnumerable<T> source)
        {
            return source.Where(s => s != null)
             .Select(s => conversionMethod.ToJson(s))
             .ToList();
        }
    }

    internal class NodeConversionMethod : IJsonConversionMethod<MapNode, JsonMapNode>
    {
        public JsonMapNode ToJson(MapNode node)
        {
            return new(node.Id, new[] { node.Position.X, node.Position.Y });
        }

        public MapNode ParseJson(JsonMapNode jsonNode)
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
    }

    internal class EdgeConversionMethod : IJsonConversionMethod<MapEdge, JsonMapEdge>
    {
        public JsonMapEdge ToJson(MapEdge edge)
        {
            return new(
                id: edge.Id,
                station_id: edge.StationId ?? string.Empty,
                from: edge.From.Id,
                to: edge.To.Id,
                length: edge.Length,
                max_speed: edge.MaxSpeed,
                z_index: edge.Zindex
            );
        }

        public MapEdge ParseJson(JsonMapEdge jsonEdge)
        {
            MapNode from = MapData.Nodes.FirstOrDefault(n => n.Id == jsonEdge.from);
            MapNode to = MapData.Nodes.FirstOrDefault(n => n.Id == jsonEdge.to);
            if (from == null || to == null) return null;
            return new()
            {
                Id = jsonEdge.id,
                StationId = jsonEdge.station_id,
                From = from,
                To = to,
                Length = jsonEdge.length,
                MaxSpeed = jsonEdge.max_speed,
                Zindex = jsonEdge.z_index
            };
        }
    }

    internal class StationConversionMethod : IJsonConversionMethod<MapStation, JsonMapStation>
    {
        public JsonMapStation ToJson(MapStation station)
        {
            return new(
                station.Id.ToString(),
                station.Name,
                station.Platforms.Select(p => p.Id.ToString()).ToArray()
            );
        }

        public MapStation ParseJson(JsonMapStation jsonStation)
        {
            List<MapPlatform> platforms = MapData.Platforms.Where(p => p.Station.Name == jsonStation.name).ToList();
            return new()
            {
                Id = jsonStation.id,
                Name = jsonStation.name,
                Platforms = platforms
            };
        }
    }

    internal class PlatformConversionMethod : IJsonConversionMethod<MapPlatform, JsonMapPlatform>
    {
        public JsonMapPlatform ToJson(MapPlatform platform)
        {
            return new(
                    platform.Id,
                    platform.Number,
                    platform.Edge.Id,
                    platform.Station.Id,
                    platform.VerticalAlignment.ToString()
                    );
        }

        public MapPlatform ParseJson(JsonMapPlatform jsonPlatform)
        {
            MapEdge edge = MapData.Edges.FirstOrDefault(e => e.Id == jsonPlatform.edge);
            MapStation station = MapData.Stations.FirstOrDefault(s => s.Id == jsonPlatform.station);

            return new()
            {
                Id = jsonPlatform.id,
                Number = jsonPlatform.number,
                Edge = edge,
                Station = station,
                VerticalAlignment = Enum.Parse<PlatformVerticalAlignment>(jsonPlatform.verticalalignment)
            };
        }
    }

    internal class MovementConversionMethod : IJsonConversionMethod<MapMovement, JsonMapMovement>
    {
        public JsonMapMovement ToJson(MapMovement movement)
        {
            return new(
                from: movement.from.Id,
                to: movement.to.Id
            );
        }

        public MapMovement ParseJson(JsonMapMovement movement)
        {
            return new()
            {
                from = MapData.Edges.FirstOrDefault(e => e.Id == movement.from),
                to = MapData.Edges.FirstOrDefault(e => e.Id == movement.to)
            };
        }
    }

    internal class SignalConversionMethod : IJsonConversionMethod<MapSignal, JsonMapSignal>
    {
        public JsonMapSignal ToJson(MapSignal signal)
        {
            return new(
                id: signal.Id,
                node: signal.Node.Id,
                edge: signal.Movement.from.Id,
                state: signal.State.ToString().ToLower()
            );
        }

        public MapSignal ParseJson(JsonMapSignal signal)
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
    }

    internal interface IJsonConversionMethod<T, TJson>
    {
        public TJson ToJson(T source);
        public T ParseJson(TJson json);
    }
}
