namespace SpaceReserve.Utility.Resources;

public enum ResultStatus
{
    Success,
    NotFound,
    ErrorMessage
}

public class Result<T>
{
    public bool IsSuccess => Status == ResultStatus.Success;
    public ResultStatus Status { get; set; }
    public T? Value { get; set; }
    public string? Message { get; set; }

    public static Result<T> Success(T value) =>
        new() { Status = ResultStatus.Success, Value = value };

    public static Result<T> ErrorMessage(ResultStatus status, string message) =>
        new() { Status = status, Message = message };
    public static Result<T> NotFound(ResultStatus status, string message) =>
        new() { Status = status, Message = message };
}
