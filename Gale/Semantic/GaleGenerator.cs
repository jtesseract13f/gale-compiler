using Gale.Ast;
using Gale.Compiler;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Gale.Semantic;

public class GaleGenerator (AssemblyDefinition assembly)
{
    //public AssemblyDefinition AssemblyDefinition { get; set; }
    public void AddFunctionToPackage(TypeDefinition package, FunctionAst function)
    {
        //Method : Bar
        
        //var returnType = new TypeReference()
        var func = new MethodDefinition(function.Identifier, MethodAttributes.Private | MethodAttributes.HideBySig, assembly.MainModule.TypeSystem.Void);
        package.Methods.Add(func);
        var types = package.NestedTypes;
        func.Body.InitLocals = true;
        var il_Bar_5 = func.Body.GetILProcessor();
        //BLOCK CODES
        il_Bar_5.Emit(OpCodes.Ret);
    }
    
    
}