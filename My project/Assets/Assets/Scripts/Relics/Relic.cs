using UnityEngine;

[CreateAssetMenu(fileName = "Relic", menuName = "Relics/Relic")]
public class Relic : ScriptableObject
{
    [Header("Identity")]
    public string relicName;
    public string description;
    public Sprite icon;

    [Header("Effect")]
    public RelicEffectType effectType;
    public Card.CardColor triggerColor; // Only used for Spin_OnColor
    public int effectValue = 1;

    [Header("Stacking")]
    public int maxStacks = 1; // Set to 1 to disable stacking
}
