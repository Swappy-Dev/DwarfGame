using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    [Header("Bangos nustatymai")]
    public float expandSpeed = 10f;  // Kaip greitai banga plečiasi
    public float lifetime = 0.6f;    // Kiek laiko banga egzistuoja prieš išnykstant
    public int damage = 1;           // Kiek gyvybių atims iš žaidėjo

    void Start()
    {
        // Automatiškai sunaikina šį objektą po nurodyto laiko
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Kiekvieną kadrą didiname objekto mastelį (Scale) į visas puses
        transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;
    }

    // Ši funkcija suveikia, kai banga paliečia kitą objektą
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Patikrinam, ar banga palietė objektą su Tagu "Player"
        if (collision.CompareTag("Player"))
        {
            // Pasiimame PlayerData komponentą iš žaidėjo objekto
            PlayerData playerHealth = collision.GetComponent<PlayerData>();

            if (playerHealth != null)
            {
                // Iškviečiame tavo funkciją ir padarome žalą!
                playerHealth.TakeDamage(damage);
                Debug.Log("Shockwave sėkmingai sužeidė žaidėją per PlayerData!");
            }
        }
    }
}