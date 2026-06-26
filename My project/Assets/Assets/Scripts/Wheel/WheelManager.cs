using System;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    public static WheelManager Instance { get; private set; }

    [Header("Wheel State")]
    [SerializeField] private List<WheelWedge> wedges = new();

    public IReadOnlyList<WheelWedge> Wedges => wedges;

    public int SpinCount { get; private set; } = 0;
    public WedgeType LastSpin { get; private set; }
    private bool hasSpun = false;

    // Set by relics on acquisition
    public bool PreventRepeat { get; set; } = false;   // Balanced Wheel
    public int GuaranteedGoldEveryN { get; set; } = 0; // Rigged Bearing (0 = disabled)

    // Fired after every spin with the result
    public static event Action<WedgeType> OnSpin;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeDefaultWheel();
    }

    private void InitializeDefaultWheel()
    {
        wedges.Clear();
        wedges.Add(new WheelWedge(WedgeType.Red,    1f));
        wedges.Add(new WheelWedge(WedgeType.Green,  1f));
        wedges.Add(new WheelWedge(WedgeType.Blue,   1f));
        wedges.Add(new WheelWedge(WedgeType.Yellow, 1f));
    }

    public WedgeType Spin()
    {
        SpinCount++;

        // Rigged Bearing: force Gold on every Nth spin
        if (GuaranteedGoldEveryN > 0 && SpinCount % GuaranteedGoldEveryN == 0)
        {
            LastSpin = WedgeType.Gold;
            hasSpun  = true;
            OnSpin?.Invoke(WedgeType.Gold);
            return WedgeType.Gold;
        }

        WedgeType result = GetWeightedRandom();

        // Balanced Wheel: reroll once if same color came up last spin
        if (PreventRepeat && hasSpun && result == LastSpin && wedges.Count > 1)
            result = GetWeightedRandom();

        LastSpin = result;
        hasSpun  = true;
        OnSpin?.Invoke(result);
        return result;
    }

    private WedgeType GetWeightedRandom()
    {
        float total = 0f;
        foreach (WheelWedge w in wedges)
            total += w.weight;

        float roll       = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (WheelWedge w in wedges)
        {
            cumulative += w.weight;
            if (roll <= cumulative)
                return w.type;
        }

        return wedges[wedges.Count - 1].type;
    }

    // Relic: Loaded Wheel — convert one random wedge to Wild
    public void MakeRandomWedgeWild()
    {
        if (wedges.Count == 0) return;
        int idx = Random.Range(0, wedges.Count);
        Debug.Log($"[WheelManager] Wedge {idx} converted to Wild.");
        wedges[idx].type = WedgeType.Wild;
    }

    // Relic: Lucky Horseshoe — multiply all wedges of targetType by factor
    public void MultiplyColorWeight(WedgeType targetType, float factor)
    {
        foreach (WheelWedge w in wedges)
        {
            if (w.type == targetType)
                w.weight *= factor;
        }
        Debug.Log($"[WheelManager] {targetType} wedge weights x{factor}.");
    }

    // General: add a new wedge
    public void AddWedge(WedgeType type, float weight = 1f)
    {
        wedges.Add(new WheelWedge(type, weight));
        Debug.Log($"[WheelManager] Added {type} wedge (weight {weight}).");
    }
}
