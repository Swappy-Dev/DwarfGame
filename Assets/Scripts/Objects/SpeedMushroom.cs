using UnityEngine;
using System.Collections;

public class SpeedMushroom : Item
{
    public float duration = 30f; // Efekto trukmė sekundėmis
    public float multiplier = 1.5f; // 50% greičio padidėjimas

    public override void UseItem()
    {
        PlayerMovement movement = FindAnyObjectByType<PlayerMovement>();

        if (movement != null)
        {
            // Paleidžiame efektą ant paties žaidėjo objekto, kad ištrynus UI daiktą laikas nesustotų
            movement.StartCoroutine(SpeedBuffRoutine(movement));

            Debug.Log("Suvalgytas greičio grybas! Greitis padidintas 50% na " + duration + " sek.");

            // Paslepiame ir ištriname daiktą iš inventoriaus
            Destroy(gameObject);
        }
    }

    private IEnumerator SpeedBuffRoutine(PlayerMovement movement)
    {
        movement.speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        movement.speedMultiplier = 1f; // Grąžiname pradinį greitį
        Debug.Log("Greičio grybo efektas baigėsi.");
    }
}