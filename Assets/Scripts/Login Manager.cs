using System.Collections;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("Page Windows")]
    [SerializeField] CanvasGroup LoginPage;
    [SerializeField] CanvasGroup LoadingPage;

    [Header("Access Point Objects")]
    [SerializeField] GameObject LoadingImage;
    [SerializeField] GameObject LoadingTexts;
    [SerializeField] float CurrentLoadingRotationValue;
    [SerializeField] CanvasGroup AccessPointLoadingItems;
    [SerializeField] CanvasGroup AccessPointMainItems;
    [SerializeField] TMP_InputField AccessPointText;
    [SerializeField] GameObject ErrorMessage;
    [SerializeField] GameObject ErrorImage;
    [SerializeField] GameObject EditLine;
    [Header("Username & Password")]
    [SerializeField] TMP_InputField UserName;
    [SerializeField] TMP_InputField Password;
    [Header("Logging In")]
    [SerializeField] GameObject RememberCheckMark;
    [SerializeField] GameObject ErrorLoggingInText;


    //InputField Items
    [SerializeField] GameObject GatewayInputFieldText;
    [SerializeField] GameObject TextArea;

    Vector3 GatewayInputFieldOriginalPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("AccessPoint"))
        {
            LoginPage.alpha = 0f;
            LoginPage.blocksRaycasts = false;
            LoginPage.interactable = false;

            LoadingPage.alpha = 1f;
            LoadingPage.blocksRaycasts = true;
            LoadingPage.interactable = true;

            string AccessPoint = PlayerPrefs.GetString("AccessPoint"); // "Guest" = default
            string UserName = PlayerPrefs.GetString("Username"); // "Guest" = default
            string Password = PlayerPrefs.GetString("Password"); // "Guest" = default

            StartCoroutine(ModemDataManager.RequestLoginToken(AccessPoint, UserName, Password, success =>
            {
                if (success)
                {
                    Debug.Log("Proceed to dashboard UI");

                    LoadingPage.alpha = 0f;
                    LoadingPage.blocksRaycasts = false;
                    LoadingPage.interactable = false;
                }
                else
                {
                    Debug.Log("Show error message");

                    LoginPage.alpha = 1f;
                    LoginPage.blocksRaycasts = true;
                    LoginPage.interactable = true;

                    LoadingPage.alpha = 0f;
                    LoadingPage.blocksRaycasts = false;
                    LoadingPage.interactable = false;
                    StartCoroutine(UIShakerAnimation.Shake(ErrorLoggingInText.GetComponent<RectTransform>(), UIShakerAnimation.ShakeDirection.Horizontal));
                    ErrorLoggingInText.SetActive(true);
                    PlayerPrefs.DeleteAll();
                }
            }));
        }
        else
        {
            LoginPage.alpha = 1f;
            LoginPage.blocksRaycasts = true;
            LoginPage.interactable = true;

            LoadingPage.alpha = 0f;
            LoadingPage.blocksRaycasts = false;
            LoadingPage.interactable = false;

            EditLine.SetActive(false);
            StartCoroutine(GateWayAccessPointChecker());
            GatewayInputFieldText.transform.localPosition = GatewayInputFieldOriginalPosition;
        }
    }

    IEnumerator GateWayAccessPointChecker()
    {
        while (true)
        {
            StartCoroutine(LoadingGatewaypoint());
            yield return new WaitForSeconds(5f);
            string gatewayIP = GetGatewayIPAddress();

            if (string.IsNullOrEmpty(gatewayIP))
            {
                Debug.LogWarning("No gateway detected. Are you connected to a router?");

                ErrorMessage.SetActive(true);
                ErrorImage.SetActive(true);

                LoadingImage.SetActive(false);
                LoadingTexts.SetActive(false);
            }
            else
            {
                Debug.Log("Gateway IP: " + gatewayIP);

                AccessPointLoadingItems.alpha = 0f;
                AccessPointLoadingItems.interactable = false;
                AccessPointLoadingItems.blocksRaycasts = false;

                AccessPointMainItems.alpha = 1f;
                AccessPointMainItems.interactable = true;
                AccessPointMainItems.blocksRaycasts = true;

                AccessPointText.text = gatewayIP;

                LoadingImage.SetActive(false);

                yield break;
            }
        }
    }

    //For Clicking On Edit Access Point
    public void OnAccessPointEditButtonPress() {
        AccessPointText.interactable = !AccessPointLoadingItems.interactable;
        if (AccessPointText.interactable) {
            //Access Point Input Field Input Validators
            EventSystem.current.SetSelectedGameObject(AccessPointText.gameObject);
            AccessPointText.onValidateInput += ValidateChar;
            AccessPointText.onEndEdit.AddListener(ValidateIpAddress);
            EditLine.SetActive(true);
        }
    }

    //Input Validator For Input only Numbers and Dots In Access Point Input Field
    private string ipPattern = @"^(\d{1,3}\.){3}\d{1,3}$";
    private char ValidateChar(string text, int index, char addedChar)
    {
        // Allow only digits and dot
        if (char.IsDigit(addedChar) || addedChar == '.')
            return addedChar;

        return '\0';
    }

    private void ResetInputFieldText(string ip)
    {
        AccessPointText.text = ip;

        GatewayInputFieldText.transform.localPosition = GatewayInputFieldOriginalPosition;
        TextArea.transform.GetChild(0).gameObject.transform.localPosition = GatewayInputFieldOriginalPosition;

        AccessPointText.ForceLabelUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(AccessPointText.GetComponent<RectTransform>());
    }

    private void ValidateIpAddress(string input)
    {
        if (Regex.IsMatch(input, ipPattern))
        {
            string[] parts = input.Split('.');
            foreach (string part in parts)
            {
                if (int.Parse(part) > 255)
                {
                    ResetInputFieldText(GetGatewayIPAddress());
                    Debug.LogWarning("Invalid IP segment: must be between 0 and 255");
                    StartCoroutine(UIShakerAnimation.Shake(AccessPointMainItems.GetComponent<RectTransform>(), UIShakerAnimation.ShakeDirection.Horizontal));
                    StartCoroutine(DisableInputFieldNextFrame());
                    EditLine.SetActive(false);
                    return;
                }
            }
            Debug.Log("Valid IP: " + input);
        }
        else
        {
            ResetInputFieldText(GetGatewayIPAddress());
            StartCoroutine(UIShakerAnimation.Shake(AccessPointMainItems.GetComponent<RectTransform>(), UIShakerAnimation.ShakeDirection.Horizontal));
            Debug.LogWarning("Invalid IP format");
        }

        AccessPointText.onValidateInput -= ValidateChar;
        AccessPointText.onEndEdit.RemoveListener(ValidateIpAddress);
        StartCoroutine(DisableInputFieldNextFrame());
        EditLine.SetActive(false);
    }

    private IEnumerator DisableInputFieldNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        AccessPointText.interactable = false;
    }

    IEnumerator LoadingGatewaypoint() {
        ErrorImage.SetActive(false);
        ErrorMessage.SetActive(false);

        AccessPointLoadingItems.alpha = 1f;
        AccessPointLoadingItems.interactable = true;
        AccessPointLoadingItems.blocksRaycasts = true;

        AccessPointMainItems.alpha = 0f;
        AccessPointMainItems.interactable = false;
        AccessPointMainItems.blocksRaycasts = false;

        LoadingTexts.SetActive(true);
        LoadingImage.SetActive(true);
        while (LoadingImage.activeInHierarchy) {
            CurrentLoadingRotationValue = CurrentLoadingRotationValue + 0.5f * Time.deltaTime;
            LoadingImage.transform.Rotate(0, 0, LoadingImage.transform.rotation.z - CurrentLoadingRotationValue);
            yield return null;
        }
        LoadingTexts.SetActive(false);
        LoadingImage.SetActive(false);
    }

    //Remember Me Button
    public void RememberMeButtonPress() {
        RememberCheckMark.SetActive(!RememberCheckMark.activeInHierarchy);
    }

    //For Login Button
    public void OnLoginPress() {
        LoginPage.alpha = 0f;
        LoginPage.blocksRaycasts = false;
        LoginPage.interactable = false;

        LoadingPage.alpha = 1f;
        LoadingPage.blocksRaycasts = true;
        LoadingPage.interactable = true;

        StartCoroutine(ModemDataManager.RequestLoginToken(AccessPointText.text,UserName.text,Password.text, success =>
        {
            if (success)
            {
                Debug.Log("Proceed to dashboard UI");

                LoadingPage.alpha = 0f;
                LoadingPage.blocksRaycasts = false;
                LoadingPage.interactable = false;

                if (RememberCheckMark.activeInHierarchy) {
                    PlayerPrefs.SetString("AccessPoint",AccessPointText.text);
                    PlayerPrefs.SetString("Username",UserName.text);
                    PlayerPrefs.SetString("Password",Password.text);
                    PlayerPrefs.Save();
                }
            }
            else {
                Debug.Log("Show error message");

                LoginPage.alpha = 1f;
                LoginPage.blocksRaycasts = true;
                LoginPage.interactable = true;

                LoadingPage.alpha = 0f;
                LoadingPage.blocksRaycasts = false;
                LoadingPage.interactable = false;
                StartCoroutine(UIShakerAnimation.Shake(ErrorLoggingInText.GetComponent<RectTransform>(), UIShakerAnimation.ShakeDirection.Horizontal));
                ErrorLoggingInText.SetActive(true);
            }
        }));
    }

    public static string GetGatewayIPAddress()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus == OperationalStatus.Up &&
                (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                 ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
            {
                var props = ni.GetIPProperties();
                foreach (var gateway in props.GatewayAddresses)
                {
                    var addr = gateway.Address;
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        string ip = addr.ToString();
                        if (ip != "0.0.0.0" && !ip.EndsWith(".255"))
                        {
                            return ip; // found a valid gateway
                        }
                    }
                }
            }
        }
        return null; // no valid gateway found
    }
}
