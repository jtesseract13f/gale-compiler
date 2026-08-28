namespace Gale.Ast;

public abstract class Expression : AstNode { }

public class CallExpression : Expression
{
    public CallData Data { get; }
    public CallExpression(CallData data) => Data = data;
}

public class SelectorExpression : Expression
{
    public SelectorData Data { get; }
    public SelectorExpression(SelectorData data) => Data = data;
}

public class StringLiteral : Expression
{
    public string Value { get; }
    public StringLiteral(string value) => Value = value;
}

public class VariableExpression : Expression
{
    public VariableData Data { get; }
    public VariableExpression(VariableData data) => Data = data;
}

public class VariableData
{
    public string Name { get; }
    public string? Package { get; }
    public VariableData(string name, string? package)
    {
        Name = name;
        Package = package;
    }
}

public class CallData
{
    public Expression Function { get; }
    public List<Expression> Arguments { get; }
    public CallData(Expression function, List<Expression> arguments)
    {
        Function = function;
        Arguments = arguments ?? new List<Expression>();
    }
}

public class SelectorData
{
    public Expression Parent { get; }
    public string Name { get; }
    public SelectorData(Expression parent, string name)
    {
        Parent = parent;
        Name = name;
    }
}