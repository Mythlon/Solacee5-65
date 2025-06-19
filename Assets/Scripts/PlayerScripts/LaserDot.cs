using UnityEngine;

public class LaserDotPulse : MonoBehaviour
{
    [Header("Scale Pulse")]
    public float pulseSpeed = 2f;
    public float scaleMin = 0.03f;
    public float scaleMax = 0.06f;

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        float scale = Mathf.Lerp(scaleMin, scaleMax, t);
        transform.localScale = baseScale * scale;
    }
}
