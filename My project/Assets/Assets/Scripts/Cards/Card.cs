using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public CardColor color;
    public int effectValue;
    public string description;
    public Sprite sprite;
    public CardEffectType effectType;
    public int cardPrice;

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

    public static Color GetDisplayColor(CardColor color)
    {
        switch (color)
        {
            case CardColor.Red:
                return Color.red;

            case CardColor.Green:
                return Color.green;

            case CardColor.Blue:
                return Color.blue;

            case CardColor.Yellow:
                return Color.yellow;

            default:
                return Color.white;
        }
    }
}
