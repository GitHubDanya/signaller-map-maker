using System;
using System.Linq;
using System.Collections.Generic;
using Godot;

namespace signallerMap.Scripts.Data
{
    internal static class MapFactory
    {
        public static Dictionary<string, int> NodeIds = new();
        public static Dictionary<string, int> SignalIds = new();
        public static int StationUID = 0;
        public static int PlatformUID = 0;
        public static string CurrentIdPrefix
        {
            get => _idPrefix;
            set => setPrefix(value);
        }
        private static string _idPrefix = "XX";

        private static void setPrefix(string value)
        {
            if (value.Length < 2)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Prefix must be 2 characters."
                );
            if (value.Length > 2)
                value = value[..2];
            _idPrefix = value.ToUpper();
        }

        public static void UpdateIndecies()
        {
            Dictionary<string, int> nodeIds = new();
            foreach (MapNode node in MapData.Nodes)
                nodeIds[node.Prefix] = node.Serial;
            NodeIds = nodeIds;

            Dictionary<string, int> signalIds = new();
            foreach (MapSignal signal in MapData.Signals)
                signalIds[signal.Id[..2]] = signal.Id[2..].ToInt();
            SignalIds = signalIds;

            StationUID = MapData.Stations
                .OrderByDescending(s => s.Id)
                .FirstOrDefault().Id.ToInt();

            PlatformUID = MapData.Platforms
                .OrderByDescending(p => p.Id)
                .FirstOrDefault().Id.ToInt();
        }

        public static MapNode CreateMapNode(Vector2 pos)
        {
            string prefix = _idPrefix;
            if (!NodeIds.ContainsKey(prefix))
                NodeIds[prefix] = 1;
            int serial = NodeIds[prefix]++;

            MapNode node = new()
            {
                Serial = serial,
                Prefix = prefix,
                Id = prefix + serial.ToString(),
                Position = pos,
            };

            return node;
        }

        public static MapEdge CreateMapEdge(
            MapNode from,
            MapNode to,
            int length,
            int zIndex,
            int maxSpeed,
            bool stumps = false
        )
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(length),
                    "Length cannot be 0 or less."
                );
            if (maxSpeed <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maxSpeed),
                    "Speed limit cannot be 0 or less"
                );

            int fromId = from.Serial;
            int toId = to.Serial;
            string id = from.Prefix + Math.Min(fromId, toId) + Math.Max(fromId, toId);
            if (from.Prefix != to.Prefix)
                id += 'N';

            MapEdge edge = new MapEdge()
            {
                Id = id,
                From = from,
                To = to,
                Length = length,
                Zindex = zIndex,
                MaxSpeed = maxSpeed,
            };

            return edge;
        }

        public static MapMovement CreateMapMovement(MapEdge from, MapEdge to)
        {
            MapMovement movement = new() { from = from, to = to };

            return movement;
        }

        public static MapSignal CreateMapSignal(MapMovement movement)
        {
            MapNode node = movement.GetNode();
            if (node == null)
                throw new ArgumentNullException(
                    nameof(movement),
                    "Cannot create signal - movement has no valid node."
                );

            string prefix = node.Prefix;
            if (!SignalIds.ContainsKey(prefix))
                SignalIds[prefix] = 1;
            int serial = SignalIds[prefix]++;

            MapSignal signal = new()
            {
                Id = prefix + serial.ToString(),
                Node = node,
                Movement = movement,
            };

            if (!signal.Node.Signals.Contains(signal))
                signal.Node.Signals.Add(signal);

            return signal;
        }

        public static MapStation CreateMapStation(
            string name,
            List<MapPlatform> platforms = null
        )
        {
            if (platforms == null)
                platforms = new();
            StationUID++;
            return new()
            {
                Id = StationUID.ToString(),
                Name = name,
                Platforms = platforms,
            };
        }

        public static MapPlatform CreateMapStationPlatform(
            int number,
            MapEdge edge,
            MapStation station,
            PlatformVerticalAlignment alignment
        )
        {
            PlatformUID++;
            return new()
            {
                Id = PlatformUID.ToString(),
                Number = number,
                Edge = edge,
                Station = station,
                VerticalAlignment = alignment,
            };
        }
    }
}
