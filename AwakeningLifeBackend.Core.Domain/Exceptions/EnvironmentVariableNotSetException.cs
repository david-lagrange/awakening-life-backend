namespace AwakeningLifeBackend.Core.Domain.Exceptions;
public class EnvironmentVariableNotSetException : Exception
{
    public EnvironmentVariableNotSetException(string variableName) : base($"Environment variable {variableName} is not set.")
    {
    }
}
