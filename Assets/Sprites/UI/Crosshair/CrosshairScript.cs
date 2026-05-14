// CrosshairScript.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairScript : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        Cursor.visible = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = Object.FindFirstObjectByType<PlayerMovement>();
    }

    void LateUpdate()
    {
        if (Cursor.visible)
        {
            spriteRenderer.enabled = false;
            return;
        }

        spriteRenderer.enabled = true;
        if (Mouse.current == null || Camera.main == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        worldPos.z = 0;
        transform.position = worldPos;

        if (playerMovement != null)
        {
            Vector2 dir = (Vector2)worldPos - (Vector2)playerMovement.transform.position;
            playerMovement.SetFacingDirection(dir.normalized);
        }
    }
}