using System.Collections;
using UnityEngine;

public class BreakObject : MonoBehaviour
{
    [Header("Sveikatos nustatymai")]
    public int hitsToBreak = 3;       // Kiek smūgių reikia atakuojant?
    public bool breakOnStep = true;   // Ar sulūžta užlipus (Trigger) iškart?

    [Header("Sprite nustatymai")]
    [SerializeField] private Sprite brokenSprite;

    [Header("Drebėjimo nustatymai")]
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Collider2D objectCollider;
    private Vector3 originalPosition;
    private int currentHits = 0;
    private bool isBroken = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<Collider2D>();
        originalPosition = transform.position;

        // Maža saugumo patikra: jei pamiršai uždėti Trigger
        if (objectCollider != null && !objectCollider.isTrigger)
        {
            Debug.LogWarning(gameObject.name + " turi turėti 'Is Trigger' varnelę!");
        }
    }

    // --- ATAKOS LOGIKA (Keli smūgiai) ---
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
            // Tik sudrebiname, nes dar liko gyvybių
            StartCoroutine(Shake());
        }
    }

    // --- UŽLIPIMO LOGIKA (Iškart) ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tikriname ar tai žaidėjas ir ar objektas dar nesulūžęs
        if (breakOnStep && !isBroken && collision.CompareTag("Player"))
        {
            Debug.Log("Žaidėjas užlipo ant " + gameObject.name);
            ExecuteBreak(); // Kviečiame tiesiogiai, kad ignoruotų hitsToBreak
        }
    }

    // --- SULAUŽYMAS ---
    private void ExecuteBreak()
    {
        isBroken = true;
        StopAllCoroutines();
        transform.position = originalPosition;

        if (brokenSprite != null)
            spriteRenderer.sprite = brokenSprite;

        // Svarbu: išjungiame collider, kad Trigger nebesuveiktų dar kartą
        Collider2D[] allColliders = GetComponents<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            col.enabled = false;
        }
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
}