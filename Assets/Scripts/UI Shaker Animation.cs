using System.Collections;
using UnityEngine;

public class UIShakerAnimation
{
    public enum ShakeDirection
    {
        Horizontal,
        Vertical,
        Both
    }

    public static IEnumerator Shake(RectTransform target, ShakeDirection direction = ShakeDirection.Horizontal,
                                    float duration = 0.3f, float strength = 10f, int vibrato = 10)
    {
        Vector2 originalPos = target.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float damper = 1f - (elapsed / duration);
            float offset = Mathf.Sin(elapsed * vibrato * Mathf.PI) * strength * damper;

            Vector2 shakeOffset = Vector2.zero;

            switch (direction)
            {
                case ShakeDirection.Horizontal:
                    shakeOffset = new Vector2(offset, 0);
                    break;
                case ShakeDirection.Vertical:
                    shakeOffset = new Vector2(0, offset);
                    break;
                case ShakeDirection.Both:
                    shakeOffset = new Vector2(offset, offset);
                    break;
            }

            target.anchoredPosition = originalPos + shakeOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to original
        target.anchoredPosition = originalPos;
    }
}
