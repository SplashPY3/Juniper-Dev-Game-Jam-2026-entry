using UnityEngine;
[System.Serializable]
public class EnemyAction
{
    public IntentType intentType;
    public int value;
    public string description;

    public enum IntentType
    {
        Attack,
        Block,
        Heal,
        Buff,
        Debuff
    }
}
