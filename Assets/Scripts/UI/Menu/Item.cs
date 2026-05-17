using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Inventoriaus nustatymai")]
    // Čia įsitempsi UI Prefabą (kuris turi RectTransform ir Image, o ne SpriteRenderer)
    public GameObject inventoryUIItemPrefab;

    public virtual void UseItem()
    {
        Debug.Log("using item");
    }
}