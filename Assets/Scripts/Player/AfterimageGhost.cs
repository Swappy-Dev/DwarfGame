using UnityEngine;

/// <summary>
/// Attach to a ghost GameObject. Call Init() immediately after AddComponent.
/// The ghost fades itself and self-destructs — no dependency on the player object.
/// </summary>
public class AfterimageGhost : MonoBehaviour
{
    private SpriteRenderer ghostRenderer;
    private float fadeDuration;
    private float elapsed;
    private Color startColor;

    /// <summary>
    /// Copies the source renderer's sprite and material, applies the tint color,
    /// and sets up sorting to sit just behind the player.
    /// </summary>
    public void Init(SpriteRenderer source, Color color, float duration)
    {
        ghostRenderer = gameObject.AddComponent<SpriteRenderer>();

        ghostRenderer.sprite = source.sprite;

        // sharedMaterial references the actual asset on disk — always stable
        // across play sessions. source.material creates an instance that can
        // go stale or null between runs.
        ghostRenderer.sharedMaterial = source.sharedMaterial;

        ghostRenderer.sortingLayerID = source.sortingLayerID;
        ghostRenderer.sortingOrder = source.sortingOrder - 1;

        ghostRenderer.color = color;
        startColor = color;
        fadeDuration = duration;
        elapsed = 0f;
    }

    private void Update()
    {
        if (ghostRenderer == null)
        {
            Destroy(gameObject);
            return;
        }

        elapsed += Time.deltaTime;

        float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / fadeDuration);
        ghostRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (elapsed >= fadeDuration)
            Destroy(gameObject);
    }
}