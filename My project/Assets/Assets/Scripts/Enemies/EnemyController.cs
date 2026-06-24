using UnityEngine;
using static EnemyAction;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Enemy enemyData;

    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }
    public int Block { get; private set; }
    public int AttackMultiplier { get; private set; }
    public int GoldReward { get; private set; }

    private int actionIndex = 0;

    private void Awake()
    {
        Initialize(enemyData);
    }

    private void Initialize(Enemy data)
    {
        enemyData = data;

        MaxHP = enemyData.maxHP;
        CurrentHP = MaxHP;
        GoldReward = enemyData.goldReward;
        Block = 0;
        AttackMultiplier = 1;
        actionIndex = 0;
    }
    public EnemyAction GetNextAction(
        int playerHP,
        int playerMaxHP,
        int playerBlock,
        int playerDamageBonus,
        bool playerHealedThisTurn)
    {
        if (enemyData.Actions == null || enemyData.Actions.Count == 0)
        {
            Debug.LogWarning($"{enemyData.enemyName} has no configured actions.");
            return null;
        }

        if (enemyData.actionSelectionMode == Enemy.ActionSelectionMode.Sequential)
            return GetSequentialAction();

        if (enemyData.actionSelectionMode == Enemy.ActionSelectionMode.Random)
            return GetRandomAction();

        return GetReactiveAction(
            playerHP,
            playerMaxHP,
            playerBlock,
            playerDamageBonus,
            playerHealedThisTurn);
    }
    // new
    private EnemyAction GetReactiveAction(
        int playerHP,
        int playerMaxHP,
        int playerBlock,
        int playerDamageBonus,
        bool playerHealedThisTurn)
    {
        float playerHPPercent = playerMaxHP > 0
            ? (float)playerHP / playerMaxHP
            : 0f;

        // A vulnerable player should always be pressured with an attack.
        if (playerHPPercent < 0.35f)
        {
            EnemyAction attack = GetRandomActionOfType(IntentType.Attack);

            if (attack == null)
                Debug.LogWarning($"{enemyData.enemyName} needs an Attack action for its low-health reaction.");

            return attack;
        }

        // React to healing by either doubling future attack damage or halving the player's buff.
        if (playerHealedThisTurn)
        {
            EnemyAction healReaction = ChooseRandomAction(IntentType.Buff, IntentType.Debuff);
            if (healReaction != null)
                return healReaction;
        }

        // Counter an active player buff with block or a debuff.
        if (playerDamageBonus > 0)
        {
            EnemyAction buffReaction = ChooseRandomAction(IntentType.Block, IntentType.Debuff);
            if (buffReaction != null)
                return buffReaction;
        }

        float enemyHPPercent = MaxHP > 0
            ? (float)CurrentHP / MaxHP
            : 0f;

        // Low health? Try to heal if possible.
        if (enemyHPPercent <= 0.35f)
        {
            EnemyAction heal = FindAction(IntentType.Heal);
            if (heal != null)
                return heal;
        }

        // Player has block? Buff instead of wasting attack.
        if (playerBlock > 0)
        {
            EnemyAction defensiveAction = ChooseRandomAction(IntentType.Buff, IntentType.Block);
            if (defensiveAction != null)
                return defensiveAction;
        }

        // Otherwise, pick randomly from available actions.
        return GetRandomAction();
    }
    // new
    private EnemyAction GetSequentialAction()
    {
        EnemyAction action = enemyData.Actions[actionIndex];

        actionIndex++;

        if (actionIndex >= enemyData.Actions.Count)
            actionIndex = 0;

        return action;
    }
    // new
    private EnemyAction GetRandomAction()
    {
        if (enemyData.Actions == null || enemyData.Actions.Count == 0)
            return null;

        return enemyData.Actions[Random.Range(0, enemyData.Actions.Count)];
    }
    // new
    private EnemyAction FindAction(IntentType type)
    {
        foreach (EnemyAction action in enemyData.Actions)
        {
            if (action.intentType == type)
                return action;
        }

        return null;
    }

    private EnemyAction ChooseRandomAction(IntentType firstType, IntentType secondType)
    {
        EnemyAction firstAction = FindAction(firstType);
        EnemyAction secondAction = FindAction(secondType);

        if (firstAction == null)
            return secondAction;

        if (secondAction == null)
            return firstAction;

        return Random.Range(0, 2) == 0 ? firstAction : secondAction;
    }

    private EnemyAction GetRandomActionOfType(IntentType type)
    {
        int matchCount = 0;

        foreach (EnemyAction action in enemyData.Actions)
        {
            if (action.intentType == type)
                matchCount++;
        }

        if (matchCount == 0)
            return null;

        int selectedMatch = Random.Range(0, matchCount);

        foreach (EnemyAction action in enemyData.Actions)
        {
            if (action.intentType != type)
                continue;

            if (selectedMatch == 0)
                return action;

            selectedMatch--;
        }

        return null;
    }

    public void TakeDamage(int amount)
    {
        amount = Mathf.Max(0, amount);

        int blockedDamage = Mathf.Min(Block, amount);
        int remainingDamage = amount - blockedDamage;

        // Any incoming attack consumes the enemy's entire block stack.
        Block = 0;

        CurrentHP -= remainingDamage;

        if (CurrentHP < 0)
            CurrentHP = 0;

        Debug.Log($"Enemy HP: {CurrentHP}/{MaxHP}");
    }

    public void AddBlock(int amount)
    {
        Block += Mathf.Max(0, amount);
    }

    public void ClearBlock()
    {
        Block = 0;
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + Mathf.Max(0, amount));
    }

    public void DoubleAttackDamage()
    {
        AttackMultiplier = 2;
    }

    public bool isDead()
    {
        return CurrentHP <= 0;
    }
}
