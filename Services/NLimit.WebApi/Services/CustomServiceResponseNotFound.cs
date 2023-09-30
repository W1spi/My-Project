using System.ComponentModel;

namespace NLimit.WebApi.Services;

public class CustomServiceResponseNotFound
{
    [DefaultValue(StatusCodes.Status404NotFound)]
    public int? Code { get; set; }

    [DefaultValue("Not Found")]
    public string? Message { get; set; }

    public CustomServiceResponseNotFound(int code, string message)
    {
        Code = code;
        Message = message;
    }
}
