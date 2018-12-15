namespace RedditFlairs.Web.Models
{
    public class ApiResult<TData>
    {
        public TData Data { get; set; }
        public string Error { get; set; }
        public bool Success => string.IsNullOrEmpty(Error);

        public static ApiResult<TData> FromError(string error)
        {
            return new ApiResult<TData> {Error = error};
        }

        public static ApiResult<TData> FromResult(TData result)
        {
            return new ApiResult<TData> {Data = result};
        }
    }
}