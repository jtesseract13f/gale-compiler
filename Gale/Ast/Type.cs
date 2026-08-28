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

public class NamedType : Type
{
    public TypeName TypeName { get; }
    public NamedType(TypeName typeName) => TypeName = typeName;
}

public class ArrayType : Type
{
    public Expression Length { get; }
    public Type ElementType { get; }
    public ArrayType(Expression length, Type elementType)
    {
        Length = length;
        ElementType = elementType;
    }
}

public class StructType : Type
{
    public StructTypeData Data { get; }
    public StructType(StructTypeData data) => Data = data;
}

public class PointerType : Type
{
    public Type BaseType { get; }
    public PointerType(Type baseType) => BaseType = baseType;
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