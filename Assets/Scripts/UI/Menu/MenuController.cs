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
        // Using the New Input System's "Quick Check" syntax
        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            bool isOpening = !menuCanvas.activeSelf;
            menuCanvas.SetActive(isOpening);

            // Jei meniu atidarytas -> rodyti pelę. Jei uždarytas -> paslėpti.
            SetCursorState(isOpening);
        }
    }

    void SetCursorState(bool visible)
    {
        Cursor.visible = visible;
    }
}