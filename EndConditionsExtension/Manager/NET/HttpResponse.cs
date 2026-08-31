using System.Net;

namespace EndConditionsExtension.Manager.NET;

internal readonly struct HttpResponse
{
    internal HttpResponse(long statusCode, string body, string error)
    {
        StatusCode = statusCode;
        Body = body;
        Error = error;
    }

    public long StatusCode { get; }

    public string Body { get; }

    public string Error { get; }

    public bool Completed => StatusCode > 0;

    public bool IsSuccess => StatusCode is >= 200 and < 300;

    public HttpStatusCode Status => Completed ? (HttpStatusCode)StatusCode : HttpStatusCode.ServiceUnavailable;

    public string Reason => Error ?? (Completed ? $"HTTP {StatusCode}" : "the server did not answer");
}