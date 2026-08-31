using Gale.Ast;
using Gale.Compiler;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Gale.Semantic;

public enum SymbolType
{
    Variable,
    Function,
    Method,
    Type
}
public class Symbol
{
    public string Scope { get; set; }
    public string Identifier { get; set; }
    public SymbolType SymbolType { get; set; }
}
public class GaleGenerator (AssemblyDefinition assembly)
{
    public Dictionary<string, Symbol> SymbolTable { get; set; } = new();
    public void AddFunctionToPackage(TypeDefinition package, FunctionAst function)
    {
        //Method : Bar
        
        //var returnType = new TypeReference()
        var func = new MethodDefinition(function.Identifier, MethodAttributes.Private | MethodAttributes.HideBySig, assembly.MainModule.TypeSystem.Void);
        package.Methods.Add(func);
        var types = package.NestedTypes;
        func.Body.InitLocals = true;
        //var il_Bar_5 = func.Body.GetILProcessor();
        //BLOCK CODES
        foreach (var statement in function.Body.Statements)
        {
            AddStatementToMethod(func, statement);
        }
        //
        //il_Bar_5.Emit(OpCodes.Ret);
    }

    public void AddStatementToMethod(MethodDefinition method, StatementAst statement)
    {
        var body = method.Body.GetILProcessor();
        if (statement is VarDeclarationAst varAst)
        {
            var variable =  new VariableDefinition(TypeHelper(varAst.TypeName));
            body.Body.Variables.Add(variable);
            //TODO: add var checks
            SymbolTable[varAst.Identifier] = new Symbol()
            {
                Identifier = varAst.Identifier,
                Scope = method.Name,
                SymbolType = SymbolType.Variable
            };
            //body.Emit(OpCodes.Newobj);
            
        }
        
        var lv_a_12 = new VariableDefinition(assembly.MainModule.TypeSystem.Int32);
        md_br2_10.Body.Variables.Add(lv_a_12);
        il_br2_11.Emit(OpCodes.Ldc_I4, 3);
        il_br2_11.Emit(OpCodes.Stloc, lv_a_12);
    }
    

    public TypeReference TypeHelper(string TypeName)
    {
        switch (TypeName)
        {
            case "int":
                return assembly.MainModule.TypeSystem.Int32;
            default:
                return assembly.MainModule.TypeSystem.Void;
        }
    }
}