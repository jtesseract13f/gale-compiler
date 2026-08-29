using Gale.Ast;
using Gale.Compiler;

namespace Gale.Semantic;

public class SemanticAnalyzer
{
    private SymbolTable _symbols;

    public void Analyze(AstNode root)
    {
        _symbols = new SymbolTable();
        CollectDeclarations(root);
        ResolveAndCheck(root);
    }

    private void CollectDeclarations(AstNode node)
    {
        switch (node)
        {
            case SourceFile src:
                // Создаём область для пакета
                var pkgScope = _symbols.CreatePackageScope(src.Package);
                _symbols.CurrentPackage = pkgScope;
                _symbols.EnterScope(pkgScope.ScopeName);
                foreach (var decl in src.Declarations)
                    CollectDeclarations(decl);
                _symbols.ExitScope();
                break;

            case TopLevelFunctionDecl func:
                // Добавить символ функции в текущую область (пакет)
                var funcSym = new FunctionSymbol(func.Name, func.Signature);
                _symbols.DefineSymbol(funcSym);
                break;

            case TypeDecl typeDecl:
                // Добавить символ типа
                var typeSym = new TypeSymbol(typeDecl.Name, typeDecl.TypeNode);
                _symbols.DefineSymbol(typeSym);
                break;

            // ... для методов, переменных, констант
        }
    }

    private void ResolveAndCheck(AstNode node)
    {
        switch (node)
        {
            case NamedType named:
                // Разрешаем имя типа
                var sym = _symbols.Lookup(named.TypeName.Identifier);
                if (sym == null)
                    ReportError($"Type '{named.TypeName.Identifier}' not found");
                else
                {
                    // Заменяем NamedType на реальный Type (например, на StructType)
                    // Используем информацию из sym
                    var resolvedType = sym.Type; // предположим, это Type
                    ReplaceNodeWith(named, resolvedType);
                }
                break;

            case VariableExpr varExpr:
                // Разрешаем имя переменной
                var varSym = _symbols.Lookup(varExpr.Data.Name);
                if (varSym == null)
                    ReportError($"Variable '{varExpr.Data.Name}' not found");
                else
                {
                    // Запоминаем тип выражения для дальнейшего использования
                    varExpr.SemanticType = varSym.Type;
                }
                break;

            case Block block:
                // Входим в новую область для блока
                _symbols.EnterScope("block");
                // Обрабатываем операторы блока
                foreach (var stmt in block.Statements)
                    ResolveAndCheck(stmt);
                _symbols.ExitScope();
                break;

            // ... другие узлы
        }
    }
}