using Godot;
using System;
using System.Collections.Generic;

namespace signallerMap.Scripts.Data
{
    internal record JsonMapSignal(
        string id,
        string node,
        string edge,
        string state
    );

    internal record JsonMapNode(
        string id,
        float[] position
    );

    internal record JsonMapMovement(
        string from,
        string to
    );

    internal record JsonMapEdge(
        string id,
        string station_id,
        string from,
        string to,
        double length,
        double max_speed
    );
}