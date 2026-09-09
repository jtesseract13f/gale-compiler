namespace Gale.AST;

public class FunctionDeclarationAst : AstNode
{
    public BlockAst? Block { get; set; }
    public List<ParameterAst> Parameters { get; set; } = new List<ParameterAst>();
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

public abstract class StatementAst : AstNode
{
    //public List<ExpressionAst> Expressions { get; set; } = new List<ExpressionAst>();
}

public class ExpressionStatementAst : StatementAst
{
    public ExpressionAst ExpressionAst { get; set; }
}

public class MassAssigmentStatementAst : StatementAst
{
    public List<AssigmentStatementAst> Assigments { get; set; } = new List<AssigmentStatementAst>();
}

public class AssigmentStatementAst : StatementAst
{
    public string Identifier { get; set; }
    public List<ExpressionAst> Expressions { get; set; } = new List<ExpressionAst>();
}

public class DeclarationStatementAst : StatementAst
{
    public IdentifierAst Identifier { get; set; }
    public string Type { get; set; }
    public ExpressionAst? Expression { get; set; }
}

public class MassDeclarationStatementAst : StatementAst
{
    public List<DeclarationStatementAst> Declarations { get; set; } = new List<DeclarationStatementAst>();
}

public class ExpressionAst : AstNode
{
    
}

public class BinaryExpressionAst : ExpressionAst
{
    public ExpressionAst LeftOperand { get; set; }
    public ExpressionAst RightOperand { get; set; }
    public BinaryExpressionType Operation { get; set; }
}

public enum BinaryExpressionType
{
    Plus,
    Minus,
    Mul,
    Div,
    //TODO: Add logic
}

public class FunctionCallAst : ExpressionAst
{
    public IdentifierAst Identifier { get; set; }
    public List<ExpressionAst> Parameters { get; set; } = new List<ExpressionAst>();
}

public abstract class OperandAst : ExpressionAst;
public class IdentifierAst : OperandAst
{
    public string Name { get; set; }

    public List<string> QualifiedIdentifiers { get; set; } = new List<string>();
    //TYPE?
}


public abstract class Literal : OperandAst
{
    
}

public class IntegerLiteral : Literal
{
    public long Value { get; set; }
}