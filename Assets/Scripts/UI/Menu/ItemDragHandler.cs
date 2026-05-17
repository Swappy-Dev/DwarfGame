using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Canvas mainCanvas;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            // Jei pamiršai uždėti, kodas uždeda pats, kad nebūtų klaidų
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Surandame pagrindinį Canvas, kad daiktas vilkimo metu būtų viršuje
        mainCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // Perkeliam į Canvas viršų, kad vilkimas matytųsi virš visų langelių
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

        // Tikriname, ką pelė užkabino atleidimo metu
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
            // Jei numetėme į tą patį slotą, iš kurio paėmėme
            if (dropSlot == originalSlot)
            {
                ReturnToOriginal();
                return;
            }

            // Jei naujas slotas jau turi kitą daiktą – sukeičiame vietomis
            if (dropSlot.currentItem != null)
            {
                GameObject swappedItem = dropSlot.currentItem;

                swappedItem.transform.SetParent(originalParent);
                swappedItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                originalSlot.currentItem = swappedItem;
            }
            else
            {
                // Jei naujas slotas buvo tuščias, senąjį išvalome
                originalSlot.currentItem = null;
            }

            // Įdedame daiktą į naują slotą
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            // Jei numetėme pro šalį – grąžiname
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