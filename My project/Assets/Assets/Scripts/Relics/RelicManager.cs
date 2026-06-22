using System;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }

    [Header("Active Relics")]
    [SerializeField] private List<RelicInstance> activeRelics = new();

    public IReadOnlyList<RelicInstance> ActiveRelics => activeRelics;

    // Fired whenever the relic list changes so the UI can refresh
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
        CombatManager.OnSpin            += HandleSpin;
        CombatManager.OnDamageDealt     += HandleDamageDealt;
        CombatManager.OnDamageTaken     += HandleDamageTaken;
        CombatManager.OnEnemyKilled     += HandleEnemyKilled;
        CombatManager.OnPlayerTurnStart += HandleTurnStart;
    }

    private void OnDisable()
    {
        CombatManager.OnSpin            -= HandleSpin;
        CombatManager.OnDamageDealt     -= HandleDamageDealt;
        CombatManager.OnDamageTaken     -= HandleDamageTaken;
        CombatManager.OnEnemyKilled     -= HandleEnemyKilled;
        CombatManager.OnPlayerTurnStart -= HandleTurnStart;
    }

    public void AddRelic(Relic relic)
    {
        if (relic == null)
        {
            Debug.LogWarning("[RelicManager] AddRelic called with null relic.");
            return;
        }

        RelicInstance existing = activeRelics.Find(r => r.data == relic);

        if (existing != null)
        {
            if (existing.CanStack)
            {
                existing.currentStacks++;
                Debug.Log($"[RelicManager] {relic.relicName} stacked to {existing.currentStacks}/{relic.maxStacks}.");
                ApplyAcquisitionEffect(relic, 1);
            }
            else
            {
                Debug.Log($"[RelicManager] {relic.relicName} is already at max stacks ({relic.maxStacks}).");
                return;
            }
        }
        else
        {
            RelicInstance newInstance = new RelicInstance(relic);
            activeRelics.Add(newInstance);
            Debug.Log($"[RelicManager] Acquired relic: {relic.relicName}.");
            ApplyAcquisitionEffect(relic, 1);
        }

        OnRelicsChanged?.Invoke();
    }

    // Returns the total flat damage bonus from all Passive_BonusDamage relics
    public int GetBonusDamage()
    {
        int bonus = 0;

        foreach (RelicInstance r in activeRelics)
        {
            if (r.data.effectType == RelicEffectType.Passive_BonusDamage)
                bonus += r.ScaledValue;
        }

        return bonus;
    }

    private void ApplyAcquisitionEffect(Relic relic, int stacksGained)
    {
        if (relic.effectType == RelicEffectType.Passive_BonusMaxHP)
            CombatManager.Instance?.GainMaxHP(relic.effectValue * stacksGained);
    }

    private void HandleSpin(Card.CardColor color)
    {
        foreach (RelicInstance r in activeRelics)
        {
            if (r.data.effectType == RelicEffectType.Spin_OnAnyColor)
                ApplyTriggeredEffect(r);
            else if (r.data.effectType == RelicEffectType.Spin_OnColor && r.data.triggerColor == color)
                ApplyTriggeredEffect(r);
        }
    }

    private void HandleDamageDealt(int damage)
    {
        foreach (RelicInstance r in activeRelics)
        {
            if (r.data.effectType == RelicEffectType.Combat_OnDamageDealt)
                ApplyTriggeredEffect(r);
        }
    }

    private void HandleDamageTaken(int damage)
    {
        foreach (RelicInstance r in activeRelics)
        {
            if (r.data.effectType == RelicEffectType.Combat_OnDamageTaken)
                ApplyTriggeredEffect(r);
        }
    }

    private void HandleEnemyKilled()
    {
        foreach (RelicInstance r in activeRelics)
        {
            if (r.data.effectType == RelicEffectType.Combat_OnKill)
                ApplyTriggeredEffect(r);
        }
    }

    private void HandleTurnStart()
    {
        foreach (RelicInstance r in activeRelics)
        {
            if (r.data.effectType == RelicEffectType.Combat_OnTurnStart)
                ApplyTriggeredEffect(r);
        }
    }

    private void ApplyTriggeredEffect(RelicInstance instance)
    {
        if (CombatManager.Instance == null) return;

        CombatManager.Instance.HealPlayer(instance.ScaledValue);
        Debug.Log($"[RelicManager] {instance.data.relicName} triggered: +{instance.ScaledValue} HP.");
    }

#if UNITY_EDITOR
    [Header("Editor Testing")]
    [SerializeField] private Relic testRelic;

    [ContextMenu("Grant Test Relic")]
    private void GrantTestRelic()
    {
        AddRelic(testRelic);
    }

    [ContextMenu("Clear All Relics")]
    private void ClearAllRelics()
    {
        activeRelics.Clear();
        OnRelicsChanged?.Invoke();
        Debug.Log("[RelicManager] All relics cleared.");
    }
#endif
}
