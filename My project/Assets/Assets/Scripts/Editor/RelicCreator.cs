#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class RelicCreator
{
    private const string SavePath = "Assets/Assets/Data/Relics/";
    private const string IconPath = "Assets/Assets/Art/Relics/";

    [MenuItem("Tools/Relic Creator/Create GDD Relics")]
    public static void CreateGDDRelics()
    {
        if (!AssetDatabase.IsValidFolder(SavePath.TrimEnd('/')))
            AssetDatabase.CreateFolder("Assets/Assets/Data", "Relics");

        // Loaded Wheel — one random wedge becomes Wild
        CreateRelic(
            fileName:    "LoadedWheel",
            relicName:   "Loaded Wheel",
            description: "One random wedge on your wheel becomes Wild.",
            effectType:  RelicEffectType.Wheel_AddWild,
            relicPrice:  80,
            iconFile:    "LoadedWheel.png"
        );

        // Lucky Horseshoe — Red wedges 20% more likely
        CreateRelic(
            fileName:         "LuckyHorseshoe",
            relicName:        "Lucky Horseshoe",
            description:      "Red wedges on your wheel are 20% more likely to be spun.",
            effectType:       RelicEffectType.Wheel_EnlargeColor,
            targetWedgeType:  WedgeType.Red,
            weightMultiplier: 1.2f,
            relicPrice:       60,
            iconFile:         "LuckyHorseshoe.png"
        );

        // Balanced Wheel — no repeat spins
        CreateRelic(
            fileName:    "BalancedWheel",
            relicName:   "Balanced Wheel",
            description: "The wheel cannot land on the same color twice in a row.",
            effectType:  RelicEffectType.Spin_PreventRepeat,
            relicPrice:  50,
            iconFile:    "BalancedWheel.png"
        );

        // Magnet — gain +1 Energy after spinning Blue
        CreateRelic(
            fileName:        "Magnet",
            relicName:       "Magnet",
            description:     "Whenever the wheel lands on Blue, gain 1 Energy.",
            effectType:      RelicEffectType.Spin_OnColor_GainEnergy,
            targetWedgeType: WedgeType.Blue,
            effectValue:     1,
            maxStacks:       3,
            relicPrice:      40,
            iconFile:        "Magnet.png"
        );

        // Rigged Bearing — every 5th spin is Gold
        CreateRelic(
            fileName:     "RiggedBearing",
            relicName:    "Rigged Bearing",
            description:  "Every 5th spin is guaranteed to land on Gold.",
            effectType:   RelicEffectType.Spin_GuaranteedEveryN,
            spinInterval: 5,
            relicPrice:   70,
            iconFile:     "RiggedBearing.png"
        );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[RelicCreator] GDD relics created in " + SavePath);
        EditorUtility.DisplayDialog("Relic Creator", "GDD relics created!\n" + SavePath, "OK");
    }

    private static void CreateRelic(
        string fileName,
        string relicName,
        string description,
        RelicEffectType effectType,
        WedgeType targetWedgeType  = WedgeType.Red,
        float weightMultiplier     = 1.2f,
        int effectValue            = 1,
        int spinInterval           = 5,
        int maxStacks              = 1,
        int relicPrice             = 50,
        string iconFile            = "")
    {
        string fullPath = SavePath + fileName + ".asset";

        if (AssetDatabase.LoadAssetAtPath<Relic>(fullPath) != null)
        {
            Debug.Log($"[RelicCreator] {fileName}.asset already exists, skipping.");
            return;
        }

        Relic relic            = ScriptableObject.CreateInstance<Relic>();
        relic.relicName        = relicName;
        relic.description      = description;
        relic.effectType       = effectType;
        relic.targetWedgeType  = targetWedgeType;
        relic.weightMultiplier = weightMultiplier;
        relic.effectValue      = effectValue;
        relic.spinInterval     = spinInterval;
        relic.maxStacks        = maxStacks;
        relic.relicPrice       = relicPrice;

        if (!string.IsNullOrEmpty(iconFile))
            relic.icon = AssetDatabase.LoadAssetAtPath<Sprite>(IconPath + iconFile);

        AssetDatabase.CreateAsset(relic, fullPath);
        Debug.Log($"[RelicCreator] Created {fileName}.asset");
    }
}
#endif
