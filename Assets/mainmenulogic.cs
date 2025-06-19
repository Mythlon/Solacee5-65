using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenu;
    public GameObject optionsMenu;

    [Header("Sliders")]
    public Slider sensitivitySlider;
    public Slider volumeSlider;

    private const float defaultSensitivity = 2.5f;
    private const float defaultVolume = 100f;

    public ButtonWobble[] allWobbleButtons;

    void Start()
    {
        optionsMenu.SetActive(false);
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        AudioListener.volume = volumeSlider.value;

        // Чувствительность мыши
        if (!PlayerPrefs.HasKey("MouseSensitivity"))
            PlayerPrefs.SetFloat("MouseSensitivity", defaultSensitivity);

        float sens = PlayerPrefs.GetFloat("MouseSensitivity");
        sensitivitySlider.value = sens;
        UpdateSensitivityText(sens);

        // Громкость
        if (!PlayerPrefs.HasKey("MasterVolume"))
            PlayerPrefs.SetFloat("MasterVolume", defaultVolume);

        float vol = PlayerPrefs.GetFloat("MasterVolume");
        volumeSlider.value = vol;
        UpdateVolumeText(vol);

        AudioListener.volume = vol;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SolaceLevel");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenOptions()
    {

        foreach (var wobble in allWobbleButtons)
        {
            wobble.ForceStopWobble();
        }

        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void SaveAndBack()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value);
        PlayerPrefs.SetFloat("MasterVolume", volumeSlider.value);
        PlayerPrefs.Save();

        AudioListener.volume = volumeSlider.value;

        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public TextMeshProUGUI sensitivityValueText;

    public void UpdateSensitivityText(float value)
    {

        sensitivityValueText.text = value.ToString("0.0");
    }

    public TextMeshProUGUI volumeValueText;

    public void UpdateVolumeText(float value)
    {
        volumeValueText.text = value.ToString("0.0");
    }




}
