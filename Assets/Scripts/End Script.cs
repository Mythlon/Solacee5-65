using System.Collections;
using UnityEngine;

public class AltarProximityEnd : MonoBehaviour
{
    [Header("Player & Detection")]
    public Transform player;
    public float maxDistance = 20f;
    public float minDistance = 3f;

    [Header("Audio")]
    public AudioSource heartbeatSource;
    public AudioClip heartbeatClip;
    public AnimationCurve heartbeatRateCurve;

    private bool isEnding = false;

    void Awake()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }
    }

    void Start()
    {
        StartCoroutine(HeartbeatController());
    }

    void Update()
    {
        if (isEnding || player == null) return;

        Transform positionSource = player;
        if (player.TryGetComponent<CharacterController>(out var cc))
        {
            positionSource = cc.transform;
        }
        else if (player.childCount > 0)
        {
            positionSource = player.GetChild(0);
        }

        float distance = Vector3.Distance(positionSource.position, transform.position);

        if (distance <= minDistance)
        {
            FindObjectOfType<AltarEndScreen>().TriggerEnd();
            isEnding = true;
        }
    }

    IEnumerator HeartbeatController()
    {
        while (!isEnding)
        {
            if (player == null)
            {
                yield return null;
                continue;
            }

            Transform positionSource = player;
            if (player.TryGetComponent<CharacterController>(out var cc))
            {
                positionSource = cc.transform;
            }
            else if (player.childCount > 0)
            {
                positionSource = player.GetChild(0);
            }

            float distance = Vector3.Distance(positionSource.position, transform.position);

            if (distance > maxDistance)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
            float interval = Mathf.Lerp(1.5f, 0.3f, heartbeatRateCurve.Evaluate(t));

            if (heartbeatSource != null && heartbeatClip != null)
                heartbeatSource.PlayOneShot(heartbeatClip);

            yield return new WaitForSeconds(interval);
        }
    }
}