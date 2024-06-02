using System.ComponentModel;

namespace NLimit.WebApi.Services.ResponseTemplates;

public class CustomResponseExamplesAnauthorized
{
    [DefaultValue(StatusCodes.Status401Unauthorized)]
    public int? Code { get; set; }

    [DefaultValue("Anauthorized")]
    public string? Message { get; set; }

    public CustomResponseExamplesAnauthorized(int code, string message)
    {
        Code = code;
        Message = message;
    }
}
