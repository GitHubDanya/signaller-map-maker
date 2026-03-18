using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace signallerMap.Scripts
{
    internal partial class TrainRoute
    {
        public int DepartingPlatform { get; set; }
        public int ArrivingPlatform { get; set; }
        public List<MapEdge> Route { get; set; } = new();
    }

    
}