using System.ComponentModel;
using Gale.AST;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.CompilerServices.SymbolWriter;

namespace Gale.CodeGeneration;

public class FunctionSymbol
{
    public string Identifier { get; set; }
    public MethodDefinition? Method { get; set; }
}

public class VariableSymbol
{
    public string Identifier { get; set; }
    public string Type { get; set; }
    public bool IsOutOfScope { get; set; } = true;
    public VariableDefinition Definition { get; set; }
}

public class GaleGenerator
{
    private ModuleParameters _moduleParameters;
    private AssemblyDefinition _assembly;
    //PACKAGE FUNCTIONS TABLE
    private Dictionary<string, FunctionSymbol> _functionSymbols = new();

    public GaleGenerator(ModuleParameters moduleParameters, string filename)
    {
        _moduleParameters = moduleParameters;
        _assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Gale", Version.Parse("1.0.0.0")),
            Path.GetFileName(filename), _moduleParameters);
    }
    
    public void GenerateProgram(SourceFileAst root)
    {
        //GENERATE MAIN MODULE TO PROGRAM CLASS
        //GENERATE STATIC METHODS FOR FUNCTIONS
        //GENERATE MAIN METHOD
        if (root.Main != null)
        {
            var program = new TypeDefinition(root.ModuleName, "Program", 
                TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed, 
                _assembly.MainModule.TypeSystem.Object);
            var mainDefinition = GenerateMethodFromFunction("Main", root.Main);
            program.Methods.Add(mainDefinition);
        }
    }

    public MethodDefinition GenerateMethodFromFunction(string name, FunctionDeclarationAst funcAst)
    {
        var methodDefinition = new MethodDefinition(name, 
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, 
            _assembly.MainModule.TypeSystem.Void);
        methodDefinition.Body.InitLocals = true;
        var ilBody = methodDefinition.Body.GetILProcessor();
        var symbolTable = new Dictionary<string, VariableSymbol>();
        GenerateBlock(funcAst.Block, ilBody, methodDefinition, symbolTable);
        ilBody.Emit(OpCodes.Ret);
        return methodDefinition;
    }

    public void GenerateBlock(BlockAst block, ILProcessor ilBody, MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        foreach (var statement in block.Statements)
        {
            switch (statement)
            {
                case AssigmentStatementAst assigment:
                {
                    GenerateAssigment(assigment, ilBody, method, symbols);
                    break;
                }
                case MassAssigmentStatementAst massAssigment:
                {
                    GenerateMassAssigment(massAssigment, ilBody, method, symbols);
                    break;
                }
                case MassDeclarationStatementAst:
                {
                    break;
                }
                case ExpressionStatementAst:
                {
                    break;
                }
                default:
                    break;
            }
            //GenerateSimpleStatement(ilBody, method, symbols);
        }
    }

    public void GenerateMassAssigment(MassAssigmentStatementAst assigmentStmt, ILProcessor ilBody,
        MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        foreach (var assigment in assigmentStmt.Assigments)
        {
            GenerateAssigment(assigment, ilBody, method, symbols);
        }
    }
    
    public void GenerateAssigment(AssigmentStatementAst assigmentStmt, ILProcessor ilBody, MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        GenerateExpression(assigmentStmt.Expression, ilBody, method, symbols);
        var varSymbol = symbols[assigmentStmt.Identifier.Name];//TODO: add check
        ilBody.Emit(OpCodes.Stloc,  varSymbol.Definition);
    }

    public void GenerateExpression(ExpressionAst expression, ILProcessor ilBody,
        MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        switch (expression)
        {
            case IntegerLiteral literal:
                ilBody.Emit(OpCodes.Ldc_I4, literal.Value);
                break;
            case BinaryExpressionAst binary:
                GenerateBinaryExpression(binary, ilBody, method, symbols);
                break;
        }
    }

    public void GenerateBinaryExpression(BinaryExpressionAst expression, ILProcessor ilBody,
        MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        GenerateExpression(expression.LeftOperand, ilBody, method, symbols);
        GenerateExpression(expression.RightOperand, ilBody, method, symbols);
        switch (expression.Operation)
        {
            case BinaryExpressionType.Plus:
                ilBody.Emit(OpCodes.Add);
                break;
            case BinaryExpressionType.Mul:
                ilBody.Emit(OpCodes.Mul);
                break;
            case BinaryExpressionType.Minus:
                ilBody.Emit(OpCodes.Sub);
                break;
            case BinaryExpressionType.Div:
                ilBody.Emit(OpCodes.Div);
                break;
            default:
                throw new InvalidEnumArgumentException($"Not implemented {expression.LeftOperand} {expression.Operation} {expression.RightOperand}");
        }
    }

    public void GenerateMassDeclaration(MassDeclarationStatementAst massDeclaration, ILProcessor ilBody,
        MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        foreach (var declaration in massDeclaration.Declarations)
        {
            GenerateDeclaration(declaration, ilBody, method, symbols);
        }
    }

    public void GenerateDeclaration(DeclarationStatementAst declaration, ILProcessor ilBody,
        MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        if (declaration.Expression is not null)
            GenerateExpression(declaration.Expression, ilBody, method, symbols);
        var lv_a_8 = new VariableDefinition(_assembly.MainModule.TypeSystem.Int32); //TODO: NEED TYPE
        //md_Main_6.Body.Variables.Add(lv_a_8);
    }
}