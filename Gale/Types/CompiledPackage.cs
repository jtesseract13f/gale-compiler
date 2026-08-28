using Mono.Cecil;

namespace Gale.Types;

public class CompiledPackage
{
    public string Name { get; set; }
    public TypeDefinition PackageType { get; set; }
    public List<GlobalFunction> GlobalFunctions { get; set; }
}