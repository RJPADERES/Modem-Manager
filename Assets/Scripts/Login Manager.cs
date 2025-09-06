using System.Collections;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{

    [Header("Access Point Objects")]
    [SerializeField] GameObject LoadingImage;
    [SerializeField] GameObject LoadingTexts;
    [SerializeField] float CurrentLoadingRotationValue;
    [SerializeField] CanvasGroup AccessPointLoadingItems;
    [SerializeField] CanvasGroup AccessPointMainItems;
    [SerializeField] TMP_InputField AccessPointText;
    [SerializeField] GameObject ErrorMessage;
    [SerializeField] GameObject ErrorImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GateWayAccessPointChecker());
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

    // Update is called once per frame
    void Update()
    {
        
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
