using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    [SerializeField] private bool enableUndoAfterExecute = false;

    private readonly Stack<GameCommand> _buffer = new Stack<GameCommand>();  // 플래닝 입력(최근 취소 LIFO)
    private readonly Queue<GameCommand> _queue = new Queue<GameCommand>();  // 커밋 후 실행 FIFO
    private readonly Stack<ExecutionRecord> _undo = new Stack<ExecutionRecord>(); // (선택) 실행 후 Undo

    public CommandContext Context { get; private set; } = new CommandContext(0);

    // 외부에서 컨텍스트 주입(턴 시작 시 RNG 시드 등)
    public void Init(CommandContext ctx) => Context = ctx;

    // 커맨드를 플래닝 버퍼에 적재(유효하지 않으면 거부)
    public bool Buffer(GameCommand cmd)
    {
        if (cmd == null || !cmd.CanExecute(Context)) return false;
        _buffer.Push(cmd);
        GameManager.Event?.Post(EventType.CommandBuffered, this, new CommandEventPayload(cmd));
        return true;
    }

    // 최근 플래닝 입력 취소(프리뷰만 적용했다면 상태 변경 없음)
    public bool UndoBuffered()
    {
        if (_buffer.Count == 0) return false;
        var cmd = _buffer.Pop();
        GameManager.Event?.Post(EventType.CommandUndone, this, new CommandEventPayload(cmd));
        return true;
    }

    // 버퍼를 순서 보존하여 실행 큐로 커밋
    public void CommitBufferToQueue()
    {
        if (_buffer.Count == 0) return;
        var arr = _buffer.ToArray(); System.Array.Reverse(arr); _buffer.Clear();
        foreach (var c in arr)
        {
            _queue.Enqueue(c);
            GameManager.Event?.Post(EventType.CommandCommitted, this, new CommandEventPayload(c));
        }
    }

    // 버퍼를 매크로로 묶어 원자적 실행이 되도록 커밋
    public void CommitBufferAsMacro()
    {
        if (_buffer.Count == 0) return;
        var arr = _buffer.ToArray(); System.Array.Reverse(arr); _buffer.Clear();
        var macro = new MacroCommand(arr);
        _queue.Enqueue(macro);
        GameManager.Event?.Post(EventType.CommandCommitted, this, new CommandEventPayload(macro));
    }

    // 큐의 다음 커맨드 실행(성공 시 선택적으로 Undo 스택에 기록)
    public bool ExecuteNext()
    {
        if (_queue.Count == 0) return false;

        var cmd = _queue.Dequeue();
        var rec = cmd.Execute(Context);
        if (!rec.Success) return false;

        if (enableUndoAfterExecute && cmd.SupportsUndo) _undo.Push(rec);

        GameManager.Event?.Post(EventType.CommandExecuted, this, new ExecutionEventPayload(rec));
        return true;
    }

    // 큐를 모두 비울 때까지 실행
    public void ExecuteAll()
    {
        while (ExecuteNext()) { }
    }

    // 마지막으로 실행된 커맨드를 롤백(대규모 롤백은 스냅샷 권장)
    public bool UndoLastExecuted()
    {
        if (!enableUndoAfterExecute || _undo.Count == 0) return false;

        var rec = _undo.Pop();
        if (rec.Command.SupportsUndo)
        {
            rec.Command.Undo(Context, rec.Memento);
            GameManager.Event?.Post(EventType.CommandUndone, this, new ExecutionEventPayload(rec));
            return true;
        }
        return false;
    }

    // 전체 초기화
    public void ClearAll()
    {
        _buffer.Clear(); _queue.Clear(); _undo.Clear();
    }

    // 편의 프로퍼티
    public int BufferedCount => _buffer.Count;
    public bool CanUndoBuffered => _buffer.Count > 0;
}
