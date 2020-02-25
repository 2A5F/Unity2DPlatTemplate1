using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
{
    private readonly ConcurrentBag<T> _objects = new ConcurrentBag<T>();
    private readonly Func<T> _object_generator;
    private readonly Func<T, T> _when_put;

    public ObjectPool(Func<T> objectGenerator) => _object_generator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
    public ObjectPool(Func<T> objectGenerator, Func<T, T> when_put) : this(objectGenerator) => _when_put = when_put;

    public T GetObject()
    {
        if (_objects.TryTake(out T item)) return item;
        return _object_generator();
    }

    public void PutObject(T item)
    {
        _objects.Add(_when_put != null ? _when_put(item) : item);
    }
}
