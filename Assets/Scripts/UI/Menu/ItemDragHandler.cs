using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // BŪTINA naujajai Input sistemai

// Pridėjome IPointerClickHandler paprastiems paspaudimams gaudyti
public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Canvas mainCanvas;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        mainCanvas = GetComponentInParent<Canvas>();
    }

    // --- NAUJA FUNKCIJA: Greitasis perkėlimas su Shift + Click ---
    public void OnPointerClick(PointerEventData eventData)
    {
        // Tikriname, ar paspaustas KAIRYSIS pelės klavišas
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Tikriname, ar šiuo metu laikomas nuspaustas bet kuris SHIFT klavišas
            if (Keyboard.current != null && Keyboard.current.shiftKey.isPressed)
            {
                TryFastTransfer();
            }
        }
    }

    private void TryFastTransfer()
    {
        Slot currentSlot = transform.parent.GetComponent<Slot>();
        if (currentSlot == null) return;

        // Surandame abu valdiklius žaidime
        InventoryController inventory = FindAnyObjectByType<InventoryController>();
        HotbarController hotbar = FindAnyObjectByType<HotbarController>();

        if (inventory == null || hotbar == null) return;

        // Tikriname, kur šiuo metu esame, ir ieškome tikslo panelės
        bool isInHotbar = currentSlot.transform.IsChildOf(hotbar.hotbarPanel.transform);
        Transform targetPanel = isInHotbar ? inventory.inventoryPanel.transform : hotbar.hotbarPanel.transform;

        // Surandame visus tikslo panelės langelius (ieškome ir neaktyvių)
        Slot[] targetSlots = targetPanel.GetComponentsInChildren<Slot>(true);

        foreach (Slot slot in targetSlots)
        {
            // Ieškome pirmo laisvo langelio tikslo panelėje
            if (slot.currentItem == null)
            {
                // Atjungiame daiktą iš seno langelio
                currentSlot.currentItem = null;

                // Prisegame prie naujo langelio
                transform.SetParent(slot.transform);
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = gameObject;

                Debug.Log($"Daiktas greituoju būdu perkeltas į: {slot.gameObject.name}");
                return; // Darbas baigtas, nutraukiame ciklą
            }
        }

        Debug.Log("Perkėlimas nepavyko: tikslo vietoje nėra laisvų langelių!");
    }

    // --- TAVO SENEJI DRAG / DROP METODAI (LIKO NEPAKEISTI) ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        if (mainCanvas != null)
            transform.SetParent(mainCanvas.transform);

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Slot dropSlot = null;

        if (eventData.pointerEnter != null)
        {
            dropSlot = eventData.pointerEnter.GetComponent<Slot>();
            if (dropSlot == null)
            {
                dropSlot = eventData.pointerEnter.GetComponentInParent<Slot>();
            }
        }

        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null)
        {
            if (dropSlot == originalSlot)
            {
                ReturnToOriginal();
                return;
            }

            if (dropSlot.currentItem != null)
            {
                GameObject swappedItem = dropSlot.currentItem;
                swappedItem.transform.SetParent(originalParent);
                swappedItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                originalSlot.currentItem = swappedItem;
            }
            else
            {
                originalSlot.currentItem = null;
            }

            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            ReturnToOriginal();
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void ReturnToOriginal()
    {
        transform.SetParent(originalParent);
        Slot originalSlot = originalParent.GetComponent<Slot>();
        if (originalSlot != null)
        {
            originalSlot.currentItem = gameObject;
        }
    }
}