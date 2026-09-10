using Antlr4.Runtime.Tree;
using Gale.Helpers;

namespace Gale.AST;

public abstract class AstNode;

public class GaleAstBuilder : GoParserBaseVisitor<AstNode>
{
    public override AstNode VisitSourceFile(GoParser.SourceFileContext context)
    {
        var sourceAst = new SourceFileAst();
        var functions = context.functionDecl();
        //var mainFunction = new FunctionDeclarationAst();
        sourceAst.ModuleName = context.packageClause().packageName().identifier().IDENTIFIER().GetText();
        foreach (var function in functions)
        {
            var func = new FunctionDeclarationAst();
            var block = function.block();
            var blockAst = (BlockAst)VisitBlock(block);
            func.Block = blockAst;
            func.Name = function.IDENTIFIER().GetText();
            //function.signature();
            var parameters = function.signature()?.parameters().parameterDecl();
            foreach (var parameter in parameters)
            {
                var identifiers = parameter.identifierList().IDENTIFIER();
                var type = parameter.type_();
                foreach (var id in identifiers)
                {
                    var parameterAst = new ParameterAst(){
                        Name = id.GetText(), 
                        Type = type.typeName().IDENTIFIER().GetText()};
                    func.Parameters.Add(parameterAst);
                }
            }
            var result = function?.signature()?.result()?.type_()?.typeName();
            if (result != null)
            {
                var returnType =  function.signature()?.result().type_().typeName().IDENTIFIER().GetText() ?? "void";
                func.ReturnType = returnType;
                var qualifiedIdent = function.signature()?.result()?.type_()?.typeName()?.qualifiedIdent()?.IDENTIFIER() ?? [];
                foreach (var terminal in qualifiedIdent) { }
            }
            
            if (func.Name  == "main")
            {
                sourceAst.Main = func;
            }
            sourceAst.Functions.Add(func);
        }
        return sourceAst;
    }

    public override AstNode VisitBlock(GoParser.BlockContext context)
    {
        var blockAst = new BlockAst();
        var statements = context.statementList().statement();
        foreach (var statement in statements)
        {
            var statementAst = (StatementAst)VisitStatement(statement);
            blockAst.Statements.Add(statementAst);
        }
        return blockAst;
    }

    public override AstNode VisitStatement(GoParser.StatementContext context)
    {
        var block = context.block();
        if (block != null) return VisitBlock(block);
        
        var declaration = context.declaration();
        if (declaration != null) return VisitDeclaration(declaration);
        
        var simpleStmt = context.simpleStmt();
        if (simpleStmt != null) return VisitSimpleStmt(simpleStmt);
        
        var ifStmt = context.ifStmt();
        var forStmt = context.forStmt();
        var labeledStmt = context.labeledStmt();
        var fallthroughStmt = context.fallthroughStmt();
        var goStmt = context.goStmt();
        var gotoStmt = context.gotoStmt();
        
        var returnStmt = context.returnStmt();
        var breakStmt = context.breakStmt();
        var continueStmt = context.continueStmt();
        var selectStmt = context.selectStmt();
        var switchStmt = context.switchStmt();
        var deferStmt = context.deferStmt();
        
        return base.VisitStatement(context);
    }

    public override AstNode VisitDeclaration(GoParser.DeclarationContext context)
    {
        var varDecl = context.varDecl();
        if (varDecl != null) return VisitVarDecl(varDecl);
        
        var constDecl = context.constDecl();
        var typeDecl = context.typeDecl();
        
        return base.VisitDeclaration(context);
    }

    public override AstNode VisitVarDecl(GoParser.VarDeclContext context)
    {
        var varSpecs = context.varSpec();
        var massDeclaration = new MassDeclarationStatementAst();
        foreach (var varSpec in varSpecs)
        {
            var identifierList = varSpec.identifierList().IDENTIFIER();
            var expressionList = varSpec.expressionList().expression();
            var type = varSpec.type_();
            //var name = type.typeName();
            //var lit = type.typeLit();
            //var args = type.typeArgs();
            for (int i = 0; i < identifierList.Length; ++i)
            {
                var declaration = new DeclarationStatementAst();
                declaration.Identifier = new IdentifierAst() { Name = identifierList[i].GetText() };
                declaration.Type = type.typeName()?.IDENTIFIER().GetText() ?? "";
                if (expressionList.Length > i)
                {
                    declaration.Expression = (ExpressionAst)VisitExpression(expressionList[i]);
                }
                massDeclaration.Declarations.Add(declaration);
            }
        }
        return massDeclaration;
    }

    public override AstNode VisitSimpleStmt(GoParser.SimpleStmtContext context)
    {
        var assignment = context.assignment();
        if (assignment != null)
        {
            var expressions = assignment.expressionList();
            var assigmentsAst = new MassAssigmentStatementAst();
            var right = expressions[0].expression();
            var left = expressions[1].expression();

            for (int i = 0; i < right.Length; ++i)
            {
                var assignmentAst = new AssigmentStatementAst();
                assignmentAst.Identifier = (IdentifierAst)VisitExpression(right[i]);
                assignmentAst.Expression = (ExpressionAst)VisitExpression(left[i]);
                assigmentsAst.Assigments.Add(assignmentAst);
            }

            return assigmentsAst;
        }
        var expressionStmt = context.expressionStmt();
        if (expressionStmt != null)
        {
            var expression = VisitExpression(expressionStmt.expression());
            return new ExpressionStatementAst(){ExpressionAst = (ExpressionAst)expression};
        }
        var sendStmt = context.sendStmt();
        var shortVarDecl = context.shortVarDecl();
        
        return base.VisitSimpleStmt(context);
    }

    public override AstNode VisitExpression(GoParser.ExpressionContext context)
    {
        var primaryExpr = context.primaryExpr();
        if (primaryExpr != null)
        {
            var primary = VisitPrimaryExpr(primaryExpr);
            return primary;
        }

        var binaryExpression = new BinaryExpressionAst();
        if (context.MINUS() != null)
        {
            binaryExpression.Operation = BinaryExpressionType.Minus;
        } else if(context.PLUS() != null)
        {
            binaryExpression.Operation = BinaryExpressionType.Plus;
        }
        
        context.DIV();
        context.AMPERSAND();
        context.EQUALS();
        context.EXCLAMATION();
        context.STAR();
        
        var expressions = context.expression();
        if (expressions.Length == 2)
        {
            var left = VisitExpression(expressions[0]);
            var right = VisitExpression(expressions[1]);
            binaryExpression.LeftOperand = (ExpressionAst)VisitExpression(expressions[0]);
            binaryExpression.RightOperand = (ExpressionAst)VisitExpression(expressions[1]);
        }
        return binaryExpression;
    }

    public override AstNode VisitPrimaryExpr(GoParser.PrimaryExprContext context)
    {
        var operand = context.operand();
        ExpressionAst operandAst = null;
        var arguments = context.arguments();
        context.methodExpr();
        context.conversion();
        context.typeAssertion();
        context.index();
        context.slice_();
        
        if (operand != null)
        {
            operandAst = (ExpressionAst)VisitOperand(operand);
            if (arguments.Length == 0) return operandAst;
        }
        
        var funcAst = new FunctionCallAst();
        if (arguments.Length > 0)
        {
            if (arguments[0].expressionList() is not null)
            {
                var expressions = arguments[0].expressionList().expression();
                foreach (var expression in expressions)
                {
                    funcAst.Parameters.Add((ExpressionAst)VisitExpression(expression));
                }
            }
            funcAst.Identifier = (IdentifierAst)operandAst;
            return funcAst;
        }
        
        return base.VisitPrimaryExpr(context);
    }

    public override AstNode VisitOperand(GoParser.OperandContext context)
    {
        var operandName = context.operandName();
        if (operandName != null)
        {
            var namedOperand = new IdentifierAst();
            var identifier = operandName.IDENTIFIER();
            if (identifier != null)
            {
                namedOperand.Name = identifier.GetText();
            }
            var qualifiedIdentifier = operandName.qualifiedIdent();
            if (qualifiedIdentifier != null)
            {
                namedOperand.QualifiedIdentifiers = qualifiedIdentifier.IDENTIFIER()
                    .Select(x => x.GetText()).ToList();
            }
            return namedOperand;
        }
        var literal = context.literal();
        if (literal != null)
        {
            var basicLit = literal.basicLit();
            if (basicLit != null)
            {
                var lit = VisitBasicLit(basicLit);
                return lit;
            }
            var functionLit = literal.functionLit();
            var compositeLit = literal.compositeLit();
        }
        context.typeArgs();
        var expression = context.expression();
        if (expression != null) return VisitExpression(expression);
        //context.
        return base.VisitOperand(context);
    }

    public override AstNode VisitBasicLit(GoParser.BasicLitContext context)
    {
        var integerLit = context.integer();
        if (integerLit != null)
        {
            return new IntegerLiteral() { Value = IntegerParseHelper.ParseInteger(integerLit) }; // NEED CONVERTOR
        }
        var stringContext = context.string_();
        var nilLit = context.NIL_LIT();
        var floatLit = context.FLOAT_LIT();
        return base.VisitBasicLit(context);
    }
}