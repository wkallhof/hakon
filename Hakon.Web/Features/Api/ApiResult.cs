namespace Hakon.Web.Features.Api
{
    public class ApiResult{
        public bool Success { get; set; }
        public string Error { get; set; }
    }
    
    public class ApiResult<T> : ApiResult
    {
        public T Data { get; set; }
    }
}