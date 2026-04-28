using UnityEngine;
using UnityEngine.InputSystem; // Add this!

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        // Using the New Input System's "Quick Check" syntax
        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }
}