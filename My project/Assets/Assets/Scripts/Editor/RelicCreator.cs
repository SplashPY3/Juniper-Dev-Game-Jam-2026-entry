#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class RelicCreator
{
    private const string SavePath = "Assets/Assets/Data/Relics/";

    [MenuItem("Tools/Relic Creator/Create Sample Relics")]
    public static void CreateSampleRelics()
    {
        if (!AssetDatabase.IsValidFolder(SavePath.TrimEnd('/')))
            AssetDatabase.CreateFolder("Assets/Assets/Data", "Relics");

        CreateRelic("IronRing",       "Iron Ring",       "Deal +2 damage with every card played.",    RelicEffectType.Passive_BonusDamage,  effectValue: 2, maxStacks: 3);
        CreateRelic("LuckyCharm",     "Lucky Charm",     "When the wheel lands Red, heal 3 HP.",      RelicEffectType.Spin_OnColor,          effectValue: 3, maxStacks: 2, triggerColor: Card.CardColor.Red);
        CreateRelic("Amulet",         "Amulet",          "Heal 1 HP at the start of each turn.",      RelicEffectType.Combat_OnTurnStart,    effectValue: 1, maxStacks: 5);
        CreateRelic("VampireFang",    "Vampire Fang",    "Heal 2 HP whenever you deal damage.",        RelicEffectType.Combat_OnDamageDealt,  effectValue: 2, maxStacks: 2);
        CreateRelic("AdrenalGland",   "Adrenal Gland",   "When the wheel spins any color, heal 1 HP.", RelicEffectType.Spin_OnAnyColor,       effectValue: 1, maxStacks: 3);
        CreateRelic("PhoenixFeather", "Phoenix Feather", "Heal 5 HP when you kill an enemy.",          RelicEffectType.Combat_OnKill,         effectValue: 5, maxStacks: 2);
        CreateRelic("VitalCrystal",   "Vital Crystal",   "Gain +5 max HP.",                            RelicEffectType.Passive_BonusMaxHP,   effectValue: 5, maxStacks: 4);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[RelicCreator] Sample relics created in " + SavePath);
        EditorUtility.DisplayDialog("Relic Creator", "Sample relics created in:\n" + SavePath, "OK");
    }

    private static void CreateRelic(
        string fileName,
        string relicName,
        string description,
        RelicEffectType effectType,
        int effectValue,
        int maxStacks,
        Card.CardColor triggerColor = Card.CardColor.Red)
    {
        string fullPath = SavePath + fileName + ".asset";

        if (AssetDatabase.LoadAssetAtPath<Relic>(fullPath) != null)
        {
            Debug.Log($"[RelicCreator] {fileName}.asset already exists, skipping.");
            return;
        }

        Relic relic        = ScriptableObject.CreateInstance<Relic>();
        relic.relicName    = relicName;
        relic.description  = description;
        relic.effectType   = effectType;
        relic.triggerColor = triggerColor;
        relic.effectValue  = effectValue;
        relic.maxStacks    = maxStacks;

        AssetDatabase.CreateAsset(relic, fullPath);
    }
}
#endif
