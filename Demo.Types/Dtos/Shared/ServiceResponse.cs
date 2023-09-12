namespace Demo.Types.Dtos.Shared
{
    public class ServiceResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = false;
        public T? Data { get; set; }
    }
}
