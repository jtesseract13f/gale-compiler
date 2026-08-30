using System.Diagnostics;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Gale.Ast;
using Gale.Compiler;
using Gale.Semantic;
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
        IParseTree tree = parser.sourceFile();

        Console.WriteLine(tree.GetText());
        PrintIParseTree(tree);
        var gale = new GaleVisitor();
        var ast = gale.VisitSourceFile((GoParser.SourceFileContext)tree);
        
        var mp = new ModuleParameters
        {
            Architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture ==  System.Runtime.InteropServices.Architecture.Arm64 ? TargetArchitecture.ARM64 : TargetArchitecture.AMD64,
            Kind =  ModuleKind.Console
        };

        var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Foo", Version.Parse("1.0.0.0")),
            Path.GetFileName("Foo.dll"), mp);

        var mainProgram = new TypeDefinition("main", "Program", 
            TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed, 
            assembly.MainModule.TypeSystem.Object);

        var Gale = new GaleGenerator(assembly);
        Gale.AddFunctionToPackage(mainProgram, new FunctionAst(){Identifier = "main"});
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