using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class RouterLoginFlow : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(GetRandCount());
    }

    IEnumerator GetRandCount()
    {
        string url = "http://192.168.100.1/asp/GetRandCount.asp";

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(new byte[0]); // empty body
        req.downloadHandler = new DownloadHandlerBuffer();


        req.SetRequestHeader("Accept", "*/*");
        req.SetRequestHeader("Accept-Encoding", "gzip, deflate");
        req.SetRequestHeader("Accept-Language", "en-PH,en;q=0.9");
        req.SetRequestHeader("Connection", "keep-alive");
        req.SetRequestHeader("Content-Length", "0");
        req.SetRequestHeader("Cookie", "sid=YOUR_SID_VALUE:Language:english:id=1");
        req.SetRequestHeader("Origin", "http://192.168.100.1");
        req.SetRequestHeader("Referer", "http://192.168.100.1/");
        req.SetRequestHeader("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Mobile Safari/537.36");
        req.SetRequestHeader("X-Requested-With", "XMLHttpRequest");

        yield return req.SendWebRequest();

        Debug.Log("Response code: " + req.responseCode);
        Debug.Log("Token: " + req.downloadHandler.text);

        string token = req.downloadHandler.text;
        string username = "telecomadmin";
        string password = "admintelecom";
        string encodedPass = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

        string postData =
            "UserName=" + UnityWebRequest.EscapeURL(username) +
            "&PassWord=" + UnityWebRequest.EscapeURL(encodedPass) +
            "&Language=english" +
            "&x.X_HW_Token=" + token;

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        UnityWebRequest mreq = new UnityWebRequest("http://192.168.100.1/login.cgi", "POST");
        mreq.uploadHandler = new UploadHandlerRaw(bodyRaw);
        mreq.downloadHandler = new DownloadHandlerBuffer();
        yield return mreq.SendWebRequest();
        Debug.Log("Response code: " + mreq.responseCode);
        foreach (var kv in mreq.GetResponseHeaders())
            Debug.Log($"{kv.Key} = {kv.Value}");
    }

    IEnumerator AccessSite() {
        string baseUrl = "http://192.168.100.1";
        UnityWebRequest TokenCapturer = UnityWebRequest.PostWwwForm(baseUrl + "/asp/GetRandCount.asp","");
        yield return TokenCapturer.SendWebRequest();
        string token = TokenCapturer.downloadHandler.text;
        //Debug.Log("Token: " + token);

        string username = "telecomadmin";
        string password = "admintelecom";
        string encodedPass = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(password));


        var formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("UserName="+username+"&PassWord="+encodedPass+"&Language=english&x.X_HW_Token="+token));

        Debug.Log("UserName:" + username);
        Debug.Log("PassWord:" + encodedPass);
        Debug.Log("Language:" + "english");
        Debug.Log("x.X_HW_Token:" + token);

        StartCoroutine(LoginRequest(username,encodedPass,token));


        //using var www = UnityWebRequest.Post("https://www.my-server.com/myform", formData);
        /*using var www = UnityWebRequest.Post(baseUrl+"/login.cgi", formData);
        www.redirectLimit = 0;
        yield return www.SendWebRequest();
        Debug.Log("Request Result:" + www.result);
        Debug.Log("Response code: " + www.responseCode);
        foreach (var kv in www.GetResponseHeaders())
            Debug.Log($"{kv.Key} = {kv.Value}");
        */


        /*UnityWebRequest tokenReq = UnityWebRequest.PostWwwForm(baseUrl + "/asp/GetRandCount.asp", LoginSiteAccess.downloadHandler.text);
        yield return tokenReq.SendWebRequest();
        string token = tokenReq.downloadHandler.text.Trim();
        Debug.Log("Token: " + token);

        string username = "telecomadmin";
        string password = "admintelecom";
        string encodedPass = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

        WWWForm loginForm = new WWWForm();
        loginForm.AddField("UserName", username);
        loginForm.AddField("PassWord", encodedPass);
        loginForm.AddField("Language", "english");
        loginForm.AddField("x.X_HW_Token", token);

        Debug.Log("UserName:"+username);
        Debug.Log("PassWord:"+encodedPass);
        Debug.Log("Language:"+"english");
        Debug.Log("x.X_HW_Token:"+token);

        UnityWebRequest loginReq = UnityWebRequest.Post(baseUrl + "/login.cgi", loginForm);
        loginReq.redirectLimit = 0;
        yield return loginReq.SendWebRequest();
        Debug.Log("Response code: " + loginReq.responseCode);
        foreach (var kv in loginReq.GetResponseHeaders())
            Debug.Log($"{kv.Key} = {kv.Value}");

        */
    }

    IEnumerator LoginRequest(string username, string encodedPass, string token)
    {
        string url = "http://192.168.100.1/login.cgi";

        // Build form-urlencoded body manually
        string postData =
            "UserName=" + UnityWebRequest.EscapeURL(username) +
            "&PassWord=" + UnityWebRequest.EscapeURL(encodedPass) +
            "&Language=english" +
            "&x.X_HW_Token=" + token;

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();

        // Add headers
        req.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        req.SetRequestHeader("Referer", "http://192.168.100.1/index.asp");
        req.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36");
        req.SetRequestHeader("Cookie", "sid=YOUR_SESSION_ID; Language=english; id=1");

        // Debug what YOU are sending
        Debug.Log("=== REQUEST START ===");
        Debug.Log("URL: " + url);
        Debug.Log("Method: POST");
        Debug.Log("Headers:");
        Debug.Log("Content-Type: application/x-www-form-urlencoded");
        Debug.Log("Accept: */*");
        Debug.Log("User-Agent: UnityPlayer/2021.3.0f1");
        Debug.Log("Body: " + postData);
        Debug.Log("=== REQUEST END ===");

        yield return req.SendWebRequest();

        Debug.Log("Response Code: " + req.responseCode);

        Debug.Log("Response Headers:");
        foreach (var kv in req.GetResponseHeaders())
            Debug.Log($"{kv.Key} = {kv.Value}");

        Debug.Log("Response Body: " + req.downloadHandler.text);
    }


    /*IEnumerator LoginFlow()
    {
        string baseUrl = "http://192.168.100.1";

        // 1. Get Token
        UnityWebRequest tokenReq = UnityWebRequest.PostWwwForm(baseUrl + "/asp/GetRandCount.asp", "");
        yield return tokenReq.SendWebRequest();
        string token = tokenReq.downloadHandler.text.Trim();
        Debug.Log("Token: " + token);

        // 2. Login
        string username = "telecomadmin";
        string password = "admintelecom";
        string encodedPass = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

        WWWForm loginForm = new WWWForm();
        loginForm.AddField("UserName", username);
        loginForm.AddField("PassWord", encodedPass);
        loginForm.AddField("Language", "english");
        loginForm.AddField("x.X_HW_Token", token);

        Debug.Log(username);
        Debug.Log(encodedPass);
        Debug.Log(token);

        UnityWebRequest loginReq = UnityWebRequest.Post(baseUrl + "/login.cgi", loginForm);
        //loginReq.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        // prevent auto-redirect
        loginReq.redirectLimit = 0;

        yield return loginReq.SendWebRequest();

        // Debug
        Debug.Log("Response code: " + loginReq.responseCode);
        foreach (var kv in loginReq.GetResponseHeaders())
            Debug.Log($"{kv.Key} = {kv.Value}");

        Debug.Log("Body: " + loginReq.downloadHandler.text);


        /*Debug.Log(loginReq.downloadHandler.text);

        Debug.Log(loginReq.responseCode);

        // Debug ALL headers
        foreach (var kv in loginReq.GetResponseHeaders())
            Debug.Log($"{kv.Key} = {kv.Value}");

        string cookie = null;
        foreach (var kv in loginReq.GetResponseHeaders())
            if (kv.Key.ToLower() == "set-cookie")
                cookie = kv.Value;

        if (cookie == null)
        {
            Debug.LogError("Login failed — no cookie received.");
            yield break;
        }
        Debug.Log("Received cookie: " + cookie);

        // 3. Fetch Dashboard to Extract "onttoken"
        UnityWebRequest dashReq = UnityWebRequest.Get(baseUrl + "/index.asp");
        dashReq.SetRequestHeader("Cookie", cookie);
        yield return dashReq.SendWebRequest();
        string html = dashReq.downloadHandler.text;*/
    //}
}
