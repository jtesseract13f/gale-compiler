namespace Gale.Ast;

public class DeclarationAst : StatementAst
{
    
}


public class VarDeclarationAst : DeclarationAst
{
    public string TypeName { get; set; }
    public string Identifier { get; set; }
    public ExpressionAst Expression { get; set; }
}
