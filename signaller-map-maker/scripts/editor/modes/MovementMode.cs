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
        private Color movementInColor = Color.FromHtml("E55949");
        private Color movementOutColor = Color.FromHtml("4DE248");
        private Color movementInAndOutColor = Color.FromHtml("F2C34D");
        internal MovementMode(Editor editor)

        {
            _editor = editor;
            _selectionManager = editor.selectionManager;
            _selectionManager.SetSelectableNodeCount(1);
            _selectionManager.SetSelectableEdgeCount(2);
            selectedNodes = _selectionManager.SelectedNodes;
            selectedEdges = _selectionManager.SelectedEdges;

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
        
        public void MouseClick(Vector2 position)
        {
            _selectionManager.ClearSelection();
        }
        
        public void OnInputEvent(EditorInputEvent inputEvent, EditorInputEventArgs args)
        {
            switch (inputEvent)
            {
                case EditorInputEvent.EdgeClick when args is EditorInputOnEdgeArgs edgeArgs:
                    _selectionManager.SelectEdge(edgeArgs.Edge); break;
                case EditorInputEvent.EdgeHover when args is EditorInputOnEdgeArgs edgeArgs:
                    DisplayEdgeMovements(edgeArgs.Edge); break;
                case EditorInputEvent.EdgeUnhover when args is EditorInputOnEdgeArgs edgeArgs:
                    HideEdgeMovements(edgeArgs.Edge); break;
            }
        }
        
        public void OnUiEvent(EditorUiEvent uiEvent, EditorUiEventArgs args)
        {
            switch (uiEvent)
            {
                case EditorUiEvent.CreateMovementPressed when args is null:
                    CreateAndLogMovementBetweenSelected(); break;
                case EditorUiEvent.CreateSignalPressed when args is null:
                    CreateSignalBetweenSelected(); break;
            }
        }
        
        private void CreateAndLogMovementBetweenSelected()
        {
            MapMovement movement = CreateMovementBetweenSelected();
            if (movement == null) return;

            var command = new CreateNodeMovementCommand(_editor, movement);
            CommandManager.ExecuteCommand(command);
        }

        private void CreateSignalBetweenSelected()
        {
            MapMovement movement = CreateMovementBetweenSelected();
            if (movement == null) return;
            
            MapSignal signal = MapFactory.CreateMapSignal(movement);

            var command = new CreateSignalCommand(_editor, signal);
            CommandManager.ExecuteCommand(command);
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

        private void colorEdges(List<MapEdge> edges, Color color)
        {
            foreach (MapEdge _edge in edges)
                if (selectedEdges.Contains(_edge)) break;
                else _editor.mapGrapher.ChangeEdgeColor(_edge, color);
        }       
    }
}