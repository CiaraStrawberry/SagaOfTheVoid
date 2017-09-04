using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class types for hulls
/// </summary>
public enum eShipClass {
    /// <summary>
    /// +30% Shield & Armor, -10% DPS, -10% Speed
    /// </summary>
    Tank,
    /// <summary>
    /// -10% Shield & Armor, +30% DPS
    /// </summary>
    Offense,
    /// <summary>
    /// -10% Shield & Armor, +1 Weapon Slot
    /// </summary>
    Defense,
    /// <summary>
    /// -10% Damage, +30% Speed, -1 Weapon Slot
    /// </summary>
    Support
}
