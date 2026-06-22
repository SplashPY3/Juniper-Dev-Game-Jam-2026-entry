using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Card;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private int playerHP = 30;
    [SerializeField] private int playerBlock = 5;
    [SerializeField] private int playerDamageBonus = 0;
    [SerializeField] private int enemyHP = 20;
    [SerializeField] private int enemyDamage = 5;
    [SerializeField] private bool alreadySpun = false;
    [SerializeField] private bool cardPlayed = false;

    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private TMP_Text enemyHealthText;

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [SerializeField] private SpriteRenderer wheelRenderer;

    [SerializeField] private CardButton[] cards;

    [SerializeField] private DeckManager deckManager;

    [SerializeField] private Button drawCardButton;

    private Card.CardColor spunColor;

    private enum CombatState
    {
        NotStarted,
        PlayerTurn,
        PlayerTurnSpun,
        EnemyTurn,
        Won,
        Lost
    }

    private CombatState currentState = CombatState.NotStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        victoryPanel.SetActive(false); // disable the UI before starting the game
        defeatPanel.SetActive(false);

        EnterCombat();
        UpdateHealthUI();
    }

    void EnterCombat()
    {
        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        currentState = CombatState.PlayerTurn;
        drawCardButton.interactable = false;
        ResetPlayableCards();
    }

    void StartEnemyTurn()
    {
        currentState = CombatState.EnemyTurn;

        Invoke(nameof(EnemyAttack), 1f); // (TEMPORARY) attack after a delay to make the transitions between players smoother
    }

    public void SpinWheel()
    {
        if (currentState != CombatState.PlayerTurn || alreadySpun.Equals(true))
        {
            return;
        }

        alreadySpun = true;

        currentState = CombatState.PlayerTurnSpun;

        spunColor = (Card.CardColor)Random.Range(0, 4);

        UpdatePlayableCards();

    }
    private void UpdatePlayableCards()
    {
        wheelRenderer.color = Card.GetDisplayColor(spunColor);

        foreach (CardButton card in cards)
        {
            bool playable = card != null && card.Data != null && card.Data.color == spunColor;
            card?.SetPlayable(playable);
        }
    }

    void ResetPlayableCards()
    {
        deckManager.ClearSelection();

        foreach (CardButton card in cards)
        {
            card?.ShowNeutral();
        }
    }

    public void TryDrawOneCard()
    {
        if (currentState != CombatState.PlayerTurnSpun)
            return;

        // Drawing requires a card to have been played first.
        if (!cardPlayed)
            return;

        deckManager.DrawOneCard();
        drawCardButton.interactable = false;
    }

    public bool PlayCard(CardButton card)
    {
        if (card == null || card.Data == null)
            return false;

        if (currentState != CombatState.PlayerTurnSpun)
            return false;

        if (cardPlayed)
            return false;

        if (card.Data.color != spunColor)
            return false;

        cardPlayed = true;

        ResolveCard(card.Data);

        deckManager.DiscardFromHand(card);

        deckManager.ClearSelection();

        drawCardButton.interactable =
        currentState == CombatState.PlayerTurnSpun;

        return true;
    }

    private void ResolveCard(Card card)
    {
        switch (card.effectType)
        {
            case CardEffectType.Damage:
                DamageEnemy(card.effectValue + playerDamageBonus);
                break;

            case CardEffectType.Heal:
                playerHP += card.effectValue;
                break;

            case CardEffectType.Block:
                playerBlock += card.effectValue;
                break;

            case CardEffectType.Buff:
                playerDamageBonus += card.effectValue;
                break;
        }

        UpdateHealthUI();
    }

    void EnemyAttack()
    {
        int blockedDamage = Mathf.Min(playerBlock, enemyDamage);
        playerBlock -= blockedDamage;

        int remainingDamage = enemyDamage - blockedDamage;

        if (remainingDamage > 0)
        {
            DamagePlayer(remainingDamage);
        }

        StartPlayerTurn();
    }

    private bool DamageEnemy(int damage)
    {
        enemyHP = Mathf.Max(0, enemyHP - damage);
        UpdateHealthUI();

        if (enemyHP > 0)
        {
            return false;
        }

        WinCombat();
        return true;
    }

    private bool DamagePlayer(int damage)
    {
        playerHP = Mathf.Max(0, playerHP - damage);
        UpdateHealthUI();

        if (playerHP > 0)
        {
            return false;
        }

        LoseCombat();
        return true;
    }

    private void WinCombat()
    {
        currentState = CombatState.Won;
        ShowVictory();
    }

    private void LoseCombat()
    {
        currentState = CombatState.Lost;
        ShowDefeat();
    }

    public void EndPlayerTurn()
    {
        if (currentState != CombatState.Won)
        {
            ResetPlayableCards();
            StartEnemyTurn();
            alreadySpun = false;
            cardPlayed = false;
            drawCardButton.interactable = false;
        }
    }

    private void UpdateHealthUI()
    {
        playerHealthText.text = playerHP.ToString();
        enemyHealthText.text = enemyHP.ToString();
    }

    void ShowVictory()
    {
        victoryPanel.SetActive(true);
    }

    void ShowDefeat()
    {
        defeatPanel.SetActive(true);
    }
}
