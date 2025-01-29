namespace Shared.RequestFeatures;

public class DependantEntityParameters : RequestParameters
{
    public DependantEntityParameters() => OrderBy = "name";

    public string? SearchTerm { get; set; }

}
