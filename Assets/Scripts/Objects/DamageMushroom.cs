using UnityEngine;
using System.Collections;

public class DamageMushroom : Item
{
    [Header("Jėgos grybo nustatymai")]
    public float damageMultiplierIncrease = 2f; // 2f reiškia 2 kartus daugiau žalos (sudvigubina)
    public float duration = 10f;

    

    public override void UseItem()
    {
        PlayerAttack attack = FindAnyObjectByType<PlayerAttack>();

        if (attack != null)
        {
            // Paleidžiame efektą ant žaidėjo objekto
            attack.StartCoroutine(DamageBuffRoutine(attack));

            Debug.Log("Suvalgytas jėgos grybas! Žala padidinta 2 kartus " + duration + " sek.!");

            // Sunaikiname UI elementą
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageBuffRoutine(PlayerAttack attack)
    {
        // Sudauginame esamą daugiklį (jei suvalgytum kelis grybus, jie daugintųsi teisingai)
        attack.damageMultiplier = damageMultiplierIncrease;

        yield return new WaitForSeconds(duration);

        // Pasibaigus laikui, padaliname atgal iš to paties skaičiaus, kad sugrįžtų pradinis galingumas
        attack.damageMultiplier = 1f;

        Debug.Log("Jėgos grybo efektas baigėsi. Žala grįžo į normą.");
    }
}