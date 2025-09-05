using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class RouterLoginFlow : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return LoginFlow();
    }

    IEnumerator LoginFlow()
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
    }
}
