using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using signallerMap.Scripts.Data;
using System.Linq;

namespace signallerMap.Scripts.editor
{
    internal static class MapCommand
    {
        public static ICommand CreateNode(Editor e, MapNode n) =>
            new MapActionCommand<MapNode>(n, e.CreateNode, e.DeleteNode);
        public static ICommand CreateEdge(Editor e, MapEdge ed) =>
            new MapActionCommand<MapEdge>(ed, e.CreateEdge, e.DeleteEdge);
        public static ICommand CreateMovement(Editor e, MapMovement m) =>
            new MapActionCommand<MapMovement>(m, e.CreateMovement, e.DeleteMovement);
        public static ICommand CreateSignal(Editor e, MapSignal s) =>
            new MapActionCommand<MapSignal>(s, e.CreateSignal, e.DeleteSignal);
        public static ICommand DeleteNode(Editor e, MapNode n) =>
            new MapActionCommand<MapNode>(n, e.DeleteNode, e.CreateNode);
        public static ICommand DeleteEdge(Editor e, MapEdge ed) =>
            new MapActionCommand<MapEdge>(ed, e.DeleteEdge, e.CreateEdge);
        public static ICommand DeleteMovement(Editor e, MapMovement m) =>
            new MapActionCommand<MapMovement>(m, e.DeleteMovement, e.CreateMovement);
        public static ICommand DeleteSignal(Editor e, MapSignal s) =>
            new MapActionCommand<MapSignal>(s, e.DeleteSignal, e.CreateSignal);
    }

    internal class MapActionCommand<T> : ICommand where T : class
    {
        private readonly T _target;
        private readonly Action<T> _doAction;
        private readonly Action<T> _undoAction;

        public bool IsValid => _target != null;

        public MapActionCommand(T target, Action<T> doAction, Action<T> undoAction)
        {
            _target = target;
            _doAction = doAction;
            _undoAction = undoAction;
        }

        public void Execute() => _doAction?.Invoke(_target);
        public void Undo() => _undoAction?.Invoke(_target);
    }

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

    internal interface ICommand
    {
        void Execute();
        void Undo();
        bool IsValid { get; }
    }
}