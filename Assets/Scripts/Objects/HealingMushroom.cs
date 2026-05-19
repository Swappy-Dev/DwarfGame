using UnityEngine;

public class HealingMushroom : Item
{
    public int healAmount = 2;

    public override void UseItem()
    {
        PlayerData playerData = FindAnyObjectByType<PlayerData>();

        if (playerData != null)
        {
            if (playerData.currentHealth >= playerData.maxHealth)
            {
                Debug.Log("Gyvybės jau pilnos!");
                return;
            }

            // Tiesiog iškviečiame naująją funkciją!
            playerData.Heal(healAmount);

            // Sunaikiname grybą iš hotbar
            Destroy(gameObject);
        }
    }
}