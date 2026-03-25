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
            selectedNodes = _selectionManager.Nodes.Items;
            selectedEdges = _selectionManager.Edges.Items;

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
                    CreateNode(clickArgs.Position); break;
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

        public void CreateNode(Vector2 position)
        {
            var childrenNodes = _editor.nodeContainer.GetChildren().Where(c => c is Sprite2D).Cast<Sprite2D>();
            Vector2 nodePos = new Vector2(
                (float)Math.Round(position.X / 25f) * 25f,
                (float)Math.Round(position.Y / 25f) * 25f);
            MapNode existingNode = MapData.Nodes.FirstOrDefault(n => n.Position.IsEqualApprox(nodePos));

            if (existingNode != null) return;

            MapNode node = MapFactory.CreateMapNode(nodePos);

            CommandManager.ExecuteCommand(MapCommand.CreateNode(_editor, node));

            _selectionManager.SelectNode(node);
        }

        private void DeleteNode(MapNode node = null)
        {
            if (node == null && selectedNodes.Count > 0) node = selectedNodes[0];
            if (node == null) return;

            CommandManager.ExecuteCommand(MapCommand.DeleteNode(_editor, node));
        }

        public void CreateEdge(EditorUiCreateEdgeArgs args)
        {
            if (selectedNodes[0] == null || selectedNodes.Count < 1 || selectedNodes[1] == null) return;
            if (int.TryParse(args.EdgeLength, out int el) == false
            || int.TryParse(args.EdgeZindex, out int z) == false 
            || int.TryParse(args.EdgeSpeed, out int esl) == false) return;

            MapEdge edge = MapFactory.CreateMapEdge
            (from: selectedNodes[0], to: selectedNodes[1], length: el, zIndex: Math.Max(1, z), maxSpeed: esl);

            CommandManager.ExecuteCommand(MapCommand.CreateEdge(_editor, edge));

            _selectionManager.SelectEdge(edge);
        }

        private void DeleteEdge(MapEdge edge = null)
        {
            if (edge == null && selectedEdges.Count > 0) edge = selectedEdges[0];
            if (edge == null) return;
            CommandManager.ExecuteCommand(MapCommand.DeleteEdge(_editor, selectedEdges.ElementAtOrDefault(0)));
        }
    }
}