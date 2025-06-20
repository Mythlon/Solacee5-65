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
    private const float defaultVolume = 20f;

    public ButtonWobble[] allWobbleButtons;

    void Start()
    {
        optionsMenu.SetActive(false);
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);


        // Чувствительность мыши
        if (!PlayerPrefs.HasKey("MouseSensitivity"))
            PlayerPrefs.SetFloat("MouseSensitivity", defaultSensitivity);

        float sens = PlayerPrefs.GetFloat("MouseSensitivity");
        sensitivitySlider.value = sens;
        UpdateSensitivityText(sens);

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
        Debug.Log("Options button clicked!");
        //foreach (var wobble in allWobbleButtons)
        //{
        //    wobble.ForceStopWobble();
        //}
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);

    }

    public void SaveAndBack()
    {

        PlayerPrefs.Save();


        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public TextMeshProUGUI sensitivityValueText;

    public void UpdateSensitivityText(float value)
    {

        sensitivityValueText.text = value.ToString("0.0");
    }

    public TextMeshProUGUI volumeValueText;





}
