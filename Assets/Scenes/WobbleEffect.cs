using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonWobble : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform rectTransform;
    bool isHovering = false;
    float wobbleTime = 0f;

    Vector3 originalScale;
    Quaternion originalRotation;

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
            wobbleTime += Time.deltaTime * 10f;
            float wobbleAmount = Mathf.Sin(wobbleTime) * 5f;
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale * 1.1f, 10f * Time.deltaTime);
            rectTransform.localRotation = Quaternion.Euler(0, 0, wobbleAmount);
        }
        else
        {
            wobbleTime = 0;
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

    public void ForceStopWobble()
    {
        isHovering = false;
        rectTransform.localScale = originalScale;
        rectTransform.localRotation = originalRotation;
    }

    
}
