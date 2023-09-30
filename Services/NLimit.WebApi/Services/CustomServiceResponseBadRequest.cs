using System.ComponentModel;

namespace NLimit.WebApi.Services;

public class CustomServiceResponseBadRequest
{
    [DefaultValue(StatusCodes.Status400BadRequest)]
    public int? Code { get; set; }

    [DefaultValue("Bad Request")]
    public string? Message { get; set; }

    public CustomServiceResponseBadRequest(int code, string message)
    {
        Code = code;
        Message = message;
    }
}
