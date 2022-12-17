namespace ExternalSort;

public class Helpers
{
    public static (int, string) ParseLine(string line)
    {
        var parts = line.Split(". ");
        return (int.Parse(parts[0]), parts[1]);
    }
}
