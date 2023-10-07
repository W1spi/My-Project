using System.ComponentModel;

namespace NLimit.WebApi.Services.ResponseTemplates;

public class CustomResponseExamplesBadRequest 
{
    [DefaultValue(StatusCodes.Status400BadRequest)]
    public int? Code { get; set; }

    [DefaultValue("detailed description of the error")]
    public string? Message { get; set; }

    public CustomResponseExamplesBadRequest(int code, string message)
    {
        Code = code;
        Message = message;
    }
}
