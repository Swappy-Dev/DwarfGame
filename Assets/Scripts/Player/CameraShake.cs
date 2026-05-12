using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private float magnitude = 0.1f;
    private Vector3 originalPos;

    void OnEnable()
    {
        // Listen for the damage signal from PlayerData
        PlayerData.OnPlayerDamaged += TriggerShake;
    }

    void OnDisable()
    {
        // Stop listening when this object is disabled
        PlayerData.OnPlayerDamaged -= TriggerShake;
    }

    private void TriggerShake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Pick a random offset and move the camera
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap camera back to its start position
        transform.localPosition = originalPos;
    }
}