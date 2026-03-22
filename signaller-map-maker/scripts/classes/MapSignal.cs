using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using signallerMap.Scripts.editor;
using signallerMap.Scripts.Graphics;

namespace signallerMap.Scripts
{
    internal partial class MapSignal : Resource
    {
        public string Id { get; set; }
        public MapNode Node { get; set; }
        private MapMovement _movement;
        public MapMovement Movement
        { get => _movement; set { SetMovement(value); } }
        public SignalState State = SignalState.Proceed;
        public Sprite2D Sprite;

        private void SetMovement(MapMovement movement)
        {
            if (_movement == null) { _movement = movement; return; }
            if (movement.from != _movement.from) return;
            _movement = movement;
        }
    }
}
