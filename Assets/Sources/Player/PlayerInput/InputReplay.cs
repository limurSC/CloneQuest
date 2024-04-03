using System.Collections;
using UnityEngine;

public class InputReplay
{
    private readonly IControllable _controllable;
    private readonly InputRecord _record;
    private Coroutine _coroutine;

    public InputReplay(IControllable controllable, InputRecord inputRecord)
    {
        _controllable = controllable;
        _record = inputRecord;
    }

    public bool IsRunning => _coroutine != null;
    public void Start()
    {
        if (IsRunning) { Stop(); }
        _coroutine = Coroutines.Run(InputRoutine());
    }
    public void Stop()
    {
        if (!IsRunning) { return; }
        Coroutines.Stop(_coroutine);
        _coroutine = null;
    }

    private IEnumerator InputRoutine()
    {
        foreach (var item in _record)
        {
            yield return new WaitForSeconds(item.wait);
            Execute(item);
        }
        _coroutine = null;
    }

    private void Execute(InputRecord.Item item)
    {
        switch (item.type)
        {
            case InputRecord.Type.MovePerform:
                _controllable.Move = (float)item.value;
                break;
            case InputRecord.Type.MoveEnd:
                _controllable.Move = 0f;
                break;
            case InputRecord.Type.JumpStart:
                _controllable.Jump = true;
                break;
            case InputRecord.Type.JumpEnd:
                _controllable.Jump = false;
                break;
            case InputRecord.Type.None:
                _controllable.Move = 0f;
                _controllable.Jump = false;
                break;
        }
    }

    ~InputReplay() => Stop();
}