using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public int Gold { get; private set; }

    [SerializeField] private List<Card> playerDeck = new();

    [SerializeField] List<Card> startingDeck = new();
    [SerializeField] private int startingGold = 10;

    public IReadOnlyList<Card> PlayerDeck => playerDeck;

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

    public void StartNewRun()
    {
        Gold = startingGold;

        playerDeck.Clear();
        playerDeck.AddRange(startingDeck);
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
        if (card == null)
            return;

        playerDeck.Add(card);
    }
}
