namespace AxelCMS.Domain
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public  List<string> Errors { get; set; }
        public T Data { get; set; }

        public ApiResponse(bool isSucceeded, string message, int statusCode, T data, List<string> errors)
        {
            Succeeded = isSucceeded;
            Message = message;
            StatusCode = statusCode;
            Data = data;
            Errors = errors;
        }
        public ApiResponse(bool isSucceeded, string message, int statusCode, List<string> errors)
        {
            Succeeded = isSucceeded;
            Message = message;
            StatusCode = statusCode;
            Errors = errors;
        }
        public ApiResponse(bool isSucceeded, string message, int statusCode)
        {
            Succeeded = isSucceeded;
            Message = message;
            StatusCode = statusCode;
        }
        public ApiResponse(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }
        public ApiResponse(bool isSucceeded, T data, List<string> errors)
        {
            Succeeded = isSucceeded;
            Data = data;
            Errors = errors;
        }
        public static ApiResponse<T> Success(T data, string message, int statusCode)
        {
            return new ApiResponse<T>(true, message, statusCode, data, new List<string>());
        }
        public static ApiResponse<T> Failed(bool isSucceeded, string message, int statusCode, List<string> errors)
        {
            return new ApiResponse<T>(isSucceeded, message, statusCode, errors);
        }
        public static ApiResponse<T> Failed(List<string> errors)
        {
            return new ApiResponse<T>(false, default, errors);
        }
    }
}
