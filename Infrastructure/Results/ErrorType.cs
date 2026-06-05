namespace Infrastructure.Results;

public enum ErrorType
{
    NotFound,
    NoChange,
    Validation,
    Conflict,
    Unauthorized,
    Unknown
}