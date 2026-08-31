namespace Gale.Ast;

public abstract class StatementAst : AstNode
{
    
}

public class BlockAst : StatementAst
{
    public List<StatementAst> Statements { get; set; } = new List<StatementAst>();
}
