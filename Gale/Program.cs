using System.Diagnostics;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Gale.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Gale;

class Program
{
    static async Task Main(string[] args)
    {
        
        //String input = "your text to parse here";
        var file = "/home/jtesseract13f/Projects/Gale/Gale/TestGoFiles/hello.go";
        var stream = CharStreams.fromPath(file);
        var lexer = new GoLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new GoParser(tokens);
        IParseTree tree = parser.sourceFile();

        Console.WriteLine(tree.GetText());
        PrintIParseTree(tree);

        var gale = new GaleVisitor();
        gale.VisitSourceFile((GoParser.SourceFileContext)tree);
    }

    public static void PrintIParseTree(IParseTree root, int tabs = 0)
    {
        //Console.WriteLine("\nROOT: " + root.GetType().FullName);
        for (int i = 0; i < root.ChildCount; ++i)
        {
            var child = root.GetChild(i);
            Console.WriteLine( new string(' ', tabs) + child.GetType().FullName + "\t" + child.ToString() + "\t" + child.Payload);
            PrintIParseTree(child, tabs + 1);
        }
        
    }
}