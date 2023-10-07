using System.Net;

namespace NLimit.WebApi.Services.ResponseTemplates;

public class CustomResponseExamples
{
    public int Code { get; set; }
    public string Message { get; set; }

    public enum CustomeUserStatusCodes
    {
        EmptyRequestBody,

    }
}
