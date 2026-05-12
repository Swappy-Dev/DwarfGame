using UnityEngine;
using System;

public class PlayerGold : MonoBehaviour
{
    public int currentGold = 0;
    public static event Action<int> OnGoldChanged;

    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
    }

    void Start()
    {
        OnGoldChanged?.Invoke(currentGold); // UI inicializacija
    }
}