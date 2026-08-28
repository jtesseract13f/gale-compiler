namespace Gale.Ast;

public class FunctionDeclData(string name, FunctionSignature signature, BlockData? body)
{
    public string Name { get; } = name;
    public FunctionSignature Signature { get; } = signature;
    public BlockData? Body { get; } = body;
}

public class TopLevelFunctionDecl(FunctionDeclData data) : TopLevelDecl
{
    public FunctionDeclData Data { get; } = data;
}

public class TopLevelMethodDecl : TopLevelDecl { }  

public abstract class TopLevelDecl : AstNode { }

public class TopLevelDeclaration(Declaration declaration) : TopLevelDecl
{
    public Declaration Declaration { get; } = declaration;
}

public abstract class Declaration : AstNode { }

public class ConstDecl : Declaration { }  
public class TypeDecl : Declaration { }   
public class VarDecl : Declaration { }  