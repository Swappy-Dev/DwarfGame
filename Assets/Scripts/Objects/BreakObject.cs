using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class BreakObject : MonoBehaviour
{
    [Header("Sveikatos nustatymai")]
    public int hitsToBreak = 3;
    public bool breakOnStep = true;
    [Header("Sprite nustatymai")]
    [SerializeField] private Sprite brokenSprite;
    [Header("Drebėjimo nustatymai")]
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Collider2D objectCollider;
    private Light2D light2D;
    private Vector3 originalPosition;
    private int currentHits = 0;
    private bool isBroken = false;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<Collider2D>();
        originalPosition = transform.position;
        light2D = GetComponent<Light2D>();
        if (objectCollider != null && !objectCollider.isTrigger)
        {
            Debug.LogWarning(gameObject.name + " turi turėti 'Is Trigger' varnelę!");
        }
    }
    public void Break()
    {
        if (isBroken) return;
        currentHits++;
        if (currentHits >= hitsToBreak)
        {
            ExecuteBreak();
        }
        else
        {
            SoundManager.Instance.Play("PickaxeHit");
            StartCoroutine(Shake());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (breakOnStep && !isBroken && collision.CompareTag("Player"))
        {
            Debug.Log("Žaidėjas užlipo ant " + gameObject.name);
            ExecuteBreak();
        }
    }
    private void ExecuteBreak()
    {
        isBroken = true;
        StopAllCoroutines();
        transform.position = originalPosition;
        SoundManager.Instance.Play("PickaxeBreak");
        if (brokenSprite != null)
            spriteRenderer.sprite = brokenSprite;
        Collider2D[] allColliders = GetComponents<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            col.enabled = false;
        }
        OnBreak();
    }
    private IEnumerator Shake()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
    }
    protected virtual void OnBreak()
    {
        if (light2D != null)
        {
            light2D.enabled = false;
        }
    }
}