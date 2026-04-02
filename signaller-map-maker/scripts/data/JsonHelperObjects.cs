namespace signallerMap.Scripts.Data
{
    internal record JsonMapNode(
            string id,
            float[] position
            );

    internal record JsonMapEdge(
            string id,
            string station_id,
            string from,
            string to,
            double length,
            double max_speed,
            int z_index
            );

    internal record JsonMapStation(
            string id,
            string name,
            string[] platforms
            );
    internal record JsonMapPlatform(
            string id,
            int number,
            string edge,
            string station,
            string verticalalignment
            );

    internal record JsonMapSignal(
            string id,
            string node,
            string edge,
            string state
            );

    internal record JsonMapMovement(
            string from,
            string to
            );
}
