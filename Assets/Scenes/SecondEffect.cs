using UnityEngine;
using UnityEngine.EventSystems;

public class DarkPulseEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform rectTransform;
    Vector3 originalScale;
    bool isHovering = false;
    float pulseTimer = 0f;

    [SerializeField] float pulseSpeed = 2.0f;
    [SerializeField] float pulseAmount = 0.05f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    void Update()
    {
        if (isHovering)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float scaleFactor = 1.0f + Mathf.Sin(pulseTimer) * pulseAmount;
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale * scaleFactor, 8f * Time.deltaTime);
        }
        else
        {
            pulseTimer = 0;
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale, 8f * Time.deltaTime);
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
    }
}
