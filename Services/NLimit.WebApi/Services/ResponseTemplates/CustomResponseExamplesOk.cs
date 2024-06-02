using System.ComponentModel;

namespace NLimit.WebApi.Services.ResponseTemplates;

public class CustomResponseExamplesOk
{
    [DefaultValue(StatusCodes.Status200OK)]
    public int? Code { get; set; }

    [DefaultValue("success")]
    public string? Message { get; set; }

    public CustomResponseExamplesOk(int code, string message)
    {
        Code = code;
        Message = message;
    }
}
