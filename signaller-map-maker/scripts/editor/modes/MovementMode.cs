using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using signallerMap.Scripts.Data;
using signallerMap.Scripts.Graphics;

namespace signallerMap.Scripts.editor
{
    internal class MovementMode : IEditorMode
    {
        // This mode is responsible for defining possible movements
        // through a pair of edges.

        private Editor _editor;
        private EditorSelectionManager _selectionManager;
        private List<MapEdge> selectedEdges;
        private List<MapNode> selectedNodes;
        private List<MapSignal> selectedSignals;
        private Color movementInColor = Color.FromHtml("E55949");
        private Color movementOutColor = Color.FromHtml("4DE248");
        private Color movementInAndOutColor = Color.FromHtml("F2C34D");
        internal MovementMode(Editor editor)

        {
            _editor = editor;
            _selectionManager = editor.selectionManager;
            _selectionManager.SetSelectableNodeCount(1);
            _selectionManager.SetSelectableEdgeCount(2);
            _selectionManager.SetSelectableSignalCount(1);
            selectedNodes = _selectionManager.Nodes.Items;
            selectedEdges = _selectionManager.Edges.Items;
            selectedSignals = _selectionManager.Signals.Items;

            MapGrapher grapher = _editor.mapGrapher;
            GrapherColors grapherColors = new()
            {
                SelectedNodeColor = Color.FromHtml("4370d1"),
                SelectedEdgeColor = Color.FromHtml("348feb"),
                SecondSelectedNodeColor = Color.FromHtml("688cd9"),
                SecondSelectedEdgeColor = Color.FromHtml("64a8ed"),
            };
            grapher.colors = grapherColors;
        }
        
        public void OnInputEvent(EditorInputEvent inputEvent, EditorInputEventArgs args)
        {
            switch (inputEvent)
            {
                case EditorInputEvent.EdgeClick when args is EditorInputOnEdgeArgs edgeArgs:
                    _selectionManager.DeselectSignals(); SelectEdge(edgeArgs.Edge); break;
                case EditorInputEvent.EdgeHover when args is EditorInputOnEdgeArgs edgeArgs:
                    DisplayEdgeMovements(edgeArgs.Edge); break;
                case EditorInputEvent.EdgeUnhover when args is EditorInputOnEdgeArgs edgeArgs:
                    HideEdgeMovements(edgeArgs.Edge); break;
                case EditorInputEvent.SignalClick when args is EditorInputOnSignalArgs signalArgs:
                    _selectionManager.SelectSignal(signalArgs.Signal); break;
            }
        }
        
        public void OnUiEvent(EditorUiEvent uiEvent, EditorUiEventArgs args)
        {
            switch (uiEvent)
            {
                case EditorUiEvent.CreateMovementPressed when args is null:
                    CommandManager.ExecuteCommand(MapCommand.CreateMovement(_editor, CreateMovementBetweenSelected())); break;
                case EditorUiEvent.CreateSignalPressed when args is null:
                    CommandManager.ExecuteCommand(MapCommand.CreateSignal(_editor, CreateSignalBetweenSelected())); break;
                case EditorUiEvent.CycleSignalPressed when args is null:
                    CycleSelectedSignal(); break;
                case EditorUiEvent.DeleteSignalPressed when args is null:
                    CommandManager.ExecuteCommand(MapCommand.DeleteSignal(_editor, GetSelectedSignal())); break;
            }
        }
        
        private void SelectEdge(MapEdge edge)
        {
            _selectionManager.SelectEdge(edge);

            // Select in-between elements if two adjacent edges were selected
            if (_selectionManager.Edges.Items.Count == 2)
            {
                MapEdge sourceEdge = selectedEdges.ElementAtOrDefault(1);
                if (sourceEdge == null) return;
                MapNode node = edge.GetSharedNode(sourceEdge);
                if (node == null) return;
                MapSignal signal = node.Signals.FirstOrDefault(s => s.Movement.from == sourceEdge);
                
                _selectionManager.SelectNode(node);
                if (signal != null) _selectionManager.SelectSignal(signal);
            }
        }

        private MapMovement CreateMovementBetweenSelected()
        {
            if (selectedEdges.Count < 2
            || selectedEdges[0] == null
            || selectedEdges[1] == null
            || selectedEdges[0] == selectedEdges[1]) return null;

            MapMovement movement = MapFactory.CreateMapMovement(
                from: selectedEdges[1],
                to: selectedEdges[0]
            );

            return movement;
        }

        private MapSignal CreateSignalBetweenSelected()
        {
            MapMovement movement = CreateMovementBetweenSelected();
            if (movement == null) return null;
            MapNode node = movement.GetNode();
            if (node == null || node.Signals.Any(s => s.Movement.from == movement.from)) return null;
            
            if (!node.Movements.Contains(movement))
                CommandManager.ExecuteCommand(MapCommand.CreateMovement(_editor, movement));;

            return MapFactory.CreateMapSignal(movement);
        }

        private MapSignal GetSelectedSignal()
        {
            return _selectionManager.Signals.Items.ElementAtOrDefault(1);
        }

        private void DisplayEdgeMovements(MapEdge edge)
        {
            List<MapEdge> edgesIn = new();
            List<MapEdge> edgesOut = new();
            List<MapEdge> edgesInOut;

            var allPossibleMovements = edge.To.Movements.Concat(edge.From.Movements).ToList();

            edgesOut = allPossibleMovements.Where(m => m.from == edge).Select(m => m.to).ToList();
            edgesIn = allPossibleMovements.Where(m => m.to == edge).Select(m => m.from).ToList();

            edgesInOut = edgesIn.Intersect(edgesOut).ToList();

            colorEdges(edgesIn, movementInColor);
            colorEdges(edgesOut, movementOutColor);
            colorEdges(edgesInOut, movementInAndOutColor);
        }

        private void HideEdgeMovements(MapEdge edge)
        {
            List<MapEdge> connectedEdges = edge.To.Movements
            .Concat(edge.From.Movements)
            .Select(m => m.from == edge ? m.to : m.from)
            .ToList();
            
            foreach (MapEdge _edge in connectedEdges)
            if (!selectedEdges.Contains(_edge)) _editor.mapGrapher.DeselectEdge(_edge);
        }

        private void CycleSelectedSignal()
        {
            List<MapSignal> selectedSignals = _selectionManager.Signals.Items;
            if (selectedSignals.Count <= 0) return;
            _editor.CycleSignal(selectedSignals[0]);
        }

        private void colorEdges(List<MapEdge> edges, Color color)
        {
            foreach (MapEdge _edge in edges)
                if (selectedEdges.Contains(_edge)) break;
                else _editor.mapGrapher.ChangeEdgeColor(_edge, color);
        }       
    }
}