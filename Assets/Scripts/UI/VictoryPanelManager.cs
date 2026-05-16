using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Šį skriptą pridedam prie VictoryPanel GameObject Unity'je
public class VictoryPanelManager : MonoBehaviour
{
    [Header("Mygtukų nuorodos")]
    public Button playAgainButton;  // Priskirk Inspector lange
    public Button mainMenuButton;   // Priskirk Inspector lange

    [Header("Scenų pavadinimai")]
    public string gameSceneName = "GameScene";     // ← Pakeisk į savo žaidimo scenos pavadinimą
    public string mainMenuSceneName = "MainMenu";  // ← Pakeisk į savo meniu scenos pavadinimą

    void Awake()
    {
        // Panel turi būti paslėptas nuo pat pradžių
        gameObject.SetActive(false);
    }

    void Start()
    {
        // Prijungiame mygtukų įvykius
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgain);
        else
            Debug.LogWarning("Play Again mygtukas nepriskirtas Inspector lange!");

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenu);
        else
            Debug.LogWarning("Main Menu mygtukas nepriskirtas Inspector lange!");
    }

    // Ši funkcija iškviečiama iš BOSS_AI kai bossas miršta
    public void ShowVictory()
    {
        gameObject.SetActive(true);

        // Pristabdome laiką — žaidėjas mato ekraną ramiai
        // Jei nenori pristabdyti — šią eilutę ištrink
        Time.timeScale = 0f;
    }

    // --- MYGTUKŲ LOGIKA ---

    void OnPlayAgain()
    {
        Debug.Log("Play Again paspaustas!");
        Time.timeScale = 1f; // Atstatome laiką prieš kraunant sceną
        SceneManager.LoadScene(gameSceneName);
    }

    void OnMainMenu()
    {
        Debug.Log("Main Menu paspaustas!");
        Time.timeScale = 1f; // Atstatome laiką
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDestroy()
    {
        // Išvalom listener'ius kad nebūtų memory leak
        if (playAgainButton != null) playAgainButton.onClick.RemoveListener(OnPlayAgain);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(OnMainMenu);
    }
}