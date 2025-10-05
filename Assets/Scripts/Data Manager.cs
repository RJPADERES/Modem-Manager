using System;
using System.Collections;
using System.Text;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class ModemDataManager
{
    public static string CurrentSessionCookie;
    public static string CurrentSessionAccessPoint;

    public static IEnumerator RequestLoginToken(string ROUTER_IP, string USERNAME, string PASSWORD, Action<bool> onResult, Action<string> onCookie)
    {
        //Base64 encode password
        string encodedPass = Convert.ToBase64String(Encoding.UTF8.GetBytes(PASSWORD));

        //Get token
        string tokenUrl = $"http://{ROUTER_IP}/asp/GetRandCount.asp";
        UnityWebRequest tokenReq = UnityWebRequest.PostWwwForm(tokenUrl, "");
        tokenReq.redirectLimit = 0;
        yield return tokenReq.SendWebRequest();

        if (tokenReq.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Token request failed: " + tokenReq.error);
            onResult?.Invoke(false);
            onCookie?.Invoke(null);
            yield break;
        }

        string token = tokenReq.downloadHandler.text.Trim('\uFEFF').Trim();
        Debug.Log("Token: " + token);

        //Prepare login request
        string loginUrl = $"http://{ROUTER_IP}/login.cgi";
        WWWForm form = new WWWForm();
        form.AddField("UserName", USERNAME);
        form.AddField("PassWord", encodedPass);
        form.AddField("Language", "english");
        form.AddField("x.X_HW_Token", token);

        UnityWebRequest loginReq = UnityWebRequest.Post(loginUrl, form);
        loginReq.redirectLimit = 0;
        yield return loginReq.SendWebRequest();

        //Get cookies
        string setCookie = loginReq.GetResponseHeader("Set-Cookie");
        Debug.Log("Set-Cookie Header: " + setCookie);

        if (string.IsNullOrEmpty(setCookie))
        {
            Debug.LogError("Login failed");
            onResult?.Invoke(false);
            onCookie?.Invoke(null);
        }
        else
        {
            Debug.Log("Logged in successfully!");
            onResult?.Invoke(true);
            onCookie?.Invoke(setCookie);
        }
    }

    /*IEnumerator LoginRoutine()
    {
        //Base64 encode password
        string encodedPass = Convert.ToBase64String(Encoding.UTF8.GetBytes(PASSWORD));

        //Get token
        string tokenUrl = $"http://{ROUTER_IP}/asp/GetRandCount.asp";
        UnityWebRequest tokenReq = UnityWebRequest.PostWwwForm(tokenUrl, "");
        tokenReq.redirectLimit = 0;
        yield return tokenReq.SendWebRequest();

        if (tokenReq.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Token request failed: " + tokenReq.error);
            yield break;
        }

        string token = tokenReq.downloadHandler.text.Trim('\uFEFF').Trim(); // strip BOM + whitespace
        Debug.Log("Token: " + token);

        //Prepare login request
        string loginUrl = $"http://{ROUTER_IP}/login.cgi";
        WWWForm form = new WWWForm();
        form.AddField("UserName", USERNAME);
        form.AddField("PassWord", encodedPass);
        form.AddField("Language", "english");
        form.AddField("x.X_HW_Token", token);

        UnityWebRequest loginReq = UnityWebRequest.Post(loginUrl, form);
        loginReq.redirectLimit = 0;
        yield return loginReq.SendWebRequest();

        if (loginReq.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Login request failed: " + loginReq.error);
            yield break;
        }

        //Get cookies
        string setCookie = loginReq.GetResponseHeader("Set-Cookie");
        Debug.Log("Set-Cookie Header: " + setCookie);

        // Check success
        if (loginReq.downloadHandler.text.Contains("Logout") || loginReq.responseCode == 200)
        {
            Debug.Log("Logged in successfully!");
        }
        else
        {
            Debug.LogError("Login failed");
        }

        //StartCoroutine(GetDevicesOnModem(setCookie));
    }

    IEnumerator GetDevicesOnModem(string cookie)
    {
        string url = $"http://{ROUTER_IP}/html/bbsp/common/GetLanUserDevInfo.asp";

        UnityWebRequest req = UnityWebRequest.Get(url);  // usually a GET, not POST
        req.redirectLimit = 0;

        // Attach cookie header
        req.SetRequestHeader("Cookie", cookie);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Devices: " + req.downloadHandler.text);
            var devices = RouterParser.ParseDevices(req.downloadHandler.text);

            foreach (var dev in devices)
            {
                Debug.Log(dev.ToString());
            }
        }
        else
        {
            Debug.LogError("Request failed: " + req.error);
        }
    }*/
}
