using Gale.Compiler;

namespace Gale.Ast;

public abstract class Statement : AstNode { }

public class BlockStatement : Statement
{
    public BlockData Data { get; }
    public BlockStatement(BlockData data) => Data = data;
}

public class ExpressionStatement : Statement
{
    public Expression Expr { get; }
    public ExpressionStatement(Expression expr) => Expr = expr;
}

public class BlockData : AstNode
{
    public List<Statement> Statements { get; }
    public BlockData(List<Statement> statements) => Statements = statements ?? new List<Statement>();
    public static BlockData Empty => new BlockData(new List<Statement>());
}