using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryPanelManager : MonoBehaviour
{
    [Header("Mygtukų nuorodos")]
    public Button playAgainButton;  // Priskirk Inspector lange
    public Button mainMenuButton;   // Priskirk Inspector lange

    [Header("Scenų pavadinimai")]
    public string gameSceneName = "GameScene";     // Pakeisk į savo žaidimo scenos pavadinimą
    public string mainMenuSceneName = "MainMenu";  // Pakeisk į savo meniu scenos pavadinimą

    void Awake()
    {
        // --- SUTVARKYTA ---
        // Mygtukų klausytojus (Listeners) jungiame Awake funkcijoje.
        // Tai suveiks net jei objektas išjungiamas iškart po to arba vėliau.
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgain);
        else
            Debug.LogWarning("Play Again mygtukas nepriskirtas Inspector lange!");

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenu);
        else
            Debug.LogWarning("Main Menu mygtukas nepriskirtas Inspector lange!");

        // Paslepiame panelę pačioje Awake pabaigoje
        gameObject.SetActive(false);
    }

    // Ši funkcija iškviečiama iš BOSS_AI kai bossas miršta
    public void ShowVictory()
    {
        gameObject.SetActive(true);

        // Kadangi boso skripte sustabdei žaidėją, laiko stabdymas (Time.timeScale = 0f) 
        // yra nebūtinas, bet jei nori visiškai įšaldyti pasaulį (pvz. animacijas), gali jį palikti:
        // Time.timeScale = 0f; 
    }

    // --- MYGTUKŲ LOGIKA ---

    void OnPlayAgain()
    {
        Debug.Log("Play Again paspaustas! Kraunama scena: " + gameSceneName);
        Time.timeScale = 1f; // Atstatome laiką prieš kraunant sceną
        SceneManager.LoadScene(gameSceneName);
    }

    void OnMainMenu()
    {
        Debug.Log("Main Menu paspaustas! Kraunama scena: " + mainMenuSceneName);
        Time.timeScale = 1f; // Svarbu: Atstatome laiką, kad pagrindiniame meniu viskas veiktų
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDestroy()
    {
        // Išvalom listener'ius kad nebūtų memory leak
        if (playAgainButton != null) playAgainButton.onClick.RemoveListener(OnPlayAgain);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(OnMainMenu);
    }
}