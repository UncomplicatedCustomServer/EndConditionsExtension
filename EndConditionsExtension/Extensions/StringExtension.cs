using System;
using System.Net;
using System.Text.Json;
using EndConditionsExtension.Manager;

namespace EndConditionsExtension.Extensions;

internal static class StringExtension
{
    /// <summary>
    ///     Reads the status code (and the message) of a JSON answer of the UCS cloud.
    /// </summary>
    /// <returns>
    ///     <see cref="HttpStatusCode.Unused" /> when the answer carries no status code, which is the case of every
    ///     successful answer.
    /// </returns>
    public static HttpStatusCode GetStatusCode(this string str, out string message)
    {
        LogManager.Debug($"Parsing JSON for status code: {str}");

        message = null;

        JsonDocument doc;

        try
        {
            doc = JsonDocument.Parse(str);
        }
        catch (Exception e)
        {
            LogManager.Debug($"The answer is not a valid JSON ({e.Message}), returning HttpStatusCode.Unused");
            message = str;
            return HttpStatusCode.Unused;
        }

        var root = doc.RootElement;

        if (root.TryGetProperty("message", out var messageElement))
        {
            message = messageElement.GetString();
            LogManager.Debug($"Extracted message: {message}");
        }

        if (root.TryGetProperty("status", out var status) &&
            Enum.TryParse(status.ToString(), out HttpStatusCode statusCode))
        {
            LogManager.Debug($"Extracted status code: {statusCode}");
            return statusCode;
        }

        LogManager.Debug("Status code not found, returning HttpStatusCode.Unused");
        return HttpStatusCode.Unused;
    }
}