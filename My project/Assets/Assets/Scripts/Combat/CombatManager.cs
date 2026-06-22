using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    [SerializeField] private TMP_Text currentTurnText;

    [SerializeField] private TMP_Text playerHealthAddedText;
    [SerializeField] private TMP_Text playerHealthTakenText;
    [SerializeField] private TMP_Text playerShieldAddedText;
    [SerializeField] private TMP_Text playerBuffAddedText;
    [SerializeField] private TMP_Text enemyHealthTakenText;

    [SerializeField] private string currentTurn;

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
        UpdateTurnUI();
    }

    void StartEnemyTurn()
    {
        currentState = CombatState.EnemyTurn;

        Invoke(nameof(EnemyAttack), 2f); // (TEMPORARY) attack after a delay to make the transitions between players smoother
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
                int damage = card.effectValue + playerDamageBonus;
                enemyHealthTakenText.text = $"-{damage}";
                StartCoroutine(DamageEnemyAfterDelay(damage));
                //DamageEnemy(card.effectValue + playerDamageBonus);
                break;

            case CardEffectType.Heal:
                int health = card.effectValue;
                playerHealthAddedText.text = $"+{health}";
                StartCoroutine(AddHealthAfterDelay(health));
                //playerHP += card.effectValue;
                break;

            case CardEffectType.Block:
                playerBlock += card.effectValue;
                playerShieldAddedText.text = $"+{playerBlock}";
                break;

            case CardEffectType.Buff:
                playerDamageBonus += card.effectValue;
                playerBuffAddedText.text = $"+{playerDamageBonus}";
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
            //DamagePlayer(remainingDamage);
            playerHealthTakenText.text = $"-{remainingDamage}";
            StartCoroutine(DamagePlayerAfterDelay(remainingDamage));
        }

        playerShieldAddedText.text = "";

        StartPlayerTurn();
        UpdateTurnUI();
    }

    private bool DamageEnemy(int damage)
    {
        //enemyHealthTakenText.text = $"-{damage}";
        //StartCoroutine(DamageEnemyAfterDelay(damage));

        enemyHP = Mathf.Max(0, enemyHP - damage);
        UpdateHealthUI();

        if (enemyHP > 0)
        {
            return false;
        }

        WinCombat();
        return true;
    }

    private IEnumerator DamageEnemyAfterDelay(int damage)
    {
        yield return new WaitForSeconds(1f);
        DamageEnemy(damage);
        enemyHealthTakenText.text = "";
    }

    private bool DamagePlayer(int damage)
    {
        //playerHealthTakenText.text = $"-{damage}";
        //StartCoroutine(DamagePlayerAfterDelay(damage));

        playerHP = Mathf.Max(0, playerHP - damage);
        UpdateHealthUI();

        if (playerHP > 0)
        {
            return false;
        }

        LoseCombat();
        return true;
    }

    private IEnumerator DamagePlayerAfterDelay(int damage)
    {
        yield return new WaitForSeconds(1f);
        DamagePlayer(damage);
        playerHealthTakenText.text = "";
    }

    private IEnumerator AddHealthAfterDelay(int health)
    {
        yield return new WaitForSeconds(1f);
        playerHP += health;
        UpdateHealthUI();
        playerHealthAddedText.text = "";
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
            UpdateTurnUI();
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

    void UpdateTurnUI()
    {
        if (currentState == CombatState.PlayerTurn)
        {
            currentTurn = "Player's turn";
        }

        else if (currentState == CombatState.EnemyTurn)
        {
            currentTurn = "Enemy's turn";
        }

        currentTurnText.text = currentTurn;
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
