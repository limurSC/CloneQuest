using System.Collections.Generic;

public class SubscribersSet<TSubscriber> : HashSet<TSubscriber>, IEnumerable<TSubscriber>
{
    private readonly List<(TSubscriber subscriber, bool isAdd)> _buffer = new();
    private bool _unlock = true;

    public new void Add(TSubscriber subscriber)
    {
        if (_unlock) { base.Add(subscriber); }
        else { _buffer.Add((subscriber, true)); }
    }

    public new void Remove(TSubscriber subscriber)
    {
        if (_unlock) { base.Remove(subscriber); }
        else { _buffer.Add((subscriber, false)); }
    }

    public void Lock()
    {
        _unlock = false;
    }

    public void Unlock()
    {
        _unlock = true;
        if (_buffer.Count == 0) { return; }
        foreach (var (subscriber, isAdd) in _buffer)
        {
            if (isAdd) { base.Add(subscriber); }
            else { base.Remove(subscriber); }
        }
        _buffer.Clear();
    }
}