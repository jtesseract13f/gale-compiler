namespace Gale.Compiler;

public class Scope
{
    public Scope Parent { get; }
    private Dictionary<string, Symbol> _symbols = new();
    public string ScopeName { get; } // для отладки: "global", "main", "function: foo"

    public Scope(Scope parent, string name = null)
    {
        Parent = parent;
        ScopeName = name ?? "unnamed";
    }

    public bool Define(Symbol symbol)
    {
        if (_symbols.ContainsKey(symbol.Name))
            return false; // уже объявлено
        _symbols[symbol.Name] = symbol;
        symbol.Scope = this;
        return true;
    }

    public Symbol Lookup(string name, bool lookInParent = true)
    {
        if (_symbols.TryGetValue(name, out var sym))
            return sym;
        if (lookInParent && Parent != null)
            return Parent.Lookup(name, true);
        return null;
    }

    // Для поиска только в текущей области (без родителей)
    public Symbol LookupLocal(string name) => _symbols.TryGetValue(name, out var s) ? s : null;
}