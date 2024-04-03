using System.Collections;
using System.Collections.Generic;

public class InputRecord : IEnumerable<InputRecord.Item>
{
    private readonly List<(Type type, float time)> _records = new();
    private readonly List<float> _floatValues = new();

    public void Add(Type type, float time, object value = null)
    {
        switch (type)
        {
            case Type.MovePerform:
                _records.Add((type, time));
                _floatValues.Add((float)value);
                break;
            default:
                _records.Add((type, time));
                break;
        }
    }

    public IEnumerator<Item> GetEnumerator()
    {
        var floatPointer = _floatValues.GetEnumerator();
        var previousTime = 0f;
        foreach (var (type, time) in _records)
        {
            var wait = time - previousTime;
            previousTime = time;
            switch (type)
            {
                case Type.MovePerform:
                    yield return new Item(type, wait, floatPointer.MoveNext() ? floatPointer.Current : 0f);
                    break;
                default:
                    yield return new Item(type, wait);
                    break;
            }
        }
    }

    public void Trim() => _records.TrimExcess();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public readonly struct Item
    {
        public readonly Type type;
        public readonly float wait;
        public readonly object value;
        public Item(Type type, float wait, object value = null)
        {
            this.type = type;
            this.wait = wait;
            this.value = value;
        }
    }

    public enum Type : ushort { None, JumpStart, JumpEnd, MovePerform, MoveEnd }
}
