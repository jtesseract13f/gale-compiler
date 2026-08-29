using Gale.Compiler;

namespace Gale.Ast;

public abstract class TypeName : AstNode { }

public class IdentifiedType : TypeName
{
    public string Identifier { get; }
    public IdentifiedType(string identifier) => Identifier = identifier;
}

public class QualifiedType : TypeName
{
    public QualifiedIdent QualifiedIdent { get; }
    public QualifiedType(QualifiedIdent qualifiedIdent) => QualifiedIdent = qualifiedIdent;
}

public abstract class Type : AstNode { }

public class NamedType(TypeName typeName) : Type
{
    public TypeName TypeName { get; } = typeName;
}

public class ArrayType(Expression length, Type elementType) : Type
{
    public Expression Length { get; } = length;
    public Type ElementType { get; } = elementType;
}

public class StructType(StructTypeData data) : Type
{
    public StructTypeData Data { get; } = data;
}

public class PointerType(Type baseType) : Type
{
    public Type BaseType { get; } = baseType;
}

public class FunctionType : Type { }
public class InterfaceType : Type { }
public class SliceType : Type { }
public class MapType : Type { }
public class ChannelType : Type { }

public class StructTypeData
{
    public List<FieldDecl> Fields { get; }
    public StructTypeData(List<FieldDecl> fields) => Fields = fields ?? new List<FieldDecl>();
}

public class FieldDecl : AstNode
{
    public string Tag { get; }
    public FieldBody Body { get; }
    public FieldDecl(string tag, FieldBody body)
    {
        Tag = tag;
        Body = body;
    }
}