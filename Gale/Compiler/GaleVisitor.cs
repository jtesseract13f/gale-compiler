using Antlr4.Runtime.Tree;
using Gale.Ast;
using Mono.CompilerServices.SymbolWriter;

namespace Gale.Compiler;

public class GaleVisitor : GoParserBaseVisitor<AstNode>
{
    public override AstNode VisitSourceFile(GoParser.SourceFileContext context)
    {
        var packageName = context.packageClause().packageName().identifier().IDENTIFIER().GetText();
        var imports = VisitImportDecls(context.importDecl());

        var methodDecls = context.methodDecl()
            .Select(x => (TopLevelDecl)VisitMethodDecl(x)).ToList();
        context.functionDecl();
        var funcDecl = context.functionDecl().Select(decl => (TopLevelFunctionDecl)VisitFunctionDecl(decl)).ToList();
        var declarations = new List<TopLevelDecl>();
        declarations = declarations.Union(funcDecl).Union(methodDecls).ToList();
        var sourceFile = new SourceFile(packageName, imports, declarations);
        return sourceFile;
    }

    private List<ImportSpec> VisitImportDecls(GoParser.ImportDeclContext[] importDecls)
    {
        var imports = new List<ImportSpec>(){};
        foreach (var decl in importDecls)
        {
            var importSpecs = decl.importSpec();
            foreach (var spec in importSpecs)
            {
                var imp = spec.importPath().GetText();
                imports.Add(new ImportSpec(imp.Trim('"').Split('/').ToList(), imp));
            }
        }
        return imports;
    }

    public override AstNode VisitMethodDecl(GoParser.MethodDeclContext context)
    {
        var receiver = context.receiver();
        var func = context.FUNC().GetText();
        //var decl = new Declaration
        var topLevelDecl = new TopLevelMethodDecl();
        
        return topLevelDecl;
    }

    public override AstNode VisitFunctionDecl(GoParser.FunctionDeclContext context)
    {
        var parameters = context.signature().parameters().parameterDecl()
            .Select(x => (Parameter)VisitParameterDecl(x)).ToList();
        var functionResult = (FunctionResult)VisitResult(context.signature().result());
        var signature = new FunctionSignature(parameters, functionResult);

        var statements = context.block().statementList();
        //var body = new BlockData();
        
        var funcDecl = new FunctionDeclData(context.IDENTIFIER().GetText(), signature, null);
        var topLevelDecl = new TopLevelFunctionDecl(funcDecl);
        return topLevelDecl;
        //return base.VisitFunctionDecl(context);
    }

    public override AstNode VisitStatementList(GoParser.StatementListContext context)
    {
        var statements = context.statement().Select(x => (Statement)VisitStatement(x));
        return base.VisitStatementList(context);
    }

    public override AstNode VisitStatement(GoParser.StatementContext context)
    {
        context.block();
        return base.VisitStatement(context);
    }

    //TODO: А тут как типы определять что за шиза КАК ИХ ИДЕНТИФИЦИРОВАТЬ И ПОНИМАТЬ ЧТО ТАМ ПРОИСХОДИТ
    public override AstNode VisitResult(GoParser.ResultContext context)
    {
        if (context == null) return null;
        var parameters = context.type_().typeName();
        var result = new ResultType(new NamedType(new IdentifiedType("hui")));
        return result;
            //return base.VisitResult(context);
    }
    //TODO: а че делать куда какие параметры как их определять какие типы как их определять
    public override AstNode VisitParameterDecl(GoParser.ParameterDeclContext context)
    {
        var identifiers = context.identifierList().IDENTIFIER().Select(x => x.GetText()).ToList();
        var name = context.type_().typeName();
        var lit = context.type_().typeLit();
        var parameter = new Parameter(identifiers, new NamedType(new IdentifiedType("honk")));
        return parameter;
    }
}