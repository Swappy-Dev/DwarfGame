using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    void Start()
    {
        // Sukuriame tuščius langelius, jei hierarchijoje jų dar nėra
        if (inventoryPanel.transform.childCount == 0)
        {
            for (int i = 0; i < slotCount; i++)
            {
                Instantiate(slotPrefab, inventoryPanel.transform);
            }
        }
    }

    public bool AddItem(GameObject pickedUpObject)
    {
        Item itemScript = pickedUpObject.GetComponent<Item>();

        if (itemScript == null || itemScript.inventoryUIItemPrefab == null)
        {
            Debug.LogError("Daiktas neturi Item skripto arba neįdėtas Inventory UI Item Prefab!");
            return false;
        }

        // SVARBU: Pridėjome 'true' parametrą, kad ieškotų ir neaktyviuose (paslėptuose) puslapiuose!
        Slot[] allSlots = inventoryPanel.GetComponentsInChildren<Slot>(true);

        if (allSlots.Length == 0)
        {
            Debug.LogError("InventoryPanel nerasta jokių Slot komponentų! Patikrink hierarchiją.");
            return false;
        }

        foreach (Slot slot in allSlots)
        {
            // Surandame pirmą visiškai tuščią langelį
            if (slot.currentItem == null)
            {
                // Sukuriame UI daiktą kaip to langelio vaiką
                GameObject newUIItem = Instantiate(itemScript.inventoryUIItemPrefab, slot.transform);

                RectTransform rect = newUIItem.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.zero;
                }

                // Prisegame skriptą daikto vilkimui, jei jo nėra ant prefabo
                if (newUIItem.GetComponent<ItemDragHandler>() == null)
                {
                    newUIItem.AddComponent<ItemDragHandler>();
                }

                slot.currentItem = newUIItem;
                Debug.Log("Daiktas sėkmingai įdėtas į langelį: " + slot.gameObject.name);
                return true;
            }
        }

        Debug.Log("Visi inventoriaus langeliai pilni!");
        return false;
    }
}