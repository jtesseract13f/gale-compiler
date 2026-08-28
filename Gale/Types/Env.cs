using Mono.Cecil;

namespace Gale.Types;

public class Env
{
    public ModuleDefinition Module { get; set; }
    public TypeReference VoidType { get; set; }
    public TypeReference ObjectType { get; set; }
    public TypeReference StringType { get; set; }
    public TypeReference ConsoleType { get; set; }
    public string PackageName { get; set; }

    public Env Push() => this;
}