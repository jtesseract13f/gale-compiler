using Antlr4.Runtime.Tree;
using Gale.Ast;
using Mono.CompilerServices.SymbolWriter;

namespace Gale.Compiler;

public class GaleVisitor : GoParserBaseVisitor<AstNode>
{
    private List<T> VisitList<T>(IEnumerable<IParseTree> contexts) where T : AstNode
    {
        return contexts
            .Select(c => Visit(c) as T)
            .Where(node => node != null) 
            .ToList();
    }

}