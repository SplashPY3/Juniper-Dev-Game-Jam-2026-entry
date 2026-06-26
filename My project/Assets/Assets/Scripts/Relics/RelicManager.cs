using System;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }

    // Fired whenever the relic list changes so the HUD can refresh
    public static event Action OnRelicsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        WheelManager.OnSpin             += HandleSpin;
        CombatManager.OnEnemyKilled     += HandleEnemyKilled;
        CombatManager.OnPlayerTurnStart += HandleTurnStart;
    }

    private void OnDisable()
    {
        WheelManager.OnSpin             -= HandleSpin;
        CombatManager.OnEnemyKilled     -= HandleEnemyKilled;
        CombatManager.OnPlayerTurnStart -= HandleTurnStart;
    }

    // Reads relics from PlayerManager and applies their acquisition effects
    public void ApplyAllRelics()
    {
        if (PlayerManager.Instance == null || WheelManager.Instance == null) return;

        foreach (RelicInstance r in PlayerManager.Instance.Relics)
            ApplyAcquisitionEffect(r.data);
    }

    public void AddRelic(Relic relic)
    {
        if (relic == null)
        {
            Debug.LogWarning("[RelicManager] AddRelic called with null relic.");
            return;
        }

        PlayerManager.Instance.AddRelic(relic);
        ApplyAcquisitionEffect(relic);

        OnRelicsChanged?.Invoke();
        Debug.Log($"[RelicManager] Acquired: {relic.relicName}");
    }

    private void ApplyAcquisitionEffect(Relic relic)
    {
        if (WheelManager.Instance == null) return;

        switch (relic.effectType)
        {
            case RelicEffectType.Wheel_AddWild:
                WheelManager.Instance.MakeRandomWedgeWild();
                break;

            case RelicEffectType.Wheel_EnlargeColor:
                WheelManager.Instance.MultiplyColorWeight(relic.targetWedgeType, relic.weightMultiplier);
                break;

            case RelicEffectType.Wheel_AddWedge:
                WheelManager.Instance.AddWedge(relic.targetWedgeType);
                break;

            case RelicEffectType.Spin_PreventRepeat:
                WheelManager.Instance.PreventRepeat = true;
                break;

            case RelicEffectType.Spin_GuaranteedEveryN:
                WheelManager.Instance.GuaranteedGoldEveryN = relic.spinInterval;
                break;
        }
    }

    private void HandleSpin(WedgeType result)
    {
        if (PlayerManager.Instance == null) return;

        foreach (RelicInstance r in PlayerManager.Instance.Relics)
        {
            if (r.data.effectType == RelicEffectType.Spin_OnColor_GainEnergy
                && r.data.targetWedgeType == result)
            {
                CombatManager.Instance?.GainEnergy(r.ScaledValue);
                Debug.Log($"[RelicManager] {r.data.relicName}: +{r.ScaledValue} Energy.");
            }
            else if (r.data.effectType == RelicEffectType.Spin_OnGold_GainGold
                     && result == WedgeType.Gold)
            {
                // Gold reward — wire up when the shop/gold system supports it
                Debug.Log($"[RelicManager] {r.data.relicName}: Gold spin triggered.");
            }
        }
    }

    private void HandleEnemyKilled()
    {
        if (PlayerManager.Instance == null) return;

        foreach (RelicInstance r in PlayerManager.Instance.Relics)
        {
            if (r.data.effectType == RelicEffectType.Combat_OnKill)
                Debug.Log($"[RelicManager] {r.data.relicName}: triggered on kill.");
        }
    }

    private void HandleTurnStart()
    {
        if (PlayerManager.Instance == null) return;

        foreach (RelicInstance r in PlayerManager.Instance.Relics)
        {
            if (r.data.effectType == RelicEffectType.Combat_OnTurnStart)
                Debug.Log($"[RelicManager] {r.data.relicName}: triggered on turn start.");
        }
    }

#if UNITY_EDITOR
    [Header("Editor Testing")]
    [SerializeField] private Relic testRelic;

    [ContextMenu("Grant Test Relic")]
    private void GrantTestRelic()
    {
        AddRelic(testRelic);
    }
#endif
}
