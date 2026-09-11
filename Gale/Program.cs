using System.Diagnostics;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Gale.AST;
using Gale.CodeGeneration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Mono.Cecil;

namespace Gale;


class Program
{
    static async Task Main(string[] args)
    {
        //String input = "your text to parse here";
        var file = "/home/jtesseract13f/Projects/Gale/Gale/TestGoFiles/expressions.go";
        var stream = CharStreams.fromPath(file);
        var lexer = new GoLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new GoParser(tokens);
          
        //var lexed = new GoLexer()
        IParseTree tree = parser.sourceFile();

        Console.WriteLine(tree.GetText());
        PrintIParseTree(tree);
        var astBuilder = new GaleAstBuilder();
        var ast = astBuilder.VisitSourceFile((GoParser.SourceFileContext)tree);
        
        var mp = new ModuleParameters
        {
            Architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture ==  System.Runtime.InteropServices.Architecture.Arm64 ? TargetArchitecture.ARM64 : TargetArchitecture.AMD64,
            Kind =  ModuleKind.Console
        };

        var gale = new GaleGenerator(mp, "Filename.exe");
        
        var assembly = gale.GenerateProgram((SourceFileAst)ast);
        assembly.Write("Filename.exe");
        //var Gale = new GaleGenerator(assembly);
        //Gale.AddFunctionToPackage(mainProgram, new FunctionAst(){Identifier = "main", Body = (BlockAst)ast});
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