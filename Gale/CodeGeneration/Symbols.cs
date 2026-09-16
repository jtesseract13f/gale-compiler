using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Gale.CodeGeneration;

public class MethodBody(SymbolScope? parentScope, MethodDefinition method)
{
    public SymbolScope Scope { get; set; } = parentScope is null ? new SymbolScope() : new SymbolScope(parentScope);
    public ILProcessor IlBody { get; set; } = method.Body.GetILProcessor();
    public MethodDefinition MethodDefinition { get; set; } = method;
}
public class SymbolScope
{
    public SymbolScope? ParentScope { get; set; }
    public Dictionary<string, Symbol> Symbols { get; set; } = new();

    public SymbolScope() { }

    public SymbolScope(SymbolScope parent) { ParentScope = parent; }

    public bool IsExistsInCurrentScope(string identifier)
    {
        return Symbols.ContainsKey(identifier);
    }

    public Symbol? GetSymbol(string identifier)
    {
        if (!IsExistsInCurrentScope(identifier))
        {
            if (ParentScope is not null) return ParentScope.GetSymbol(identifier);
            return null;
        }
        return Symbols[identifier];
    }
}
public abstract class Symbol
{
    public string Identifier { get; set; }
}

public class StructSymbol : Symbol
{
    public string TypeName { get; set; }
    public Dictionary<string, StructField> Fields { get; set; } = new();
    public VariableDefinition Definition { get; set; }
}

public class StructField
{
    public string FieldName { get; set; }
    public string FieldType { get; set; }
    public VariableDefinition Definition { get; set; }
}

public class FunctionSymbol : Symbol
{
    public string ReturnedType { get; set; }
    public MethodDefinition? Method { get; set; }
}

public class VariableSymbol : Symbol
{
    public string Type { get; set; }
    public VariableDefinition Definition { get; set; }
}

public class ArraySymbol : Symbol
{
    
}

public class ParameterSymbol : Symbol
{
    public string Type { get; set; }
    public ParameterDefinition ParameterDefinition { get; set; }
    public int ParameterNumber { get; set; }
}
