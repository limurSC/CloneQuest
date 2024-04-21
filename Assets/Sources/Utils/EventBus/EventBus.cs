using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, SubscribersSet<object>> _subscribers = new();

    public static void Subscribe<TSubscriber>(TSubscriber subscriber) where TSubscriber : class
    {
        var type = typeof(TSubscriber);
        if (!_subscribers.TryGetValue(type, out var typeSubscribers))
        {
            if (!type.IsInterface) { throw new ArgumentException($"{type.Name} is not interface."); }
            typeSubscribers = new SubscribersSet<object>();
            _subscribers.Add(type, typeSubscribers);
        }
        typeSubscribers.Add(subscriber);
    }

    public static void Unsubscribe<TSubscriber>(TSubscriber subscriber)
    {
        var type = typeof(TSubscriber);
        if (!_subscribers.TryGetValue(type, out var typeSubscribers)) { return; }
        typeSubscribers.Remove(subscriber);
        if (typeSubscribers.Count == 0) { _subscribers.Remove(type); }
    }

    public static void Invoke<TSubscriber>(Action<TSubscriber> action) where TSubscriber : class
    {
        var type = typeof(TSubscriber);
        if (!_subscribers.TryGetValue(type, out var typeSubscribers)) { return; }
        typeSubscribers.Lock();
        foreach (var subscriber in typeSubscribers)
        {
            action.Invoke(subscriber as TSubscriber);
        }
        typeSubscribers.Unlock();
    }
}