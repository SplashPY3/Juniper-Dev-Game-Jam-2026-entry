using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<Card> allCards;
    [SerializeField] private List<ShopCardSlot> cardSlots;
    [SerializeField] private TMP_Text playerGoldText;

    private void Start()
    {
        GenerateInventory();
        UpdateGoldUI();
    }

    private void GenerateInventory()
    {
        List<Card> availableCards = new List<Card>(allCards);

        foreach (ShopCardSlot slot in cardSlots)
        {
            if (availableCards.Count == 0)
                return;

            int randomIndex = Random.Range(0, availableCards.Count);
            Card chosenCard = availableCards[randomIndex];

            slot.SetCard(chosenCard);

            availableCards.RemoveAt(randomIndex);
        }
    }

    public void RerollShop()
    {
        foreach (ShopCardSlot slot in cardSlots)
        {
            slot.ClearCard(slot.GetCard());
        }
        
        GenerateInventory();
    }

    public void UpdateGoldUI()
    {
        playerGoldText.text = $"Current gold: {PlayerManager.Instance.Gold}";
    }

    public void NextRound()
    {
        SceneManager.LoadScene("EnemySelection");
    }
}
