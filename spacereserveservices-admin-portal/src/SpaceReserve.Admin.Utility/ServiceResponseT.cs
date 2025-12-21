namespace SpaceReserve.Utility;
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public string? Error { get; set; } = string.Empty;

        protected internal ServiceResponse( T? data, string? error)
        {
            Data = data;
            Error = error;
        }   

    }
