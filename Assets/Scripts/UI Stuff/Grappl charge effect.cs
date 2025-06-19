using UnityEngine;

public class GrappleChargeEffect : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector3 originalPosition;

    public float shakeIntensity = 5f;
    public float wobbleSpeed = 4f;

    private float effectTime = 0f;
    private bool active = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (active)
        {
            effectTime += Time.deltaTime * wobbleSpeed;

            float offsetX = Mathf.PerlinNoise(effectTime, 0f) - 0.5f;
            float offsetY = Mathf.PerlinNoise(0f, effectTime) - 0.5f;

            rectTransform.anchoredPosition = (Vector2)originalPosition + new Vector2(offsetX, offsetY) * shakeIntensity;

        }
        else
        {
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, originalPosition, Time.deltaTime * 10f);
        }
    }

    public void TriggerMeatSplat()
    {
        StopAllCoroutines();
        StartCoroutine(DoSplat());
    }

    System.Collections.IEnumerator DoSplat()
    {
        active = true;
        rectTransform.localScale = originalScale * 1.2f;

        yield return new WaitForSeconds(0.07f);
        rectTransform.localScale = originalScale * 0.95f;

        yield return new WaitForSeconds(0.07f);
        rectTransform.localScale = originalScale;

        yield return new WaitForSeconds(0.1f);
        active = false;
    }
}
