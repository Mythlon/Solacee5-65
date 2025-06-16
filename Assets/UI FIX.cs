using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ForceCanvasOnTop : MonoBehaviour
{
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 999;
    }
}
