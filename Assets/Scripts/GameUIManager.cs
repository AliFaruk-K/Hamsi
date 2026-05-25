using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject deathPanel;

    [SerializeField] private TextMeshProUGUI deathScoreText;

    // Ölüm ekranýnda skora göre gösterilecek fotoðraflar
    [SerializeField] private GameObject[] scoreImages; // 0=düþük, 1=orta, 2=yüksek
    [SerializeField] private int[] scoreThresholds;    // eþik deðerleri, örn: {100, 300}

    [SerializeField] private TextMeshProUGUI joystickButonText;
    [SerializeField] private GameObject joystickLeft;
    [SerializeField] private GameObject joystickRight;
    [SerializeField] private TextMeshProUGUI sesButonText;
    [SerializeField] private AudioSource[] allSound;

    private bool sesAcik = true;
    private bool joystickSolda = true;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gamePanel.SetActive(false);
        deathPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(true);
        deathPanel.SetActive(false);
        Time.timeScale = 1f;
        if (joystickSolda)
        {
            joystickLeft.SetActive(true);
        }
        else
        {
            joystickRight.SetActive(true);
        }
        if (sesAcik)
        {
            foreach (var sound in allSound)
                sound.mute = false;
        }
        else
        {
            foreach (var sound in allSound)
                sound.mute = true;
        }

    }
    
    public void JoystickDegistir()
    {
        if(joystickSolda)
        {
            joystickSolda = false;
            joystickButonText.text = "Joystick Saðda";

        }
        else
        {
            joystickSolda = true;
            joystickButonText.text = "Joystick Solda";
        }
    }

    public void SesDegistir()
    {
        if (sesAcik)
        {
            sesAcik = false;
            sesButonText.text = "Ses Kapalý";
        }
        else
        {
            sesAcik = true;
            sesButonText.text = "Ses Açýk";
        }
    }

    public void ShowDeathScreen(int score)
    {
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(false);
        deathPanel.SetActive(true);
        Time.timeScale = 0f;

        deathScoreText.text = "Skor: " + score.ToString();

        // Skora göre fotoðraf seç
        foreach (var img in scoreImages)
            img.SetActive(false);

        int imageIndex = 0;
        for (int i = 0; i < scoreThresholds.Length; i++)
        {
            if (score >= scoreThresholds[i])
                imageIndex = i + 1;
        }
        imageIndex = Mathf.Clamp(imageIndex, 0, scoreImages.Length - 1);
        scoreImages[imageIndex].SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}