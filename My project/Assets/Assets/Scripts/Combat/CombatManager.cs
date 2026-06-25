using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Card;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private int playerMaxHP = 30;
    [SerializeField] private int playerHP = 30;
    [SerializeField] private int playerBlock = 0;
    [SerializeField] private int playerDamage = 0;
    [SerializeField] private int playerDamageBonus = 0;
    [SerializeField] private int playerDamageMultiplier = 1;
    //[SerializeField] private int playerGold = 0;
    [SerializeField] private bool alreadySpun = false;
    [SerializeField] private bool cardPlayed = false;

    [Header("Pity Move")]
    [SerializeField] private int pityDamage = 2;
    [SerializeField] private int pityBlock = 2;

    private bool playerHealedThisTurn;

    [SerializeField] private TMP_Text playerHealthText;

    [SerializeField] private TMP_Text goldAddedText;
    [SerializeField] private TMP_Text currentGoldText;
    [SerializeField] private TMP_Text enemyHealthText;

    [SerializeField] private TMP_Text currentTurnText;

    [SerializeField] private TMP_Text playerHealthAddedText;
    [SerializeField] private TMP_Text playerHealthTakenText;
    [SerializeField] private TMP_Text playerShieldAddedText;
    [SerializeField] private TMP_Text playerBuffAddedText;

    [SerializeField] private TMP_Text enemyHealthAddedText;
    [SerializeField] private TMP_Text enemyHealthTakenText;
    [SerializeField] private TMP_Text enemyShieldAddedText;
    [SerializeField] private TMP_Text enemyBuffAddedText;

    [SerializeField] private string currentTurn;

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [SerializeField] private SpriteRenderer wheelRenderer;

    [SerializeField] private CardButton[] cards;

    [SerializeField] private DeckManager deckManager;

    [SerializeField] private Button drawCardButton;

    [SerializeField] private EnemyController enemyController;

    //[SerializeField] private PlayerManager playerManager;

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

        enemyController.Initialize(RunManager.Instance.selectedEnemy);
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

        StartCoroutine(EnemyTurnAfterDelay());
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

    private bool HasPlayableCards()
    {
        foreach (CardButton card in cards)
        {
            if (card != null && card.Data != null && card.Data.color == spunColor)
            {
                return true;
            }
        }

        return false;
    }

    void ResetPlayableCards()
    {
        deckManager.ClearSelection();

        foreach (CardButton card in cards)
        {
            card?.ShowNeutral();
        }
    }

    private void ShowIncomingPlayerDamage(int damage)
    {
        playerHealthTakenText.text = $"-{damage}";
    }

    private void ShowIncomingEnemyDamage(int damage)
    {
        enemyHealthTakenText.text = $"-{damage}";
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
                int baseDamage = card.effectValue + playerDamageBonus;
                int damage = baseDamage * playerDamageMultiplier;

                playerDamage = damage;

                ShowIncomingEnemyDamage(playerDamage); // damage
                StartCoroutine(DamageEnemyAfterDelay(playerDamage)); // damage

                playerDamageMultiplier = 1;
                playerBuffAddedText.text = playerDamageBonus > 0
                    ? $"+{playerDamageBonus}"
                    : "";

                break;

            case CardEffectType.Heal:
                int health = card.effectValue;
                playerHealedThisTurn = true;
                playerHealthAddedText.text = $"+{health}";
                StartCoroutine(AddHealthAfterDelay(health));
                break;

            case CardEffectType.Block:
                playerBlock += card.effectValue;
                playerShieldAddedText.text = $"+{playerBlock}";
                break;

            case CardEffectType.Buff:
                if (card.cardName.Equals("Yellow Double Attack"))
                {
                    playerDamageMultiplier = card.effectValue;
                    playerBuffAddedText.text = $"x{playerDamageMultiplier}";
                    break;
                }

                playerDamageBonus += card.effectValue;
                playerBuffAddedText.text = $"+{playerDamageBonus}";
                break;
        }

        UpdateHealthUI();
    }

    private IEnumerator EnemyTurnAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (currentState != CombatState.EnemyTurn || enemyController.isDead())
        {
            yield break;
        }

        EnemyAction action = enemyController.GetNextAction(
            playerHP,
            playerMaxHP,
            playerBlock,
            playerDamageBonus,
            playerHealedThisTurn);

        playerHealedThisTurn = false;

        if (action != null)
        {
            yield return ResolveEnemyAction(action);
        }

        UpdateHealthUI();

        if (currentState == CombatState.EnemyTurn)
        {
            StartPlayerTurn();
        }
    }

    private IEnumerator ResolveEnemyAction(EnemyAction action)
    {
        switch (action.intentType)
        {
            case EnemyAction.IntentType.Attack:
                yield return ResolveEnemyAttack(action.value);
                break;

            case EnemyAction.IntentType.Block:
                yield return ResolveEnemyBlock(action.value);
                break;

            case EnemyAction.IntentType.Heal:
                yield return ResolveEnemyHeal(action.value);
                break;

            case EnemyAction.IntentType.Buff:
                yield return ResolveEnemyBuff();
                break;

            case EnemyAction.IntentType.Debuff:
                yield return ResolveEnemyDebuff();
                break;

            default:
                Debug.LogWarning($"Unsupported enemy intent: {action.intentType}");
                break;
        }
    }

    private IEnumerator ResolveEnemyAttack(int baseDamage)
    {
        int totalDamage = baseDamage * enemyController.AttackMultiplier;
        int blockedDamage = Mathf.Min(playerBlock, totalDamage);
        int remainingDamage = totalDamage - blockedDamage;

        ShowIncomingPlayerDamage(totalDamage);

        yield return new WaitForSeconds(1f);

        // Any incoming attack consumes the player's entire block stack.
        playerBlock = 0;

        if (remainingDamage > 0)
        {
            DamagePlayer(remainingDamage);
        }

        playerHealthTakenText.text = "";
        playerShieldAddedText.text = "";
    }

    private IEnumerator ResolveEnemyBlock(int amount)
    {
        enemyShieldAddedText.text = $"+{amount}";
        yield return new WaitForSeconds(1f);

        enemyController.AddBlock(amount);
        enemyShieldAddedText.text = $"+{enemyController.Block}";
    }

    private IEnumerator ResolveEnemyHeal(int amount)
    {
        enemyHealthAddedText.text = $"+{amount}";
        yield return new WaitForSeconds(1f);

        enemyController.Heal(amount);
        enemyHealthAddedText.text = "";
    }

    private IEnumerator ResolveEnemyBuff()
    {
        enemyBuffAddedText.text = "x2";
        yield return new WaitForSeconds(1f);

        enemyController.DoubleAttackDamage();
        //enemyBuffAddedText.text = "";
    }

    private IEnumerator ResolveEnemyDebuff()
    {
        int previousBonus = playerDamageBonus;
        int reducedBonus = previousBonus / 2;
        int removedBonus = previousBonus - reducedBonus;

        playerBuffAddedText.text = $"-{removedBonus}";
        yield return new WaitForSeconds(1f);

        playerDamageBonus = reducedBonus;
        playerBuffAddedText.text = playerDamageBonus > 0
            ? $"+{playerDamageBonus}"
            : "";
    }

    private bool DamageEnemy(int damage)
    {
        enemyController.TakeDamage(damage);
        enemyShieldAddedText.text = "";
        UpdateHealthUI();

        if (enemyController.isDead())
        {
            WinCombat();
            RunManager.Instance.AddDefeated(enemyController.GetEnemy());
            return true;
        }

        return false;
    }

    private IEnumerator DamageEnemyAfterDelay(int damage)
    {
        yield return new WaitForSeconds(1f);
        DamageEnemy(damage);
        enemyHealthTakenText.text = "";
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

    private IEnumerator AddHealthAfterDelay(int health)
    {
        yield return new WaitForSeconds(1f);
        playerHP = Mathf.Min(playerMaxHP, playerHP + health);
        UpdateHealthUI();
        playerHealthAddedText.text = "";
    }

    private void PityResolve()
    {
        bool heads = Random.value < 0.5f;

        if (heads)
        {
            ShowIncomingEnemyDamage(pityDamage);
            StartCoroutine(DamageEnemyAfterDelay(pityDamage));
            return;
        }

        playerBlock += pityBlock;
        playerShieldAddedText.text = $"+{playerBlock}";
        UpdateHealthUI();
    }

    private void WinCombat()
    {
        currentState = CombatState.Won;
        RewardPlayer();
        ShowVictory();
    }

    private void RewardPlayer()
    {
        //playerGold += enemyController.GoldReward;
        goldAddedText.text = $"+{enemyController.GoldReward} gold";
        PlayerManager.Instance.AddGold(enemyController.GoldReward);
        currentGoldText.text = $"Current gold: {PlayerManager.Instance.Gold}";
    }

    private void LoseCombat()
    {
        currentState = CombatState.Lost;
        ShowDefeat();
    }

    public void EndPlayerTurn()
    {

        if (currentState != CombatState.PlayerTurnSpun)
        {
            return;
        }

        if (!cardPlayed && !HasPlayableCards())
        {
            PityResolve();
        }

        ResetPlayableCards();
        StartEnemyTurn();
        UpdateTurnUI();
        alreadySpun = false;
        cardPlayed = false;
        drawCardButton.interactable = false;
    }

    private void UpdateHealthUI()
    {
        playerHealthText.text = playerHP.ToString();
        enemyHealthText.text = enemyController.CurrentHP.ToString();
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

    IEnumerator LoadShopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Shop");
    }

    void ShowVictory()
    {
        victoryPanel.SetActive(true);
        StartCoroutine(LoadShopAfterDelay(3f));
    }

    void ShowDefeat()
    {
        defeatPanel.SetActive(true);
    }
}
