using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance;
    [SerializeField] private DamagePopup popupPrefab;

    void Awake() => Instance = this;

    public void Show(int damage, Vector3 worldPosition, bool isCrit = false)
    {
        DamagePopup popup = Instantiate(popupPrefab, worldPosition, Quaternion.identity);
        popup.Setup(damage, isCrit);
    }
}