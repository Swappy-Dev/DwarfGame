using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;

    void Start()
    {
        menuCanvas.SetActive(false);
        SetCursorState(false);
    }

    void Update()
    {
        // Jei žaidimas sustabdytas (pvz., žaidėjas mirė), meniu atidaryti negalima
        if (Time.timeScale == 0f && !menuCanvas.activeSelf) return;

        // Naudojame New Input System
        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            bool isOpening = !menuCanvas.activeSelf;
            menuCanvas.SetActive(isOpening);

            SetCursorState(isOpening);

            // Pasirinktinai: sustabdomas/paleidžiamas laikas atidarius meniu žaidimo metu
            // Time.timeScale = isOpening ? 0f : 1f;
        }
    }

    void SetCursorState(bool visible)
    {
        Cursor.visible = visible;
    }
}