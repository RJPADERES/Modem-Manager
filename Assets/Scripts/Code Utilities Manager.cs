using System.Collections;
using UnityEngine;

public class CodeUtilitiesManager
{
    //UIBlinker
    public static IEnumerator Blink(CanvasGroup target, float blinkSpeed = 1f)
    {
        while (true)
        {
            // Fade out
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * blinkSpeed;
                target.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }

            // Fade in
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * blinkSpeed;
                target.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
        }
    }
}
