using Antlr4.Runtime.Tree;
using Gale.Ast;
using Mono.CompilerServices.SymbolWriter;

namespace Gale.Compiler;

public class GaleVisitor : GoParserBaseVisitor<AstNode>
{
   public override AstNode VisitSourceFile(GoParser.SourceFileContext context)
   {
      var decls = context.functionDecl()[0];
      var block = decls.block();
      VisitBlock(block);
      return base.VisitSourceFile(context);
   }

   public override AstNode VisitBlock(GoParser.BlockContext context)
   {
      var statements = context.statementList();
      //var semi = statements.SEMI();
      var t = statements.statement();
      VisitStatementList(statements);
      return base.VisitBlock(context);
   }

   public override AstNode VisitStatementList(GoParser.StatementListContext context)
   {
      foreach (var stmt in context.statement())
      {
         VisitStatement(stmt);
      }
      return base.VisitStatementList(context);
   }

   public override AstNode VisitStatement(GoParser.StatementContext context)
   {
      var simpleStmt = context.simpleStmt();
      var declaration = context.declaration();
      var ifStmt = context.ifStmt();
      var returnStmt = context.returnStmt();
      
      var labeledStmt = context.labeledStmt();
      var gotoStmt = context.gotoStmt();
      var goStmt = context.goStmt();
      
      var forStmt = context.forStmt();
      var continueStmt = context.continueStmt();
      var breakStmt = context.breakStmt();
      
      var switchStmt = context.switchStmt();
      var fallthroughStmt = context.fallthroughStmt();
      
      var selectStmt = context.selectStmt();
      //mental illnesses
      var deferStmt = context.deferStmt();

      if (simpleStmt is not null)
      {
         return VisitSimpleStmt(simpleStmt);
      }
      if (declaration is not null)
      {
         return VisitDeclaration(declaration);
      }
      
      return base.VisitStatement(context);
   }

   public override AstNode VisitSimpleStmt(GoParser.SimpleStmtContext context)
   {
      var shortVarDecl = context.shortVarDecl();
      //var sendStmt = context.sendStmt();

      if (shortVarDecl is not null)
      {
         return VisitShortVarDecl(shortVarDecl);
      }
      return base.VisitSimpleStmt(context);
   }

   public override AstNode VisitShortVarDecl(GoParser.ShortVarDeclContext context)
   {
      //var shortval = context.DECLARE_ASSIGN();
      var d = context.identifierList();
      var g = context.expressionList();
      
      
      return base.VisitShortVarDecl(context);
   }

   public override AstNode VisitExpression(GoParser.ExpressionContext context)
   {
      context.expression();
      return base.VisitExpression(context);
   }

   public override AstNode VisitDeclaration(GoParser.DeclarationContext context)
   {
      var constDecl = context.constDecl();
      var varDecl = context.varDecl();
      var typeDecl = context.typeDecl();

      if (varDecl is not null)
      {
         return VisitVarDecl(varDecl);
      }
      return base.VisitDeclaration(context);
   }

   public override AstNode VisitVarDecl(GoParser.VarDeclContext context)
   {
      var v = context.varSpec();
      
      return base.VisitVarDecl(context);
   }

   public override AstNode VisitFunctionDecl(GoParser.FunctionDeclContext context)
   {
      //context.typeParameters();
      return base.VisitFunctionDecl(context);
   }
}