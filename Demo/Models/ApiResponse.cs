namespace Demo.Models
{
    public class APIResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}