namespace Gale.Ast;

public class Location
{
    public string Document { get; }
    public int Line { get; }
    public int Column { get; }

    public Location(string document, int line, int column)
    {
        Document = document;
        Line = line;
        Column = column;
    }
}

public record Range(Location Start, Location End);

public class QualifiedIdent
{
    public string PackageName { get; }
    public string Identifier { get; }

    public QualifiedIdent(string packageName, string identifier)
    {
        PackageName = packageName;
        Identifier = identifier;
    }
}