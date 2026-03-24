using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using signallerMap.Scripts.Data;
using System.Linq;

namespace signallerMap.Scripts.editor
{
    internal static class CommandManager
    {
        private static readonly Stack<ICommand> _undoStack = new();
        private static readonly Stack<ICommand> _redoStack = new();

        public static void ExecuteCommand(ICommand command)
        {
            if (!command.IsValid) return;
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public static void Undo()
        {
            if (_undoStack.Count == 0) return;
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
        }

        public static void Redo()
        {
            if (_redoStack.Count == 0) return;
            var command = _redoStack.Pop();
            command.Execute();
            _redoStack.Push(command);
        }

        public static void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
    internal class CreateNodeCommand : ICommand
    {
        private readonly Editor _editor;
        private readonly MapNode _node;
        public bool IsValid => _node != null;
        public CreateNodeCommand(Editor editor, MapNode mapNode)
        {
            _editor = editor;
            _node = mapNode;
        }
        
        public void Execute()
        {
            _editor.CreateNode(_node);
        }

        public void Undo()
        {
            _editor.DeleteNode(_node);
        }
    }
    internal class DeleteNodeCommand : ICommand
    {
        private readonly Editor _editor;
        private readonly MapNode _node;
        public bool IsValid => _node != null;
        public DeleteNodeCommand(Editor editor, MapNode mapNode)
        {
            _editor = editor;
            _node = mapNode;
        }
        
        public void Execute()
        {
            if (_node == null) return;
            _editor.DeleteNode(_node);
        }

        public void Undo()
        {
            _editor.CreateNode(_node);
        }
    }

    internal class CreateEdgeCommand : ICommand
    {
        private readonly Editor _editor;
        private readonly MapEdge _edge;
        public bool IsValid => _edge != null;
        public CreateEdgeCommand(Editor editor, MapEdge edge)
        {
            _editor = editor;
            _edge = edge;
        }

        public void Execute()
        {
            _editor.CreateEdge(_edge);
        }

        public void Undo()
        {
            _editor.DeleteEdge(_edge);
        }
    }

    internal class DeleteEdgeCommand : ICommand
    {
        private readonly Editor _editor;
        private readonly MapEdge _edge;
        public bool IsValid => _edge != null;
        public DeleteEdgeCommand(Editor editor, MapEdge edge)
        {
            _editor = editor;
            _edge = edge;
        }

        public void Execute()
        {   
            if (_edge == null) return;
            _editor.DeleteEdge(_edge);
        }

        public void Undo()
        {
            _editor.CreateEdge(_edge);
        }
    }

    internal class CreateNodeMovementCommand : ICommand
    {
        private readonly Editor _editor;
        private readonly MapNode _node;
        private readonly MapMovement _movement;
        public bool IsValid => _node != null && _movement != null;
        public CreateNodeMovementCommand(Editor editor, MapMovement movement)
        {
            _editor = editor;
            _movement = movement;
            _node = movement?.GetNode();
        }

        public void Execute()
        {   
            if (_node == null || _movement == null) return;
            _editor.CreateMovement(_movement);
        }

        public void Undo()
        {
            if (_node == null || !_node.Movements.Contains(_movement)) return;
            _editor.DeleteNodeMovement(_movement);
        }
    }

    internal class DeleteNodeMovementCommand : ICommand
    {
        private readonly Editor _editor;
        private readonly MapNode _node;
        private readonly MapMovement _movement;
        public bool IsValid => _node != null && _movement != null;
        public DeleteNodeMovementCommand(Editor editor, MapMovement movement)
        {
            _editor = editor;
            _movement = movement;
            _node = movement?.GetNode();
        }

        public void Execute()
        {   
            if (_node == null || !_node.Movements.Contains(_movement)) return;
            _editor.DeleteNodeMovement(_movement);
        }

        public void Undo()
        {
            if (_node == null || _movement == null) return;
            _editor.CreateMovement(_movement);
        }
    }

    internal class CreateSignalCommand : ICommand
    {
        private readonly Editor _editor;
        private readonly MapSignal _signal;
        public bool IsValid => _signal != null;
        public CreateSignalCommand(Editor editor, MapSignal signal)
        {
            _editor = editor;
            _signal = signal;
        }

        public void Execute()
        {
            if (_signal == null) return;
            _editor.CreateSignal(_signal);
        }
        
        public void Undo()
        {
            _editor.DeleteSignal(_signal);
        }
    }

    internal interface ICommand
    {
        void Execute();
        void Undo();
        bool IsValid { get; }
    }
}