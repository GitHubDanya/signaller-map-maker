using Godot;
using signallerMap.Scripts.Data;
using signallerMap.Scripts.editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace signallerMap.Scripts.UI
{
    internal partial class UiController : PanelContainer
    {

        // UI Elements
        [Export] private Label nodeLabel { get; set; }
        [Export] private LineEdit nodePrefixField { get; set; }
        [Export] private Button nodeDeleteButton { get; set; }
        [Export] private Label edgeSelectedLabel { get; set; }
        [Export] private Label edgeFromLabel { get; set; }
        [Export] private Label edgeToLabel { get; set; }
        [Export] private Button edgeCreateButton { get; set; }
        [Export] private CheckBox edgeIsStumpCheckbox { get; set; }
        [Export] private LineEdit edgeLength { get; set; }
        [Export] private LineEdit edgeSpeedLimit { get; set; }
        [Export] private Button edgeDeleteButton { get; set; }
        [Export] private VBoxContainer movementsContainer { get; set; }
        [Export] private Label movementTemplate { get; set; }
        [Export] private Button movementCreateSignalButton { get; set; }
        [Export] private Label signalSelectedLabel { get; set; }
        [Export] private Button signalCycleStateButton { get; set; }
        [Export] private Button signalDeleteButton { get; set; }
        [Export] private LineEdit stationNameField { get; set; }
        [Export] private Button stationPlatformAboveButton { get; set; }
        [Export] private Button stationPlatformBelowButton { get; set; }
        [Export] private Button stationDeleteButton { get; set; }
        [Export] private Button jsonLoadButton { get; set; }
        [Export] private Button jsonSaveButton { get; set; }

        private Editor _editor;
        private EditorSelectionManager _sm;
        private FileManager _fileManager;

        public override void _Process(double delta)
        {
            LightUpdateUi();
        }

        public override void _Ready()
        {
            nodePrefixField.TextSubmitted += uiNodePrefixFieldChanged;
            nodeDeleteButton.Pressed += uiNodeDeleteButtonPressed;
            edgeCreateButton.Pressed += uiEdgeCreateButtonPressed;
            edgeIsStumpCheckbox.Toggled += uiEdgeStumpCheckboxToggled;
            edgeLength.TextSubmitted += uiEdgeLengthFieldChanged;
            edgeSpeedLimit.TextSubmitted += uiEdgeSpeedFieldChanged;
            edgeDeleteButton.Pressed += uiEdgeDeleteButtonPressed;
            movementCreateSignalButton.Pressed += uiSignalCreateButtonPressed;
            signalCycleStateButton.Pressed += uiSignalCycleButonPressed;
            signalDeleteButton.Pressed += uiSignalDeleteButtonPressed;
            stationNameField.TextSubmitted += uiStationNameFieldChanged;
            stationPlatformAboveButton.Pressed += uiStationPlatformAboveButtonPressed;
            stationPlatformBelowButton.Pressed += uiStationPlatformBelowButtonPressed;
            stationDeleteButton.Pressed += uiStationDeleteButtonPressed;
            jsonLoadButton.Pressed += uiJsonLoadButtonPressed;
            jsonSaveButton.Pressed += uiJsonSaveButtonPressed;

            _editor = GetNode<Editor>("/root/Map/Editor");
            _sm = _editor.selectionManager;
            _fileManager = GetNode<FileManager>("/root/Map/FileManager");
            
            _sm.SelectionChanged += OnSelectionChange;
        }

        private void uiNodePrefixFieldChanged(string text) =>
        MapFactory.CurrentIdPrefix = text;
        private void uiNodeDeleteButtonPressed() =>
        _editor.FireUiEvent(EditorUiEvent.NodeDeleteButtonPressed);
        private void uiEdgeCreateButtonPressed()
        { _editor.FireUiEvent(EditorUiEvent.EdgeCreateButtonPressed,
            new EditorUiCreateEdgeArgs()
            { EdgeLength = edgeLength.Text,
            EdgeSpeed = edgeSpeedLimit.Text,
            IsStump = edgeIsStumpCheckbox.ButtonPressed }); }
        private void uiEdgeStumpCheckboxToggled(bool state) {}
        private void uiEdgeLengthFieldChanged(string text)
        { if (!new Regex("^[0-9]*$").IsMatch(text)) edgeLength.Text = string.Empty; }
        private void uiEdgeSpeedFieldChanged(string text)
        { if (!new Regex("^[0-9]*$").IsMatch(text)) edgeLength.Text = string.Empty; }
        private void uiEdgeDeleteButtonPressed() =>
        _editor.FireUiEvent(EditorUiEvent.EdgeDeleteButtonPressed); 
        private void uiSignalCreateButtonPressed() =>
        _editor.FireUiEvent(EditorUiEvent.CreateSignalPressed); 
        private void uiSignalCycleButonPressed() =>
        _editor.FireUiEvent(EditorUiEvent.CycleSignalPressed); 
        private void uiSignalDeleteButtonPressed() =>
        _editor.FireUiEvent(EditorUiEvent.DeleteSignalPressed); 
        private void uiStationNameFieldChanged(string text) =>
        _editor.FireUiEvent(EditorUiEvent.StationSelected); 
        private void uiStationPlatformAboveButtonPressed() =>
        _editor.FireUiEvent(EditorUiEvent.StationPlatformAbovePressed); 
        private void uiStationPlatformBelowButtonPressed() =>
        _editor.FireUiEvent(EditorUiEvent.StationPlatformBelowPressed); 
        private void uiStationDeleteButtonPressed() =>
        _editor.FireUiEvent(EditorUiEvent.StationDeletePressed); 
        private void uiJsonLoadButtonPressed() => _fileManager.LoadData(); 
        private void uiJsonSaveButtonPressed() => _fileManager.SaveData(); 

        private void OnSelectionChange()
        {
            HandleMovementSelection();
            LightUpdateUi();
        }

        private void HandleMovementSelection()
        {
            if (_sm.Edges.Items.Count <= 0) return;
            MapEdge edge = _sm.Edges.Items[0];
            List<MapMovement> movements = edge.From.Movements.Concat(edge.To.Movements).ToList();
            loadMovementList(movements);
        }

        private void loadMovementList(List<MapMovement> movements)
        { 
            foreach (Node child in movementsContainer.GetChildren()) 
                child.QueueFree();

            foreach (MapMovement movement in movements)
            {
                Label movementLabel = (Label)movementTemplate.Duplicate();
                movementLabel.Text = $"{movement.from.Id}<>{movement.to.Id}";
                movementLabel.Visible = true;
                movementsContainer.AddChild(movementLabel);
            }
        }

        private void LightUpdateUi()
        {
            nodeLabel.Text = GetIdOrNaN(_sm.Nodes.Items, 0);
            edgeSelectedLabel.Text = GetIdOrNaN(_sm.Edges.Items, 0);
            edgeFromLabel.Text = GetIdOrNaN(_sm.Nodes.Items, 0);
            edgeToLabel.Text = GetIdOrNaN(_sm.Nodes.Items, 1);
            signalSelectedLabel.Text = GetIdOrNaN(_sm.Signals.Items, 0);
        }
        private string GetIdOrNaN<T>(List<T> list, int index) where T : class
        {
            return index < list.Count && list[index] != null
                ? (list[index] as dynamic).Id.ToString()
                : "NaN";
        }
    }
}