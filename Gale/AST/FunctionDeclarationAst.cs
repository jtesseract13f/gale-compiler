namespace Gale.AST;

public abstract class StatementAst : AstNode;
public abstract class OperandAst : ExpressionAst;
public abstract class Literal : OperandAst;

public class SourceFileAst : AstNode
{
    public List<FunctionDeclarationAst> Functions { get; set; } = new();
    public List<string> Packages { get; set; } = new();
    public string ModuleName { set; get; }
    public FunctionDeclarationAst? Main { get; set; }
}
public class FunctionDeclarationAst : AstNode
{
    public string Name { get; set; }
    public BlockAst? Block { get; set; }
    public List<ParameterAst> Parameters { get; set; } = new List<ParameterAst>();
    public string ReturnType { get; set; } = "void";
}

public class BlockAst : AstNode
{
    public List<StatementAst> Statements { get; set; } = new List<StatementAst>();
}

public class ParameterAst : AstNode
{
    public string Name { get; set; }
    public string Type { get; set; }
}

public class ExpressionStatementAst : StatementAst { public ExpressionAst ExpressionAst { get; set; } }

public class MassAssigmentStatementAst : StatementAst { public List<AssigmentStatementAst> Assigments { get; set; } = new(); }

public class AssigmentStatementAst : StatementAst
{
    public IdentifierAst Identifier { get; set; }
    public ExpressionAst Expression { get; set; }
}

public class DeclarationStatementAst : StatementAst
{
    public IdentifierAst Identifier { get; set; }
    public string Type { get; set; }
    public ExpressionAst? Expression { get; set; }
}

public class ArrayDeclarationStatementAst : DeclarationStatementAst
{
    public IdentifierAst Identifier { get; set; }
    public string Type { get; set; }
    public List<int> Dimensions { get; set; }
    public ExpressionAst? Expression { get; set; }
}

public class ReturnStatementAst : StatementAst
{
    public bool IsNoReturn { get; set; } = true;
    public ExpressionAst? ReturnedExpression { get; set; }
}

public class IfStatementAst : StatementAst
{
    public ExpressionAst? BoolExpression { get; set; }
    public BlockAst Block { get; set; }
    public IfStatementAst? IfStatement { get; set; }
    public IfStatementAst? ElseStatement { get; set; }
}

public class WhileStatementAst : StatementAst
{
    
}
public class MassDeclarationStatementAst : StatementAst
{
    public List<DeclarationStatementAst> Declarations { get; set; } = new List<DeclarationStatementAst>();
}

public abstract class ExpressionAst : AstNode
{
    public abstract string GetExpressionType();
}

public class BinaryExpressionAst : ExpressionAst
{
    public ExpressionAst LeftOperand { get; set; }
    public ExpressionAst RightOperand { get; set; }
    public BinaryExpressionType Operation { get; set; }

    public override string GetExpressionType()
    {
        var left = LeftOperand.GetExpressionType();
        var right = RightOperand.GetExpressionType();
        if (left != right) throw new Exception($"Binary expression error: {left} != {right}");
        return left ?? right;
    }
}

public enum BinaryExpressionType
{
    Plus,        // +
    Minus,       // -
    Mul,         // *
    Div,         // /
    Mod,         // %
    
    BitwiseAnd,  // &
    BitwiseOr,   // |
    LShift,      // <<
    RShift,      // >>
    
    Equals,        // ==
    NotEquals,     // !=
    Less,          // <
    LessOrEquals,  // <=
    Greater,       // >
    GreaterOrEquals, // >=
    
    LogicalAnd,  // &&
    LogicalOr,   // ||
    
    Receive,     // <-
}

public class FunctionCallAst : ExpressionAst
{
    public IdentifierAst Identifier { get; set; }
    public string ReturnedType { get; set; } = "undefined";
    public List<ExpressionAst> Parameters { get; set; } = new List<ExpressionAst>();
    public override string GetExpressionType()
    {
        return ReturnedType;
    }
}

public class IdentifierAst : OperandAst
{
    public string Name { get; set; }
    public string Type { get; set; } = "undefined";

    public List<string> QualifiedIdentifiers { get; set; } = new List<string>();
    //TYPE?
    public override string GetExpressionType()
    {
        return Type;
    }
}

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