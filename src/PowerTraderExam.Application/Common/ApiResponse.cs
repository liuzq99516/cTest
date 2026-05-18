namespace PowerTraderExam.Application.Common;

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = "success";
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "success") =>
        new() { Code = 0, Message = message, Data = data };

    public static ApiResponse<T> Fail(int code, string message) =>
        new() { Code = code, Message = message };
}
