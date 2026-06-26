using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public int Gold { get; private set; }

    public List<Card> playerDeck = new();

    [SerializeField] private List<RelicInstance> relics = new();

    public IReadOnlyList<RelicInstance> Relics => relics;

    // Fired when the relic list changes (HUD listens to this)
    public static event Action OnRelicsChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        Debug.Log($"Gold: {Gold}");
    }

    public bool CanAfford(int amount)
    {
        return Gold >= amount;
    }

    public bool SpendGold(int amount)
    {
        if (!CanAfford(amount))
            return false;

        Gold -= amount;
        return true;
    }

    public void AddCardToDeck(Card card)
    {
        playerDeck.Add(card);
    }

    public void AddRelic(Relic relic)
    {
        // Stack if the player already owns one and it can stack
        RelicInstance existing = relics.Find(r => r.data == relic);

        if (existing != null)
        {
            if (existing.CanStack)
            {
                existing.currentStacks++;
                Debug.Log($"[PlayerManager] {relic.relicName} stacked to {existing.currentStacks}/{relic.maxStacks}.");
            }
            else
            {
                Debug.Log($"[PlayerManager] {relic.relicName} already at max stacks.");
                return;
            }
        }
        else
        {
            relics.Add(new RelicInstance(relic));
        }

        OnRelicsChanged?.Invoke();
    }
