using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public Image deathPanel;
    public TextMeshProUGUI deathText;
    public float fadeDuration = 1.5f;
    public float delayBeforeReload = 2.5f;

    private bool isDead = false;

    private PlayerCam cameraLookScript;

    public AudioSource ambientAudio;
    public float audioFadeDuration = 2f;
    public CanvasGroup bloodBarGroup;
    public Image bloodBarFill;

    void Start()
    {
        deathPanel.gameObject.SetActive(false);
        deathText.gameObject.SetActive(false);
        cameraLookScript = GameObject.Find("Camera").GetComponent<PlayerCam>();

        if (ambientAudio != null)
        {
            ambientAudio.volume = 0f;
            ambientAudio.Play();
            StartCoroutine(FadeAudioIn(ambientAudio, audioFadeDuration));
        }
    }

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        if (cameraLookScript != null)
            cameraLookScript.enabled = false;

        // Immediately hide blood bar on death
        if (bloodBarGroup != null)
            bloodBarGroup.gameObject.SetActive(false);

        Time.timeScale = 0f;
        StartCoroutine(FadeToDeath());

        if (ambientAudio != null)
            StartCoroutine(FadeAudioOut(ambientAudio, audioFadeDuration));
    }

    private IEnumerator FadeToDeath()
    {
        // Show DEATH text immediately
        deathText.gameObject.SetActive(true);

        // Linger dramatically
        yield return new WaitForSecondsRealtime(1.2f);

        // Start fading in the panel
        deathPanel.gameObject.SetActive(true);
        Color panelColor = deathPanel.color;
        float time = 0f;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            panelColor.a = Mathf.Lerp(0f, 1f, t);
            deathPanel.color = panelColor;
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        panelColor.a = 1f;
        deathPanel.color = panelColor;

        // Wait before reload
        yield return new WaitForSecondsRealtime(delayBeforeReload);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator FadeAudioIn(AudioSource audioSource, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        audioSource.volume = 1f;
    }

    private IEnumerator FadeAudioOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0f;
        while (time < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
