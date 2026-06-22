public enum RelicEffectType
{
    // Passive effects (always active)
    Passive_BonusDamage,
    Passive_BonusMaxHP,
    Passive_BonusBlock,

    // Triggered when the wheel is spun
    Spin_OnColor,
    Spin_OnAnyColor,

    // Triggered during combat
    Combat_OnDamageDealt,
    Combat_OnDamageTaken,
    Combat_OnKill,
    Combat_OnTurnStart,
}
