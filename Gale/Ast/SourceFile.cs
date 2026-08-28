using Gale.Compiler;

namespace Gale.Ast;

public class SourceFile : AstNode
{
    public string Package { get; } 
    public List<ImportSpec> Imports { get; }
    public List<TopLevelDecl> Declarations { get; }

    public SourceFile(string package, List<ImportSpec> imports, List<TopLevelDecl> declarations)
    {
        Package = package;
        Imports = imports ?? new List<ImportSpec>();
        Declarations = declarations ?? new List<TopLevelDecl>();
    }
}
