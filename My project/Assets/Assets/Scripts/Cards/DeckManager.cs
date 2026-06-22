using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private List<Card> hand = new();
    [SerializeField] private List<Card> drawPile = new();
    [SerializeField] private List<Card> discardPile = new();

    [Header("Deck Setup")]
    public List<Card> startingDeck = new();

    [Header("Hand UI")]
    [SerializeField] private CardButton[] handSlots;

    public IReadOnlyList<Card> Hand => hand;

    private readonly HandSelectionManager selectionManager = new();

    void Awake()
    {
        foreach (CardButton slot in handSlots)
        {
            slot?.SetSelectionManager(selectionManager);
        }

        InitializeDeck();
    }

    void Start()
    {
        DrawOpeningHand();
    }

    public void InitializeDeck()
    {
        drawPile.Clear();
        hand.Clear();
        discardPile.Clear();

        drawPile.AddRange(startingDeck);
        Shuffle(drawPile);
    }

    public Card DrawCard()
    {
        if (drawPile.Count == 0)
        {
            ReshuffleDiscardPile();
        }

        // The draw and discard piles were both empty.
        if (drawPile.Count == 0)
        {
            return null;
        }

        int topIndex = drawPile.Count - 1;
        Card card = drawPile[topIndex];

        drawPile.RemoveAt(topIndex);
        hand.Add(card);

        return card;
    }

    public void DrawOneCard()
    {
        foreach (CardButton slot in handSlots)
        {
            // This UI slot already contains a card.
            if (slot.Data != null)
            {
                continue;
            }

            Card drawnCard = DrawCard();

            if (drawnCard == null)
            {
                Debug.Log("No cards are available to draw.");
                return;
            }

            slot.SetCard(drawnCard);
            return; // Only draw one card.
        }

        Debug.Log("The hand is full!");
    }

    public void DrawOpeningHand()
    {
        foreach (CardButton slot in handSlots)
        {
            Card drawnCard = DrawCard();

            if (drawnCard == null)
                break;

            slot.SetCard(drawnCard);
        }
    }

    public void DiscardFromHand(CardButton slot)
    {
        Card card = slot.Data;

        if (card == null)
            return;

        if (hand.Remove(card))
        {
            discardPile.Add(card);
            slot.ClearCard();
        }
    }

    public void ClearSelection()
    {
        selectionManager.ClearSelection();
    }

    private void ReshuffleDiscardPile()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
    }

    // The Fisher-Yates shuffling algorithm
    private void Shuffle(List<Card> cards)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            Card temporaryCard = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temporaryCard;
        }
    }
}
