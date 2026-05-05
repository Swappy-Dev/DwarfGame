using UnityEngine;

public class GoldNode : BreakObject
{
    [Header("Loot nustatymai")]
    public GameObject goldPrefab;
    public int goldDropMin = 1;
    public int goldDropMax = 3;
    public float dropSpread = 0.5f;

    protected override void OnBreak()
    {
        SpawnGold();
    }

    private void SpawnGold()
    {
        if (goldPrefab == null) return;

        int goldCount = Random.Range(goldDropMin, goldDropMax + 1);
        for (int i = 0; i < goldCount; i++)
        {
            Vector2 randomOffset = new Vector2(
                Random.Range(-dropSpread, dropSpread),
                Random.Range(-dropSpread, dropSpread)
            );
            Instantiate(goldPrefab, (Vector2)transform.position + randomOffset, Quaternion.identity);
        }
    }
}