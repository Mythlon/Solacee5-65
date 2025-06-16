using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AltarEndScreen : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 1.5f;
    public float delayBeforeSlash = 2f;
    public float delayAfterSlash = 2f;
    public AudioClip slashSound;
    public AudioSource audioSource;

    private PlayerCam cameraLookScript;
    private PlayerMovement movementScript;

    private void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(false);
            Color color = fadePanel.color;
            color.a = 0f;
            fadePanel.color = color;
        }

        cameraLookScript = GameObject.Find("Camera")?.GetComponent<PlayerCam>();
        movementScript = GameObject.FindObjectOfType<PlayerMovement>();
    }

    public void TriggerEnd()
    {
        if (cameraLookScript != null) cameraLookScript.enabled = false;
        if (movementScript != null) movementScript.freeze = true;

        Time.timeScale = 0f;
        StartCoroutine(FadeToEnd());
    }

    private IEnumerator FadeToEnd()
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color panelColor = fadePanel.color;
            float time = 0f;

            while (time < fadeDuration)
            {
                float t = time / fadeDuration;
                panelColor.a = Mathf.Lerp(0f, 1f, t);
                fadePanel.color = panelColor;
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            panelColor.a = 1f;
            fadePanel.color = panelColor;
        }

        yield return new WaitForSecondsRealtime(delayBeforeSlash);

        if (audioSource != null && slashSound != null)
            audioSource.PlayOneShot(slashSound);

        yield return new WaitForSecondsRealtime(delayAfterSlash);

        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
