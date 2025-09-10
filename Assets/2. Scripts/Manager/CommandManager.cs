using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    // (선택) 최근 입력 취소를 위한 버퍼 스택, 실행용 큐
    private readonly Stack<ICommand> _buffer = new Stack<ICommand>();
    private readonly Queue<ICommand> _queue = new Queue<ICommand>();

    // 편의 프로퍼티
    public int BufferedCount => _buffer.Count;
    public bool CanUndoBuffered => _buffer.Count > 0;

    // 버퍼에 커맨드 적재
    public bool Buffer(ICommand command)
    {
        if (command == null) return false;
        _buffer.Push(command);
        return true;
    }

    // 최근 버퍼 입력 취소
    public bool UndoBuffered()
    {
        if (_buffer.Count == 0) return false;
        _buffer.Pop();
        return true;
    }

    // 버퍼를 순서 보존하여 큐로 커밋
    public void CommitBufferToQueue()
    {
        if (_buffer.Count == 0) return;
        var arr = _buffer.ToArray();
        System.Array.Reverse(arr);
        _buffer.Clear();
        foreach (var c in arr) _queue.Enqueue(c);
    }

    // 큐의 다음 커맨드를 실행
    public bool ExecuteNext()
    {
        if (_queue.Count == 0) return false;
        var cmd = _queue.Dequeue();
        cmd.Execute();
        return true;
    }

    // 큐를 모두 실행
    public void ExecuteAll()
    {
        while (ExecuteNext()) {}
    }

    // 전체 초기화
    public void ClearAll()
    {
        _buffer.Clear();
        _queue.Clear();
    }


}
