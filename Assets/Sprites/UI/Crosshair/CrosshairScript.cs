using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairScript : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Jei tikroji pelė yra matoma (inventorius atidarytas), 
        // paslepiame savo crosshair objektą ir nieko nebevykdome.
        if (Cursor.visible)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

        GetComponent<SpriteRenderer>().enabled = true;

        if (Mouse.current == null || Camera.main == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Gauname tikslią pelės vietą
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        worldPos.z = 0;

        // Priskiriame poziciją be jokių Mathf.Round
        transform.position = worldPos;
    }
}