using UniversalCaluclator.Models.Numbers;

namespace UniversalCaluclator.Models.Memory;

public enum MemoryState { Off, On }

public class TMemory
{
    private TANumber _mem;
    private MemoryState _state;

    public TMemory(TANumber initial)
    {
        _mem = initial.Copy();
        _state = MemoryState.Off;
    }

    public MemoryState State => _state;
    public string StateString => _state == MemoryState.On ? "M" : "";
    public string NumberString => _mem.NumberString;

    public void Store(TANumber n)
    {
        _mem = n.Copy();
        _state = MemoryState.On;
    }

    public void Add(TANumber n)
    {
        _mem = _mem.Add(n);
        _state = MemoryState.On;
    }

    public TANumber Restore() => _mem.Copy();

    public void Clear(TANumber zero)
    {
        _mem = zero.Copy();
        _state = MemoryState.Off;
    }
}
