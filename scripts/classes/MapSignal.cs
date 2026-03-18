using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using signallerMap.Scripts.Graphics;

namespace signallerMap.Scripts
{
    internal class MapSignal
    {
        public string Id { get; set; }
        public MapNode Node { get; set; }
        public bool DrawBelowEdge = false;
        //public SignalSprite Sprite { get; set; }
        public SignalState State { get; set; } = SignalState.Proceed;
        public SignalState TargetState { get; set; } = SignalState.Proceed;
        public MapSignalMovement SignalMovement { get; set; }
        public List<MapSignalMovement> PossibleMovements = new List<MapSignalMovement>();

        // public MapSignal(MapSignal other)
        // {
        //     Id = other.Id;
        //     Node = other.Node;
        //     DrawBelowEdge = other.DrawBelowEdge;
        //     Sprite = other.Sprite;
        //     State = other.State;
        //     SignalMovement = other.SignalMovement;
        //     PossibleMovements = new List<MapSignalMovement>(other.PossibleMovements);
        // }

        public MapSignal GetNextSignal()
        {
            bool reversed = SignalMovement.From.To != Node;
            MapSignalMovement movement = SignalMovement;
            MapNode nextNode = reversed ? movement.To.From : movement.To.To;
            MapSignal nextSignal = nextNode.Signals.FirstOrDefault(s => s.SignalMovement.From == SignalMovement.To);
            return nextSignal;
        }

        public MapSignal GetPreviousSignal()
        {
            bool reversed = SignalMovement.From.To != Node;
            MapNode previousNode = reversed ? SignalMovement.From.To : SignalMovement.From.From;
            MapSignal previousSignal = previousNode.Signals.FirstOrDefault(s => s.SignalMovement.To == SignalMovement.From);
            if (previousSignal == null)
                return null;
            return previousSignal;
        }
    }

    internal class MapSignalMovement
    {
        public MapEdge From { get; set; }
        public MapEdge To { get; set; }
    }
    
}
