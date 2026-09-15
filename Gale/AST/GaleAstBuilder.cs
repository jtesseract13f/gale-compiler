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
        sourceAst.ModuleName = context.packageClause().packageName().identifier().IDENTIFIER().GetText();
        var declarations = context.declaration();
        foreach (var declaration in declarations)
        {
            var declarationAst = VisitDeclaration(declaration);
            //var type = declaration.typeDecl().typeSpec()[0];
        }
        foreach (var function in functions)
        {
            var func = (FunctionDeclarationAst)VisitFunctionDecl(function);
            if (func.Name  == "main")
            {
                sourceAst.Main = func;
                continue;
            }
            sourceAst.Functions.Add(func);
        }
        return sourceAst;
    }
    
    //public override VisitD

    public override AstNode VisitFunctionDecl(GoParser.FunctionDeclContext context)
    {
        var func = new FunctionDeclarationAst();
        func.Block = (BlockAst)VisitBlock(context.block());
        func.Name = context.IDENTIFIER().GetText();
        var parameters = context.signature()?.parameters().parameterDecl() ?? [];
        foreach (var parameter in parameters)
        {
            var identifiers = parameter.identifierList().IDENTIFIER();
            var type = parameter.type_();
            if (type != null)
            {
                var typeNameAst = VisitType_(type);
            }
            foreach (var id in identifiers)
            {
                //var parameterAst = new ParameterAst(){
                //    Name = id.GetText(), 
                //    Type = type.typeName().IDENTIFIER().GetText()};
                
                //func.Parameters.Add(parameterAst);
            }
        }
        var result = context?.signature()?.result()?.type_();
        if (result != null)
        {
            var resultAst = VisitType_(result);
            //var returnType =  context?.signature()?.result().type_().typeName().IDENTIFIER().GetText() ?? "void";
            //func.ReturnType = returnType;
            //var qualifiedIdent = context?.signature()?.result()?.type_()?.typeName()?.qualifiedIdent()?.IDENTIFIER() ?? [];
            //foreach (var terminal in qualifiedIdent) { }
        }
        return func;
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
        
        var returnStmt = context.returnStmt();
        if (returnStmt != null)
        {
            return VisitReturnStmt(returnStmt);
        }
        var ifStmt = context.ifStmt();
        if (ifStmt != null)
        {
            return VisitIfStmt(ifStmt);
        }
        var forStmt = context.forStmt();
        if (forStmt != null)
        {
            return VisitForStmt(forStmt);
        }
        var labeledStmt = context.labeledStmt();
        var fallthroughStmt = context.fallthroughStmt();
        var goStmt = context.goStmt();
        var gotoStmt = context.gotoStmt();
        
        var breakStmt = context.breakStmt();
        var continueStmt = context.continueStmt();
        var selectStmt = context.selectStmt();
        var switchStmt = context.switchStmt();
        var deferStmt = context.deferStmt();
        
        return base.VisitStatement(context);
    }

    public override AstNode VisitForStmt(GoParser.ForStmtContext context)
    {
        var forAst = new WhileStatementAst();
        context.rangeClause();
        context.forClause();
        var condition = context.condition();
        forAst.Block = (BlockAst)VisitBlock(context.block());
        if (condition != null)
        {
            forAst.BoolExpression = (ExpressionAst)VisitExpression(condition.expression());
        }

        return forAst;
    }

    public override AstNode VisitIfStmt(GoParser.IfStmtContext context)
    {
        var ifStmtAst = new IfStatementAst();
        var simple = context.simpleStmt();
        var ifstmt = context.ifStmt();
        var blockIf = context.block();
        if (ifstmt != null)
        {
            ifStmtAst.IfStatement = (IfStatementAst)VisitIfStmt(ifstmt);
        }
        
        if (blockIf != null)
        {
            ifStmtAst.Block = (BlockAst)VisitBlock(blockIf[0]);
            if (blockIf.Length == 2)
            {
                ifStmtAst.ElseStatement = new IfStatementAst()
                {
                    Block = (BlockAst)VisitBlock(blockIf[1]),
                };
            }
        }
        var expression = context.expression();
        if (expression != null)
        {
            ifStmtAst.BoolExpression = (ExpressionAst)VisitExpression(expression);
        }
        return ifStmtAst;
    }

    public override AstNode VisitReturnStmt(GoParser.ReturnStmtContext context)
    {
        var expressions = context?.expressionList()?.expression() ??[];
        if (expressions.Length == 0) return new ReturnStatementAst() { IsNoReturn = true };

        return new ReturnStatementAst() { IsNoReturn = false, 
            ReturnedExpression = (ExpressionAst)VisitExpression(expressions[0]) };
        return base.VisitReturnStmt(context);
    }

    public override AstNode VisitDeclaration(GoParser.DeclarationContext context)
    {
        var varDecl = context.varDecl();
        if (varDecl != null) return VisitVarDecl(varDecl);
        
        var constDecl = context.constDecl();
        var typeDecl = context.typeDecl();
        if (typeDecl != null)
        {
            return VisitTypeDecl(typeDecl);
        }
        
        return base.VisitDeclaration(context);
    }

    public override AstNode VisitTypeDecl(GoParser.TypeDeclContext context)
    {
        var typeSpecs = context.typeSpec();
        foreach (var typeSpec in typeSpecs)
        {
            //return VisitTypeSpec(typeSpec);
        }
        //return VisitTypeSpec();
        //context.
        return null;
    }

    public override AstNode VisitTypeSpec(GoParser.TypeSpecContext context)
    {
        var typeDef = context.typeDef();
        if (typeDef != null)
        {
            var type = typeDef.type_();
            if (type != null)
            {
                VisitType_(type);
            }
            var identifier = typeDef.IDENTIFIER();
            var typeParameters = typeDef.typeParameters();
        }
        context.aliasDecl();
        //context.
        return base.VisitTypeSpec(context);
    }

    public override AstNode VisitVarDecl(GoParser.VarDeclContext context)
    {
        var varSpecs = context.varSpec();
        var massDeclaration = new MassDeclarationStatementAst();
        foreach (var varSpec in varSpecs)
        {
            var identifierList = varSpec.identifierList().IDENTIFIER();
            var expressionList = varSpec.expressionList()?.expression();
            var type = varSpec.type_();
            VisitType_(type);
            var name = type?.typeName();
            var lit = type?.typeLit();
            var args = type?.typeArgs();
            for (int i = 0; i < identifierList.Length; ++i)
            {
                var arrayDecl = type?.typeLit()?.arrayType();
                if (arrayDecl != null)
                {
                    var arrDeclaration = new ArrayDeclarationStatementAst();
                    arrDeclaration.Identifier = new IdentifierAst() { Name = identifierList[i].GetText() };
                    arrDeclaration.Type = type.typeLit().arrayType().GetText();
                    var typeLit = type.typeLit().arrayType().elementType();
                    var elementType = typeLit?.type_()?.typeName().IDENTIFIER().GetText(); //TODO: add recursive call
                    var count = ((IntegerLiteral)VisitExpression(type.typeLit().arrayType().arrayLength().expression())).Value;
                    arrDeclaration.Type = elementType ?? "int";
                    arrDeclaration.Dimensions.Add((int)count);
                    massDeclaration.Declarations.Add(arrDeclaration);
                    continue;
                }
                
                var declaration = new DeclarationStatementAst();
                declaration.Identifier = new IdentifierAst() { Name = identifierList[i].GetText() };
                
                declaration.Type = type.typeName()?.IDENTIFIER().GetText() ?? "";
                if (expressionList?.Length > i)
                {
                    declaration.Expression = (ExpressionAst)VisitExpression(expressionList[i]);
                }
                massDeclaration.Declarations.Add(declaration);
            }
        }
        return massDeclaration;
    }

    public override AstNode VisitType_(GoParser.Type_Context context)
    {
        var type = context.type_();
        var typeArgs = context.typeArgs();
        var typeLit = context.typeLit();
        if (typeLit != null)
        {
            var typeLitAst = VisitTypeLit(typeLit);
            return typeLitAst;
        }
        var typeName = context.typeName();
        if (typeName != null)
        {
            return VisitTypeName(typeName);
        }
        
        return base.VisitType_(context);
    }

    public override AstNode VisitTypeLit(GoParser.TypeLitContext context)
    {
        var structType = context.structType();
        if (structType != null)
        {
            return VisitStructType(structType);
        }
        var arrayType = context.arrayType();
        if (arrayType != null)
        {
            return VisitArrayType(arrayType);
        }
        var pointerType = context.pointerType();
        if (pointerType != null)
        {
            return VisitPointerType(pointerType);
        }
        var sliceType = context.sliceType(); //TODO: Add functions for literals
        var channelType = context.channelType();
        var functionType = context.functionType();
        var interfaceType = context.interfaceType();
        var mapType = context.mapType();
        return base.VisitTypeLit(context);
    }

    public override AstNode VisitPointerType(GoParser.PointerTypeContext context)
    {
        return new PointerTypeAst()
        {
            PointedType = (TypeAst)VisitType_(context.type_())
        };
    }

    public override AstNode VisitArrayType(GoParser.ArrayTypeContext context)
    {
        var arrayTypeAst = new ArrayTypeAst();
        var elementType = context.elementType();
        if (elementType != null)
        {
            var elementTypeAst = (TypeAst)VisitType_(elementType.type_());
            arrayTypeAst.ElementType = elementTypeAst;
        }
        var arrayLength = context.arrayLength();
        if (arrayLength != null)
        {
            arrayTypeAst.Length = (ExpressionAst)VisitExpression(arrayLength.expression());
        }
        return arrayTypeAst;
    }

    public override AstNode VisitStructType(GoParser.StructTypeContext context)
    {
        var structType = new StructTypeAst();
        var fieldDecl = context.fieldDecl();
        foreach (var field in fieldDecl)
        {
            var fieldAst = (FieldDeclarationAst)VisitFieldDecl(field);
            structType.Fields.Add(fieldAst);
        }
        return structType;
    }

    public override AstNode VisitFieldDecl(GoParser.FieldDeclContext context)
    {
        var embeddedField = context.embeddedField();
        if (embeddedField != null)
        {
            var embeddedFieldAst = new EmbeddedFieldDeclarationAst();
            var typeArgs = embeddedField.typeArgs();
            if (typeArgs != null)
            {
                //var typeArgAst = VisitTypeArgs(typeArgs);
            }
            var typeName = embeddedField.typeName();
            if (typeName != null)
            {
                var typeNameAst = VisitTypeName(typeName);
                embeddedFieldAst.TypeName = (IdentifierTypeAst)typeNameAst;
            }
            return embeddedFieldAst;
        }
        //var stringContext = context.string_();
        var type = context.type_();
        if (type != null)
        {
            var namedFieldAst = new NamedFieldDeclaratonAst();
            var typeAst = (TypeAst)VisitType_(type);
            namedFieldAst.FieldType = typeAst;
            namedFieldAst.Identifier = context.identifierList().IDENTIFIER()[0].GetText();
            return namedFieldAst;
        }
        //var tag = context.tag;
        return base.VisitFieldDecl(context);
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
        var incrementOp = context.incDecStmt();
        return base.VisitSimpleStmt(context);
    }

    //REFACTORING
    public override AstNode VisitExpression(GoParser.ExpressionContext context)
    {
        var primaryExpr = context.primaryExpr();
        if (primaryExpr != null)
        {
            var primary = VisitPrimaryExpr(primaryExpr);
            return primary;
        }
        
        var expressions = context.expression();
        if (expressions.Length == 2)
        {
            var binaryExpression = new BinaryExpressionAst();
            binaryExpression.Operation = GetBinaryExpressionType(context);
            var left = VisitExpression(expressions[0]);
            var right = VisitExpression(expressions[1]);
            binaryExpression.LeftOperand = (ExpressionAst)left;
            binaryExpression.RightOperand = (ExpressionAst)right;
            
            return binaryExpression;
        }
        else if (expressions.Length == 1)
        {
            var unary = VisitExpression(expressions[0]);
        }

        throw new Exception($"Undefined expression {context.GetText()}");
    }

    public override AstNode VisitPrimaryExpr(GoParser.PrimaryExprContext context)
    {
        var operand = context.operand();
        if (operand != null)
        {
            var identifier = context?.IDENTIFIER();
            var typeArgs = operand.typeArgs();
            var literal = operand.literal();
            var expression = operand.expression();
            var operandAst = (OperandAst)VisitOperand(operand);
        }
        //ONE EXPRESSION
        for (int i = 0; i < context.ChildCount; ++i)
        {
            var child = context.GetChild(i);
                //var type = child.GetType();
        }
        var conversion = context.conversion();
        
        var methodExpr = context.methodExpr();
        if (methodExpr is not null)
        {
            return base.VisitPrimaryExpr(context);
        }
        var slice = context.slice_() ?? [];
        if (slice.Length > 0)
        {
            return base.VisitPrimaryExpr(context);
        }
        var index = context.index() ?? [];
        if (index.Length > 0)
        {
            //return base.VisitPrimaryExpr(context);
        }
        var arguments = context.arguments() ?? [];
        if (arguments.Length > 0)
        {
            var identifiers = context.IDENTIFIER() ?? [];
            
            if (operand != null && identifiers.Length > 0)
            {
                //var 
            }
        }
        
        return base.VisitPrimaryExpr(context);
    }

    public override AstNode VisitLiteralType(GoParser.LiteralTypeContext context)
    {
        var sliceType = context.sliceType();
                
        var structType = context.structType();
        if (structType != null)
        {
            var structAst = VisitStructType(structType);
            return structAst;
        }
        var arrayType = context.arrayType();
        if (arrayType != null)
        {
            var arrayAst = VisitArrayType(arrayType);
            return arrayAst;
        }
        var typeArgs = context.typeArgs();
        if (typeArgs != null)
        {
            var typeArgsAst = VisitTypeArgs(typeArgs);
        }
        var typeName = context.typeName();
        if (typeName != null)
        {
            var typeNameAst = VisitTypeName(typeName);
            return typeNameAst;
        }
        return base.VisitLiteralType(context);
    }

    public override AstNode VisitTypeName(GoParser.TypeNameContext context)
    {
        var identifier = context.IDENTIFIER();
        var qualifiedIdent = context?.qualifiedIdent()?.IDENTIFIER() ?? [];
        return new IdentifierTypeAst()
        {
            Name = identifier.GetText(),
            QualifiedIdentifiers = qualifiedIdent.Select(x => x.GetText()).ToList()
        };
    }

    public override AstNode VisitLiteral(GoParser.LiteralContext context)
    {
        var basicLit = context.basicLit();
        if (basicLit != null)
        {
            return VisitBasicLit(basicLit);
        }
        var compositeLit = context.compositeLit();
        if (compositeLit != null)
        {
            var compositeLiteral = new CompositeLiteral();
            var literalType = compositeLit.literalType();
            if (literalType != null)
            {
                var literalTypeAst = VisitLiteralType(literalType);
                compositeLiteral.Type = (TypeAst)literalTypeAst;
            }
            var literalValue = compositeLit.literalValue();
            if (literalValue != null)
            {
                var elementList = literalValue.elementList();
                var keyElements = elementList.keyedElement();
                foreach (var keyElement in keyElements)
                {
                    var elementAst = new Element();
                    var key = keyElement.key();
                    if (key != null)
                    {
                        var keyExpression = key.expression();
                        //var keyLiteralValue = key.literalValue();
                        elementAst.KeyExpression = (ExpressionAst)VisitExpression(keyExpression);
                    }
                    var element = keyElement.element();
                    if (element != null)
                    {
                        var expression = element.expression();
                        //var elementLiteralValue = element.literalValue();
                        elementAst.KeyExpression = (ExpressionAst)VisitExpression(expression);
                    }
                    compositeLiteral.Elements.Add(elementAst);
                }
            }

            return compositeLiteral;
        }
        
        var functionLit = context.functionLit();
        
        return base.VisitLiteral(context);
    }

    public override AstNode VisitLiteralValue(GoParser.LiteralValueContext context)
    {
        
        return base.VisitLiteralValue(context);
    }

    //only one operand
    public override AstNode VisitOperand(GoParser.OperandContext context)
    {
        var operandName = context.operandName();
        if (operandName != null)
        {
            var identifierAst = VisitOperandName(operandName);
            return identifierAst;
        }
        var literal = context.literal();
        if (literal != null)
        {
            var literalAst = VisitLiteral(literal);
            return literalAst;
        }
        var typeArgs = context.typeArgs();//??
        var expression = context.expression();
        if (expression != null) return VisitExpression(expression);
        //context.
        return null;
    }

    public override AstNode VisitOperandName(GoParser.OperandNameContext context)
    {
        var qualifiedIdent = context?.qualifiedIdent()?.IDENTIFIER() ?? [];
        var identifier = context?.IDENTIFIER()?.GetText() ?? "";
        return new IdentifierAst()
        {
            QualifiedIdentifiers = qualifiedIdent.Select(x => x.GetText()).ToList(),
            Name = identifier
        };
    }

    public override AstNode VisitBasicLit(GoParser.BasicLitContext context)
    {
        var integerLit = context.integer();
        if (integerLit != null)
        {
            return new IntegerLiteral() { Value = IntegerParseHelper.ParseInteger(integerLit) }; // NEED CONVERTOR
        }
        var stringContext = context.string_();
        if (stringContext != null)
        {
            return new StringLiteral() { Value = stringContext?.INTERPRETED_STRING_LIT().GetText() ?? "" };
        }
        var nilLit = context.NIL_LIT();
        var floatLit = context.FLOAT_LIT();
        return base.VisitBasicLit(context);
    }

    private BinaryExpressionType GetBinaryExpressionType(GoParser.ExpressionContext context)
    {
        if (context.MINUS() != null)
            return BinaryExpressionType.Minus;
        else if (context.PLUS() != null)
            return BinaryExpressionType.Plus;
        else if (context.STAR() != null)
            return BinaryExpressionType.Mul;
        else if (context.DIV() != null)
            return BinaryExpressionType.Div;
        else if (context.MOD() != null)
            return BinaryExpressionType.Mod;

        else if (context.AMPERSAND() != null)
            return BinaryExpressionType.BitwiseAnd;
        else if (context.OR() != null)
            return BinaryExpressionType.BitwiseOr;

        else if (context.LSHIFT() != null)
            return BinaryExpressionType.LShift;
        else if (context.RSHIFT() != null)
            return BinaryExpressionType.RShift;

        else if (context.NOT_EQUALS() != null)
            return BinaryExpressionType.NotEquals;
        else if (context.EQUALS() != null)
            return BinaryExpressionType.Equals;
        else if (context.LESS_OR_EQUALS() != null)
            return BinaryExpressionType.LessOrEquals;
        else if (context.GREATER_OR_EQUALS() != null)
            return BinaryExpressionType.GreaterOrEquals;
        else if (context.LESS() != null)
            return BinaryExpressionType.Less;
        else if (context.GREATER() != null)
            return BinaryExpressionType.Greater;

        else if (context.LOGICAL_AND() != null)
            return BinaryExpressionType.LogicalAnd;
        else if (context.LOGICAL_OR() != null)
            return BinaryExpressionType.LogicalOr;

        else if (context.RECEIVE() != null)
            return BinaryExpressionType.Receive;
        else
            throw new Exception($"Unknown binary operator at {context.Start}");
    }
}