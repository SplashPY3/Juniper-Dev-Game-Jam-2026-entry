using UnityEngine;

[CreateAssetMenu(fileName = "Relic", menuName = "Relics/Relic")]
public class Relic : ScriptableObject
{
    [Header("Identity")]
    public string relicName;
    public string description;
    public Sprite icon;
    public int relicPrice;

    [Header("Effect")]
    public RelicEffectType effectType;
    public WedgeType targetWedgeType;     // color/type this relic targets
    public float weightMultiplier = 1.2f; // for Wheel_EnlargeColor (1.2 = +20%)
    public int effectValue = 1;           // energy/gold amount for trigger relics
    public int spinInterval = 5;          // for Spin_GuaranteedEveryN

    [Header("Stacking")]
    public int maxStacks = 1;
}
