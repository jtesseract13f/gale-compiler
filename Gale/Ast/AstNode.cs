using Gale.Compiler;
using Type = Gale.Ast.Type;

namespace Gale.Ast;

public abstract class AstNode;

public abstract class FieldBody : AstNode { }

public class IdentifiedField(List<string> identifiers, Type type) : FieldBody
{
    public List<string> Identifiers { get; } = identifiers ?? new List<string>();
    public Type Type { get; } = type;
}

public class EmbeddedField(TypeName typeName) : FieldBody
{
    public TypeName TypeName { get; } = typeName;
}

public class FunctionSignature(List<Parameter> parameters, FunctionResult? result) : AstNode
{
    public List<Parameter> Parameters { get; } = parameters ?? new List<Parameter>();
    public FunctionResult? Result { get; } = result;
}

public abstract class FunctionResult : AstNode { }

public class ResultParameters(List<Parameter> parameters) : FunctionResult
{
    public List<Parameter> Parameters { get; } = parameters ?? new List<Parameter>();
}

public class ResultType(Type type) : FunctionResult
{
    public Type Type { get; } = type;
}

public class Parameter(List<string> identifiers, Type parameterType) : AstNode
{
    public List<string> Identifiers { get; } = identifiers ?? new List<string>();
    public Type ParameterType { get; } = parameterType;
}





