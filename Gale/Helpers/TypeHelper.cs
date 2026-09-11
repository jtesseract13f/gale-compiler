using Mono.Cecil;

namespace Gale.Helpers;

public static class TypeHelper
{
    public static TypeReference GetTypeReference(this string typeString, AssemblyDefinition assembly)
    {
        switch (typeString)
        {
            case "string":
                return assembly.MainModule.TypeSystem.String;
                break;
            case "float":
                return assembly.MainModule.TypeSystem.Double;
                break;
            case "float64":
                return assembly.MainModule.TypeSystem.Double;
                break;
            case "int":
                return assembly.MainModule.TypeSystem.Int32;
                break;
            case "long":
                return assembly.MainModule.TypeSystem.Int32;
                break;
            case "void":
                return assembly.MainModule.TypeSystem.Void;
                break;
        }
        return assembly.MainModule.TypeSystem.Single;
    }
}