using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainDashboardManager : MonoBehaviour
{
    [Header("Ram Usage")]
    [SerializeField] TextMeshProUGUI RamUsageText;
    [SerializeField] Image RamUsagePercentageImage;

    [Header("Cpu Usage")]
    [SerializeField] TextMeshProUGUI CpuUsageText;
    [SerializeField] Image CpuUsagePercentageImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RamUsageText.text = 0 + "%";
        RamUsagePercentageImage.fillAmount = 0;

        CpuUsageText.text = 0 + "%";
        CpuUsagePercentageImage.fillAmount = 0;
    }

    private void OnEnable()
    {
        StartCoroutine(GetCPUAndRamPercentage());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetCPUAndRamPercentage() {
        yield return new WaitForSecondsRealtime(1f);
        while (true) {
            Debug.Log(ModemDataManager.CurrentSessionAccessPoint);
            Debug.Log(ModemDataManager.CurrentSessionCookie);
            string url = $"http://{ModemDataManager.CurrentSessionAccessPoint}/html/ssmp/deviceinfo/deviceinfo.asp";
            UnityWebRequest req = UnityWebRequest.Get(url);
            req.SetRequestHeader("Cookie", ModemDataManager.CurrentSessionCookie);

            yield return req.SendWebRequest();

            string RequestDataDetails = req.downloadHandler.text;
            Debug.Log("Device Info HTML: " + req.downloadHandler.text);
            // Regex to find var cpuUsed = 'XX%';
            Match cpuMatch = Regex.Match(RequestDataDetails, @"var\s+cpuUsed\s*=\s*'(\d+)%'");
            if (cpuMatch.Success)
            {
                int Cpu = int.Parse(cpuMatch.Groups[1].Value);
                CpuUsageText.text = Cpu + "%";
                CpuUsagePercentageImage.fillAmount = Cpu / 100f;

                if (Cpu > 50)
                {
                    CpuUsageText.color = Color.red;
                    CpuUsagePercentageImage.color = Color.red;
                }
                else if (Cpu < 50)
                {
                    CpuUsageText.color = Color.green;
                    CpuUsagePercentageImage.color = new Color(0.4235294f, 0.8980392f, 0.9098039f);
                }
            }


            // Regex to find var memUsed = 'XX%';
            Match memMatch = Regex.Match(RequestDataDetails, @"var\s+memUsed\s*=\s*'(\d+)%'");
            if (memMatch.Success) {
                int Ram = int.Parse(memMatch.Groups[1].Value);
                RamUsageText.text = Ram +"%";
                RamUsagePercentageImage.fillAmount = Ram / 100f;

                if (Ram > 50)
                {
                    RamUsageText.color = Color.red;
                    RamUsagePercentageImage.color = Color.red;
                }
                else if (Ram < 50) {
                    RamUsageText.color = Color.green;
                    RamUsagePercentageImage.color = new Color(0.4235294f, 0.8980392f, 0.9098039f);
                }
            }
                


            yield return new WaitForSecondsRealtime(0.5f);
        }
    }   
}
