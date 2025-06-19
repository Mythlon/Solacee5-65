using UnityEngine;
using UnityEngine.EventSystems;

public class GlitchyButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform rectTransform;
    Vector3 originalScale;
    Quaternion originalRotation;
    bool isHovering = false;
    float noiseTime = 0f;

    [Header("Glitch Settings")]
    public float noiseSpeed = 2.5f;
    public float scaleAmount = 0.04f;
    public float rotationAmount = 4f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation;
    }

    void Update()
    {
        if (isHovering)
        {
            noiseTime += Time.deltaTime * noiseSpeed;

            float noiseX = Mathf.PerlinNoise(noiseTime, 0f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(0f, noiseTime) - 0.5f;

            Vector3 targetScale = originalScale + new Vector3(noiseX, noiseY, 0) * scaleAmount * 2f;
            Quaternion targetRot = Quaternion.Euler(0, 0, noiseX * rotationAmount);

            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, 10f * Time.deltaTime);
            rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, targetRot, 10f * Time.deltaTime);
        }
        else
        {
            noiseTime = 0f;
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale, 10f * Time.deltaTime);
            rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, originalRotation, 10f * Time.deltaTime);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void ForceStop()
    {
        isHovering = false;
        rectTransform.localScale = originalScale;
        rectTransform.localRotation = originalRotation;
    }
}
