using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    private void OnEnable()
    {
        PlayerGold.OnGoldChanged += UpdateGoldText;
    }

    private void OnDisable()
    {
        PlayerGold.OnGoldChanged -= UpdateGoldText;
    }

    private void UpdateGoldText(int amount)
    {
        goldText.text = ": " + amount;
    }
}