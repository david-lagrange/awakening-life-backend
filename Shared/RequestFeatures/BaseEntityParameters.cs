namespace Shared.RequestFeatures;

public class BaseEntityParameters : RequestParameters
{
    public BaseEntityParameters() => OrderBy = "name";

    public string? SearchTerm { get; set; }

}
