using Mono.Cecil;

namespace Gale.Types;

public class GlobalFunction
{
    public string Name { get; set; }
    public MethodDefinition Method { get; set; }
    public Action CompileBody { get; set; }
    public string PackageName { get; set; }
}