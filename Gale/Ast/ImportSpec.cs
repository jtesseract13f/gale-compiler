namespace Gale.Ast;

public class ImportSpec : AstNode
{
    public List<string> PackagePath { get; }
    public string ImportPath { get; }
    public ImportSpec(List<string> packagePath, string importPath)
    {
        PackagePath = packagePath ?? new List<string>();
        ImportPath = importPath;
    }
}