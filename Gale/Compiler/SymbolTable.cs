namespace Gale.Compiler;

public class SymbolTable
{
    private Scope _globalScope;
    private Dictionary<string, Scope> _packageScopes = new(); // имя пакета → область пакета
    private Scope _currentScope; // текущая область (для обхода AST)

    public SymbolTable()
    {
        _globalScope = new Scope(null, "global");
        _currentScope = _globalScope;
        // Предопределить встроенные типы и функции (int, string, make, len и т.д.)
        PredefineBuiltins();
    }

    public void EnterScope(string name)
    {
        _currentScope = new Scope(_currentScope, name);
    }

    public void ExitScope()
    {
        if (_currentScope.Parent != null)
            _currentScope = _currentScope.Parent;
        // иначе нельзя выйти из глобальной
    }

    public void DefineSymbol(Symbol symbol)
    {
        _currentScope.Define(symbol);
    }

    public Symbol Lookup(string name)
    {
        return _currentScope.Lookup(name);
    }

    public Symbol LookupInPackage(string pkgName, string name)
    {
        if (_packageScopes.TryGetValue(pkgName, out var pkgScope))
            return pkgScope.Lookup(name, lookInParent: false);
        return null;
    }

    // Создание области для пакета
    public Scope CreatePackageScope(string pkgName)
    {
        var scope = new Scope(_globalScope, $"package {pkgName}");
        _packageScopes[pkgName] = scope;
        return scope;
    }

    // Активная область пакета (при компиляции конкретного пакета)
    public Scope CurrentPackage { get; set; }

    private void PredefineBuiltins()
    {
        // Добавить в глобальную область встроенные типы
        // и функции (make, len, cap, panic и т.д.)
        // ...
    }
}