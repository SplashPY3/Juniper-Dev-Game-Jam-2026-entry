public enum RelicEffectType
{
    // Wheel modification (applied once on acquisition)
    Wheel_AddWild,           // Convert a random wedge to Wild (Loaded Wheel)
    Wheel_EnlargeColor,      // Multiply target color's weight (Lucky Horseshoe)
    Wheel_AddWedge,          // Add a new wedge of targetWedgeType

    // Spin sequence rules (set flags on WheelManager on acquisition)
    Spin_PreventRepeat,      // Cannot spin same color twice in a row (Balanced Wheel)
    Spin_GuaranteedEveryN,   // Every spinInterval spins is forced Gold (Rigged Bearing)

    // Spin trigger effects (fire when WheelManager.OnSpin matches condition)
    Spin_OnColor_GainEnergy, // After spinning targetWedgeType, gain effectValue energy (Magnet)
    Spin_OnGold_GainGold,    // After spinning Gold, gain gold (future use)

    // Combat triggers
    Combat_OnKill,
    Combat_OnTurnStart,
}
