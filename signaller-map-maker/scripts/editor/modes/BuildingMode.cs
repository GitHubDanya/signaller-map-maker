using Godot;
using System;
using System.Linq;
using signallerMap.Scripts.Data;
using System.Collections.Generic;
using signallerMap.Scripts.Graphics;
using System.Xml.Serialization;

namespace signallerMap.Scripts.editor
{
    internal class BuildingMode : IEditorMode
    {
        // This mode is responsible for creating, removing and manipulating map objects.

        private Editor _editor;
        private EditorSelectionManager _selectionManager;
        private List<MapNode> selectedNodes;
        private List<MapEdge> selectedEdges;
        
        internal BuildingMode(Editor editor)
        {
            _editor = editor;
            _selectionManager = _editor.selectionManager;
            _selectionManager.SetSelectableNodeCount(2);
            _selectionManager.SetSelectableEdgeCount(1);
            selectedNodes = _selectionManager.SelectedNodes;
            selectedEdges = _selectionManager.SelectedEdges;

            MapGrapher grapher = _editor.mapGrapher;
            GrapherColors grapherColors = new()
            {
                SelectedNodeColor = Color.FromHtml("ffce1c"),
                SelectedEdgeColor = Color.FromHtml("fcb653"),
                SecondSelectedNodeColor = Color.FromHtml("ffde66") 
            };
            grapher.colors = grapherColors;
        }

        public void OnInputEvent(EditorInputEvent inputEvent, EditorInputEventArgs args)
        {
            switch (inputEvent)
            {
                case EditorInputEvent.RMBClick when args is EditorInputMouseClickArgs clickArgs:
                    MouseClick(clickArgs.Position); break;
                case EditorInputEvent.NodeClick when args is EditorInputOnNodeArgs nodeArgs:
                    _selectionManager.SelectNode(nodeArgs.Node); break;
                case EditorInputEvent.EdgeClick when args is EditorInputOnEdgeArgs edgeArgs:
                    _selectionManager.SelectEdge(edgeArgs.Edge); break;
            }
        }

        public void OnUiEvent(EditorUiEvent uiEvent, EditorUiEventArgs args)
        {
            switch (uiEvent)
            {
                case EditorUiEvent.NodeDeleteButtonPressed:
                    DeleteNode(); break;
                case EditorUiEvent.EdgeCreateButtonPressed when args is EditorUiCreateEdgeArgs edgeArgs:
                    CreateEdge(edgeArgs); break;
                case EditorUiEvent.EdgeDeleteButtonPressed:
                    DeleteEdge(); break;
            }
        }

        public void MouseClick(Vector2 position)
        {
            var childrenNodes = _editor.nodeContainer.GetChildren().Where(c => c is Sprite2D).Cast<Sprite2D>();
            MapNode existingNode = MapData.Nodes.FirstOrDefault(n => n.Position.IsEqualApprox(position));

            if (existingNode != null) return;

            MapNode node = MapFactory.CreateMapNode(position);

            CreateNodeCommand command = new(_editor, node);
            CommandManager.ExecuteCommand(command);
            
            _selectionManager.SelectNode(node);
        }

        private void DeleteNode(MapNode node = null)
        {
            if (node == null && selectedNodes.Count > 0) node = selectedNodes[0];
            if (node == null) return;

            var command = new DeleteNodeCommand(_editor, node);
            CommandManager.ExecuteCommand(command);
        }

        public void CreateEdge(EditorUiCreateEdgeArgs args)
        {
            if (selectedNodes[0] == null || selectedNodes.Count < 1 || selectedNodes[1] == null) return;
            if (int.TryParse(args.EdgeLength, out int el) == false
            || int.TryParse(args.EdgeSpeed, out int esl) == false) return;

            MapEdge edge = MapFactory.CreateMapEdge
            (from: selectedNodes[0], to: selectedNodes[1], length: el, maxSpeed: esl);

            var command = new CreateEdgeCommand(_editor, edge);
            CommandManager.ExecuteCommand(command);

            _selectionManager.SelectEdge(edge);
        }

        private void DeleteEdge(MapEdge edge = null)
        {
            if (edge == null && selectedEdges.Count > 0) edge = selectedEdges[0];
            if (edge == null) return;
            CommandManager.ExecuteCommand(new DeleteEdgeCommand(_editor, selectedEdges[0]));
        }
    }
}