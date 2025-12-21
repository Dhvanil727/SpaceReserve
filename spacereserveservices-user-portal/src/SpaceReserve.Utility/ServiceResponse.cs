namespace SpaceReserve.Utility;

public class ServiceResponse : ServiceResponse<string>
{
    private ServiceResponse(string? data, string? error) : base(data, error)
    {
    }

    public static ServiceResponse<T> Ok<T>(T data, string? error)
    {
        return new ServiceResponse<T>(data, null);
    }
    public static ServiceResponse Ok()
    {
        return new ServiceResponse(null, null);
    }

    public static ServiceResponse ErrorMessage(string error)
    {
        return new ServiceResponse(null, error);
    }

    public static ServiceResponse NotFound(string error)
    {
        return new ServiceResponse(null, error);
    }

}
