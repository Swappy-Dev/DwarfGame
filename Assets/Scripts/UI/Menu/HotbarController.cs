using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 10;

    private Key[] hotbarKeys;

    private void Awake()
    {
        // Sugeneruojame klavišus nuo 1 iki 9, o paskutinis slotas bus 0
        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
        }
    }

    private void Start()
    {
        // SVARBU: Sukuriame tuščius hotbar langelius žaidimo pradžioje,
        // kad GetChild(index) funkcija vėliau rastų objektus!
        if (hotbarPanel.transform.childCount == 0)
        {
            for (int i = 0; i < slotCount; i++)
            {
                Instantiate(slotPrefab, hotbarPanel.transform);
            }
        }
    }

    void Update()
    {
        // Saugiklis: jei klaviatūra neprijungta, nieko nedarome
        if (Keyboard.current == null) return;

        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                UseItemInSlot(i);
            }
        }
    }

    void UseItemInSlot(int index)
    {
        // Saugiklis: tikriname, ar neviršijame turimų langelių skaičiaus hierarchijoje
        if (index >= hotbarPanel.transform.childCount) return;

        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();

        if (slot != null && slot.currentItem != null)
        {
            // Kadangi slot.currentItem yra UI objektas, ieškome Item komponento ant jo.
            // (Įsitikink, kad tavo UI Prefabas arba pasaulio daikto skriptas turi Item skriptą)
            Item item = slot.currentItem.GetComponent<Item>();

            if (item == null)
            {
                // Saugiklis: jei Item skriptas uždėtas ant kito vaikinio objekto
                item = slot.currentItem.GetComponentInChildren<Item>();
            }

            if (item != null)
            {
                item.UseItem();
            }
            else
            {
                Debug.LogWarning("Langelis turi daiktą, bet ant jo nerastas 'Item' skriptas su UseItem() funkcija!");
            }
        }
    }
}