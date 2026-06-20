using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public CardColor color;
    public int damage;
    public string description;
    public Sprite sprite;
    public CardEffectType effectType;

    public enum CardColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }

    public enum CardEffectType
    {
        Damage,
        Heal,
        Block,
        Buff
    }
}
