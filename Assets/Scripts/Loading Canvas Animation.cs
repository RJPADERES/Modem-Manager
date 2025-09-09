using System.Collections;
using UnityEngine;

public class LoadingCanvasAnimation : MonoBehaviour
{
    [SerializeField] CanvasGroup LoadingCanvas;
    [SerializeField] CanvasGroup Icon;
    [SerializeField] GameObject[] WifiHeads;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animationplaying = false;
        Icon.alpha = 1f;
        WifiHeads[0].SetActive(false);
        WifiHeads[1].SetActive(false);
        WifiHeads[2].SetActive(false);
    }

    bool animationplaying;
    // Update is called once per frame
    void Update()
    {
        if (LoadingCanvas.alpha > 0 && !animationplaying) {
            animationplaying = true;
            StartCoroutine(LoadingCanvasAnimationOnBackground());
        }
    }

    int currentwifihead = 0;
    IEnumerator LoadingCanvasAnimationOnBackground() {
        StartCoroutine(CodeUtilitiesManager.Blink(Icon,1f));
        while (LoadingCanvas.alpha > 0) {
            if (currentwifihead == 0)
            {
                WifiHeads[0].SetActive(true);
                WifiHeads[1].SetActive(false);
                WifiHeads[2].SetActive(false);
            }
            else if (currentwifihead == 1) {
                WifiHeads[0].SetActive(true);
                WifiHeads[1].SetActive(true);
                WifiHeads[2].SetActive(false);
            }
            else if (currentwifihead == 2)
            {
                WifiHeads[0].SetActive(true);
                WifiHeads[1].SetActive(true);
                WifiHeads[2].SetActive(true);
                currentwifihead = 0;

                yield return new WaitForSeconds(1f);
                WifiHeads[0].SetActive(false);
                WifiHeads[1].SetActive(false);
                WifiHeads[2].SetActive(false);
            }
            currentwifihead++;
            yield return new WaitForSeconds(0.5f);
        }
        animationplaying = false;
        Icon.alpha = 1f;
        WifiHeads[0].SetActive(false);
        WifiHeads[1].SetActive(false);
        WifiHeads[2].SetActive(false);
    }
}
