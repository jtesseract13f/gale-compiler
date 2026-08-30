namespace Gale.Ast;

public class FunctionAst : AstNode
{
    public string Identifier { get; set; } = "";
    public List<Parameter> Parameters { get; set; } = [];
    public BlockAst Body { get; set; }
    
    // параметры
    // возвращаемое значение
    // название функции
    // блок кода
}

public class Parameter : AstNode
{
    public string Identifier { get; set; }
}

public class FunctionReturn : AstNode
{
    
}