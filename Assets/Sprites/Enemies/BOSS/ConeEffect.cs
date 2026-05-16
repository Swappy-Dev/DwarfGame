using UnityEngine;

public class ConeEffect : MonoBehaviour
{
    [Header("Kūgio nustatymai")]
    public float expandSpeed = 15f;      // Kaip greitai kūgis šauna į priekį (X ašis)
    public float targetWidth = 5f;       // MAKSIMALUS PLOTIS (Y ašis) – padidink šį skaičių, kad būtų dar platesnis!
    public float lifetime = 0.4f;        // Atakos trukmė
    public int damage = 1;

    private float timer = 0f;

    void Start()
    {
        // Pradedam nuo visiško nulio
        transform.localScale = new Vector3(0f, 0f, 1f);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / lifetime; // Skaičiuojam progresą nuo 0 iki 1

        // X ašis plečiasi tiesiškai į priekį
        float newX = transform.localScale.x + expandSpeed * Time.deltaTime;

        // Y ašis (plotis) plečiasi proporcingai pagal nustatytą targetWidth
        float newY = Mathf.Lerp(0f, targetWidth, progress);

        transform.localScale = new Vector3(newX, newY, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData playerHealth = collision.GetComponent<PlayerData>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}