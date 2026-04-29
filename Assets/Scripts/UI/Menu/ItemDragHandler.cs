using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
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

        // 1. Randame Slot'ą, virš kurio atleidome pelę
        // Naudojame GetComponentInParent, nes pointerEnter gali pagauti ikoną, esančią slote
        Slot dropSlot = eventData.pointerEnter?.GetComponentInParent<Slot>();
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null)
        {
            // 2. Patikriname, ar dropSlot jau turi kitą daiktą
            if (dropSlot.currentItem != null && dropSlot.currentItem != gameObject)
            {
                GameObject swappedItem = dropSlot.currentItem;

                // Suvadiname seną daiktą į pradinį slot'ą
                swappedItem.transform.SetParent(originalParent);
                swappedItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                // Atnaujiname pradinio sloto informaciją
                originalSlot.currentItem = swappedItem;
            }
            else
            {
                // Jei slotas buvo tuščias, pradinį slotą paliekame tuščią
                originalSlot.currentItem = null;
            }

            // 3. Perkeliame tempiamą daiktą į naują slotą
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            // Jei numetėme "į lankas", grąžiname į pradžią
            transform.SetParent(originalParent);
        }

        // Visada nunuliname poziciją, kad daiktas atsistotų į centrą
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
