namespace Shared.RequestFeatures;

public class DependantEntityParameters : RequestParameters
{
    public DependantEntityParameters() => OrderBy = "name";
    public uint MinAge { get; set; }
    public uint MaxAge { get; set; } = int.MaxValue;

    public string? SearchTerm { get; set; }

}
