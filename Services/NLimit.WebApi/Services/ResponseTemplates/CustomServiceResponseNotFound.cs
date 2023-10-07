using System.ComponentModel;

namespace NLimit.WebApi.Services.ResponseTemplates;

public class CustomResponseExamplesNotFound
{
    [DefaultValue(StatusCodes.Status404NotFound)]
    public int? Code { get; set; }

    [DefaultValue("detailed description of the error")]
    public string? Message { get; set; }

    public CustomResponseExamplesNotFound(int code, string message)
    {
        Code = code;
        Message = message;
    }
}
