using Gale.Ast;
using Mono.Cecil;

namespace Gale.Compiler;

public abstract class Symbol
{
    public string Name { get; }
    public SymbolKind Kind { get; } // Variable, Function, Type, Package, Const, Method, ...
    public TypeReference Type { get; set; } // семантический тип (после разрешения)
    public Scope Scope { get; set; } // ссылка на область, где объявлен
    public bool IsExported { get; } // для Go: имя с большой буквы
    public AstNode DeclarationNode { get; } // ссылка на AST-узел объявления (для ошибок)
    // дополнительные свойства для конкретных категорий
}

public enum SymbolKind
{
    Package,
    Type,
    Function,
    Method,
    Variable,
    Const,
    Builtin,
}