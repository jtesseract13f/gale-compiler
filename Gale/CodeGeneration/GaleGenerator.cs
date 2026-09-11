using System.ComponentModel;
using Gale.AST;
using Gale.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.CompilerServices.SymbolWriter;

namespace Gale.CodeGeneration;

public class FunctionSymbol
{
    public string Identifier { get; set; }
    public string ReturnedType { get; set; }
    public MethodDefinition? Method { get; set; }
}

public class VariableSymbol
{
    public string Identifier { get; set; }
    public string Type { get; set; }
    public bool IsOutOfScope { get; set; } = true;
    public VariableDefinition Definition { get; set; }
}

public class ParameterSymbol : VariableSymbol
{
    public ParameterDefinition ParameterDefinition { get; set; }
    public int ParameterNumber { get; set; }
}


public class GaleGenerator
{
    private ModuleParameters _moduleParameters;
    private TypeDefinition _mainModule;
    private AssemblyDefinition _assembly;
    //PACKAGE FUNCTIONS TABLE
    private Dictionary<string, FunctionSymbol> _functionSymbols = new();

    public GaleGenerator(ModuleParameters moduleParameters, string filename)
    {
        _moduleParameters = moduleParameters;
        _assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Gale", Version.Parse("1.0.0.0")),
            Path.GetFileName(filename), _moduleParameters);
    }
    
    public AssemblyDefinition GenerateProgram(SourceFileAst root)
    {
        _mainModule = new TypeDefinition(root.ModuleName, "Program", 
            TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed, 
            _assembly.MainModule.TypeSystem.Object);
        _assembly.MainModule.Types.Add(_mainModule);

        foreach (var func in root.Functions)
        {
            var funcDefinition = new MethodDefinition(func.Name, 
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, 
                func.ReturnType.GetTypeReference(_assembly));
            _mainModule.Methods.Add(funcDefinition);
            var funcSymbol = new FunctionSymbol()
            {
                Identifier = func.Name,
                Method = funcDefinition,
                ReturnedType = func.ReturnType
            };
            _functionSymbols[func.Name] = funcSymbol;
        }

        foreach (var func in root.Functions)
        {
            GenerateMethodBodyFromFunction(_functionSymbols[func.Name].Method, func);
        }
        if (root.Main != null)
        {
            var mainDefinition = new MethodDefinition("Main", 
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, 
                _assembly.MainModule.TypeSystem.Void);
            _mainModule.Methods.Add(mainDefinition);
            GenerateMethodBodyFromFunction(mainDefinition, root.Main);
            _assembly.EntryPoint = mainDefinition;
            //ADD DEFAULT CONSTRUCTOR FOR PROGRAM CLASS
            var ctor_Program_10 = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, _assembly.MainModule.TypeSystem.Void);
            _mainModule.Methods.Add(ctor_Program_10);
            var il_ctor_Program_11 = ctor_Program_10.Body.GetILProcessor();
            il_ctor_Program_11.Emit(OpCodes.Ldarg_0);
            il_ctor_Program_11.Emit(OpCodes.Call, _assembly.MainModule
                .ImportReference(TypeHelpers.DefaultCtorFor(_mainModule.BaseType)));
            il_ctor_Program_11.Emit(OpCodes.Ret);
            _assembly.EntryPoint = mainDefinition;
        }
        return _assembly;
    }

    public void GenerateMethodBodyFromFunction(MethodDefinition definition, FunctionDeclarationAst funcAst)
    {
        definition.Body.InitLocals = true;
        var ilBody = definition.Body.GetILProcessor();
        var symbolTable = new Dictionary<string, VariableSymbol>();
        var count = 0;
        foreach (var parameterAst in funcAst.Parameters)
        {
            var parameter = new ParameterDefinition(parameterAst.Name, 
                ParameterAttributes.None, parameterAst.Type.GetTypeReference(_assembly));
            definition.Parameters.Add(parameter);
            var symbolVar = new ParameterSymbol()
            {
                ParameterDefinition = parameter,
                Identifier = parameterAst.Name,
                Type = parameterAst.Type,
                ParameterNumber = count
            };
            ++count;
            symbolTable[parameterAst.Name] = symbolVar;
            //parameterAst.Type
        }
        //Parameters of 'public static int Get19(int p19, int p192){...'
        GenerateBlock(funcAst.Block, ilBody, definition, symbolTable);

        ilBody.Emit(OpCodes.Ret);
        //return methodDefinition;
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
                case MassDeclarationStatementAst massDeclaration:
                {
                    GenerateMassDeclaration(massDeclaration, ilBody, method, symbols);
                    break;
                }
                case DeclarationStatementAst declaration:
                    GenerateDeclaration(declaration, ilBody, method, symbols);
                    break;
                case ExpressionStatementAst expression:
                {
                    GenerateExpression(expression.ExpressionAst, ilBody, method, symbols);
                    break;
                }
                case ReturnStatementAst returnExpression:
                {
                    if (!returnExpression.IsNoReturn) GenerateExpression(returnExpression.ReturnedExpression, ilBody, method, symbols);
                    ilBody.Emit(OpCodes.Ret);
                    break;
                }
                case IfStatementAst ifStatement:
                {
                    GenerateIfStatement(ifStatement, ilBody, method, symbols);
                    break;
                }
                default:
                    throw new Exception($"Unknown statement {statement.GetType()}");
                    break;
            }
            //GenerateSimpleStatement(ilBody, method, symbols);
        }
    }

    public void GenerateIfStatement(IfStatementAst ifStatement,
        ILProcessor ilBody, MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        if (ifStatement.BoolExpression != null)
        {
            GenerateExpression(ifStatement.BoolExpression, ilBody, method, symbols);
        }
        var elseEntryPoint = ilBody.Create(OpCodes.Nop);
        ilBody.Emit(OpCodes.Brfalse, elseEntryPoint);
        GenerateBlock(ifStatement.Block, ilBody, method, symbols);
        
        var elseEnd = ilBody.Create(OpCodes.Nop);
        ilBody.Emit(OpCodes.Br, elseEnd);
        ilBody.Append(elseEntryPoint);
        
        if (ifStatement.IfStatement != null)
        {
            GenerateIfStatement(ifStatement.IfStatement, ilBody, method, symbols);
        }
        else if (ifStatement.ElseStatement != null)
        {
            GenerateBlock(ifStatement.ElseStatement?.Block, ilBody, method, symbols);
        }
        else
        {
            
        }
        ilBody.Append(elseEnd);
        
    }
    public void GenerateMassAssigment(MassAssigmentStatementAst assigmentStmt, ILProcessor ilBody,
        MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        foreach (var assigment in assigmentStmt.Assigments)
        {
            GenerateAssigment(assigment, ilBody, method, symbols);
        }
    }
    
    public void GenerateAssigment(AssigmentStatementAst assigmentStmt, 
        ILProcessor ilBody, MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        GenerateExpression(assigmentStmt.Expression, ilBody, method, symbols);
        var varSymbol = symbols[assigmentStmt.Identifier.Name];//TODO: add check
        if (varSymbol is ParameterSymbol parameter)
        {
            ilBody.Emit(OpCodes.Starg_S, parameter.ParameterDefinition);
            return;
        }

        ilBody.Emit(OpCodes.Stloc,  varSymbol.Definition);
    }

    public void GenerateExpression(ExpressionAst expression, ILProcessor ilBody,
        MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        switch (expression)
        {
            case IntegerLiteral literal:
                ilBody.Emit(OpCodes.Ldc_I4, (int)literal.Value);
                break;
            case StringLiteral literal:
                ilBody.Emit(OpCodes.Ldstr, literal.Value);
                break;
            case BinaryExpressionAst binary:
                GenerateBinaryExpression(binary, ilBody, method, symbols);
                break;
            case FunctionCallAst call:
            {
                GenerateFunctionCall(call, ilBody, method, symbols);
                break;
            }
            case IdentifierAst identifier:
            {
                GenerateIdentifier(identifier, ilBody, method, symbols);
                break;
            } 
            //UNARY EXPRESSION
        }
    }

    public void GenerateIdentifier(IdentifierAst identifier,
        ILProcessor ilBody, MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        symbols.TryGetValue(identifier.Name, out var symbol);
        if (symbol == null) throw new Exception("Unknown Variable");
        if (symbol is ParameterSymbol parameter)
        {
            switch (parameter.ParameterNumber)
            {
                case 0:
                    ilBody.Emit(OpCodes.Ldarg_0);
                    break;
                case 1: 
                    ilBody.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    ilBody.Emit(OpCodes.Ldarg_2);
                    break;
                case 3: 
                    ilBody.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    ilBody.Emit(OpCodes.Ldarg, parameter.ParameterNumber);
                    break;
            }
            return;
        }
        ilBody.Emit(OpCodes.Ldloc, symbol.Definition);
    }
    
    public void GenerateFunctionCall(FunctionCallAst call, 
        ILProcessor ilBody, MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        if (call.Identifier.Name == "fmtPrintln")
        {
            GeneratePrintln(call, ilBody, method, symbols);
            return;
        }
        var func = _functionSymbols[call.Identifier.Name]?.Method ?? throw new Exception($"Function with name {call.Identifier.Name} not found");

        foreach (var parameter in call.Parameters)
        {
            GenerateExpression(parameter, ilBody, method, symbols);
        }
        ilBody.Emit(OpCodes.Call, func);
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
            case BinaryExpressionType.Greater:
                ilBody.Emit(OpCodes.Cgt);
                break;
            case BinaryExpressionType.Equals:
                ilBody.Emit(OpCodes.Ceq);
                break;
            case BinaryExpressionType.NotEquals:
                ilBody.Emit(OpCodes.Ceq);
                ilBody.Emit(OpCodes.Ldc_I4_0);
                ilBody.Emit(OpCodes.Ceq);
                break;
            case BinaryExpressionType.LessOrEquals:
                ilBody.Emit(OpCodes.Cgt);
                ilBody.Emit(OpCodes.Ldc_I4_0);
                ilBody.Emit(OpCodes.Ceq);
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

    public void GenerateArrayDeclaration(ArrayDeclarationStatementAst declaration, ILProcessor ilBody,
        MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        //var arr = new int[4, 5];
        var arr = new VariableDefinition(declaration.Type.GetTypeReference(_assembly).MakeArrayType());
        method.Body.Variables.Add(arr);
        ilBody.Emit(OpCodes.Ldc_I4, declaration.Dimensions.First());
        ilBody.Emit(OpCodes.Newarr, declaration.Type.GetTypeReference(_assembly));
        ilBody.Emit(OpCodes.Stloc, arr);
        symbols[declaration.Identifier.Name] = new VariableSymbol()
        {
            Definition = arr,
            Identifier = declaration.Identifier.Name,
            IsOutOfScope = false,
            Type = "[]" + declaration.Type//??
        };
    }

    public void GenerateDeclaration(DeclarationStatementAst declaration, ILProcessor ilBody,
        MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        var variable = new VariableDefinition(declaration.Type.GetTypeReference(_assembly)); //TODO: NEED TYPE
        method.Body.Variables.Add(variable);
        if (declaration.Expression is not null)
            GenerateExpression(declaration.Expression, ilBody, method, symbols);
        ilBody.Emit(OpCodes.Stloc, variable);
        symbols[declaration.Identifier.Name] = new VariableSymbol()
        {
            Definition = variable,
            Identifier = declaration.Identifier.Name,
            IsOutOfScope = false,
            Type = declaration.Type
        };
    }
    
    public void GeneratePrintln(FunctionCallAst call, 
        ILProcessor ilBody, MethodDefinition method, Dictionary<string, VariableSymbol> symbols)
    {
        var types = new string[] { };
        foreach (var parameter in call.Parameters)
        {
            GenerateExpression(parameter, ilBody, method, symbols);
            var type = GetExpressionType(parameter, symbols);
            if (type == "int")
            {
                types = [.. types, "System.Int32"];
            }
            else
            {
                types = [.. types, "System.String"];
            }
        }
        ilBody.Emit(OpCodes.Call, _assembly.MainModule.ImportReference(TypeHelpers
            .ResolveMethod(typeof(System.Console), "WriteLine",
                System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, 
                types)));
        //ilBody.Emit(OpCodes.Call, writeLineRef);
    }

    public string GetExpressionType(ExpressionAst expression, Dictionary<string, VariableSymbol> symbols)
    {
        switch (expression)
        {
            case IntegerLiteral:
                return "int";
            case StringLiteral:
                return "string";
            case BinaryExpressionAst binary:
                var leftType = GetExpressionType(binary.LeftOperand, symbols);
                var rightType = GetExpressionType(binary.RightOperand, symbols);
                if (leftType != rightType)
                    throw new Exception($"Binary expression exception: {leftType} != {rightType}");
                //GenerateBinaryExpression(binary, ilBody, method, symbols);
                return leftType;
                break;
            case FunctionCallAst call:
            {
                if (call.Identifier.Name == "fmtPrintln") return "void";
                _functionSymbols.TryGetValue(call.Identifier.Name, out var functionSymbol);
                if (functionSymbol == null) throw new Exception($"Function {call.Identifier.Name} not found");
                return functionSymbol.ReturnedType;
            }
            case IdentifierAst identifier:
            {
                symbols.TryGetValue(identifier.Name, out var variableSymbol);
                if (variableSymbol == null) throw new Exception($"Variable {identifier.Name} not found");
                //identifier.Type;
                return variableSymbol.Type;
                break;
            } case null:
                return "void";
            default:
                throw new Exception("Type undefined");
        }
    }
}