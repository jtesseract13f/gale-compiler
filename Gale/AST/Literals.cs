namespace Gale.AST;

public class IntegerLiteral : Literal
{
    public long Value { get; set; }
    public override string GetExpressionType()
    {
        return "int";
    }
}
public class StringLiteral : Literal
{
    public string Value { get; set; }
    public override string GetExpressionType()
    {
        return "string";
    }
}

public class CompositeLiteral : Literal
{
    public TypeAst Type { get; set; }
    public List<Element> Elements { get; set; } = [];
    public override string GetExpressionType()
    {
        throw new NotImplementedException();
    }
}

public class Element : AstNode
{
    public ExpressionAst? KeyExpression { get; set; }
    public ExpressionAst Expression { get; set; }
}

