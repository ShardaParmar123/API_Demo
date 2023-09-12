namespace Demo.Types.Dtos.Shared
{
    public class Alert
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
