using System;
using System.Collections.Generic;
using MEC;
using UnityEngine.Networking;

namespace EndConditionsExtension.Manager.NET;

internal static class WebQuery
{
    internal const string CoroutineTag = "ECE_Http";

    public static CoroutineHandle Get(string url, Action<HttpResponse> callback = null)
    {
        return Timing.RunCoroutine(Send(UnityWebRequest.Get(url), callback), CoroutineTag);
    }

    private static IEnumerator<float> Send(UnityWebRequest request, Action<HttpResponse> callback)
    {
        using (request)
        {
            request.timeout = 10;

            if (!TrySend(request, out string error))
            {
                Answer(callback, new HttpResponse(0, null, error));
                yield break;
            }

            while (!request.isDone)
                yield return Timing.WaitForOneFrame;

            Answer(callback, Read(request));
        }
    }

    private static bool TrySend(UnityWebRequest request, out string error)
    {
        try
        {
            request.SendWebRequest();
            error = null;
            return true;
        }
        catch (Exception e)
        {
            error = e.Message;
            LogManager.Debug(
                $"Failed to send the {request.method} request to {request.url} - {e.GetType().FullName}: {e.Message}");
            return false;
        }
    }

    private static HttpResponse Read(UnityWebRequest request)
    {
        try
        {
            return new HttpResponse(request.responseCode, request.downloadHandler?.text,
                string.IsNullOrEmpty(request.error) ? null : request.error);
        }
        catch (Exception e)
        {
            LogManager.Debug($"Failed to read the answer of {request.url} - {e.GetType().FullName}: {e.Message}");
            return new HttpResponse(0, null, e.Message);
        }
    }

    private static void Answer(Action<HttpResponse> callback, HttpResponse response)
    {
        try
        {
            callback?.Invoke(response);
        }
        catch (Exception e)
        {
            LogManager.Error("An error occurred while handling the answer of an HTTP request!");
            LogManager.Debug($"Failed to act WebQuery::Answer() - {e.GetType().FullName}: {e.Message}\n{e.StackTrace}");
        }
    }
}