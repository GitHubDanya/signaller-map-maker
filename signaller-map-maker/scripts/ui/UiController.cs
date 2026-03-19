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
        [Export] private Button nodeUpdateButton { get; set; }
        [Export] private Button nodeDeleteButton { get; set; }
        [Export] private Label edgeSelectedLabel { get; set; }
        [Export] private Label edgeFromLabel { get; set; }
        [Export] private Label edgeToLabel { get; set; }
        [Export] private Button edgeCreateButton { get; set; }
        [Export] private CheckBox edgeIsStumpCheckbox { get; set; }
        [Export] private LineEdit edgeLength { get; set; }
        [Export] private LineEdit edgeSpeedLimit { get; set; }
        [Export] private Button edgeDeleteButton { get; set; }
        [Export] private Button jsonLoadButton { get; set; }
        [Export] private Button jsonSaveButton { get; set; }

        private Editor _editor;
        private FileManager _fileManager;

        public override void _Process(double delta)
        {
            UpdateUi();
        }

        public override void _Ready()
        {
            nodePrefixField.TextSubmitted += uiNodePrefixFieldChanged;
            nodeUpdateButton.Pressed += uiNodeUpdateButtonPressed;
            nodeDeleteButton.Pressed += uiNodeDeleteButtonPressed;
            edgeCreateButton.Pressed += uiEdgeCreateButtonPressed;
            edgeIsStumpCheckbox.Toggled += uiEdgeStumpCheckboxToggled;
            edgeLength.TextSubmitted += uiEdgeLengthFieldChanged;
            edgeSpeedLimit.TextSubmitted += uiEdgeSpeedFieldChanged;
            edgeDeleteButton.Pressed += uiEdgeDeleteButtonPressed;
            jsonLoadButton.Pressed += uiJsonLoadButtonPressed;
            jsonSaveButton.Pressed += uiJsonSaveButtonPressed;

            _editor = GetNode<Editor>("/root/Map/Editor");
            _fileManager = GetNode<FileManager>("/root/Map/FileManager");
        }

        private void uiNodePrefixFieldChanged(string text)
        { _editor.NodePrefix = text.Length >= 2 ? text.Substring(0, 2) : text; }
        private void uiNodeUpdateButtonPressed()
        { _editor.UpdateNode(); }
        private void uiNodeDeleteButtonPressed()
        { CommandManager.ExecuteCommand(new DeleteNodeCommand(_editor, null)); }
        private void uiEdgeCreateButtonPressed()
        { if (_editor.SelectedNodes[0] == null || _editor.SelectedNodes[1] == null) return;
          if (int.TryParse(edgeLength.Text, out int el) == false || int.TryParse(edgeSpeedLimit.Text, out int esl) == false) return;
          _editor.CreateEdge(_editor.SelectedNodes[0], _editor.SelectedNodes[1], el, esl, edgeIsStumpCheckbox.ButtonPressed); }
        private void uiEdgeStumpCheckboxToggled(bool state) {}
        private void uiEdgeLengthFieldChanged(string text)
        { if (!new Regex("^[0-9]*$").IsMatch(text)) edgeLength.Text = string.Empty; }
        private void uiEdgeSpeedFieldChanged(string text)
        { if (!new Regex("^[0-9]*$").IsMatch(text)) edgeLength.Text = string.Empty; }
        private void uiEdgeDeleteButtonPressed()
        { CommandManager.ExecuteCommand(new DeleteEdgeCommand(_editor, null)); }
        private void uiJsonLoadButtonPressed() { _fileManager.LoadData(); }
        private void uiJsonSaveButtonPressed() { _fileManager.SaveData(); }
        private void UpdateUi()
        {
            nodeLabel.Text = _editor.SelectedNodes[0]?.Id ?? "NaN";
            edgeFromLabel.Text = _editor.SelectedNodes[0]?.Id ?? "NaN";
            edgeToLabel.Text = _editor.SelectedNodes[1]?.Id ?? "NaN";
            edgeSelectedLabel.Text = _editor.SelectedEdge?.Id ?? "NaN";
        }
    }
}