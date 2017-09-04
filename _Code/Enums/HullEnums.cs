using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hull types available
/// </summary>
public enum eHullType {
    /// <summary>
    /// Weapon slots 1, Special Slots 1, Base Armor 10, Base Shields 10, Base Speed 100, Base Maneuverability 2
    /// </summary>
    Light,
    /// <summary>
    /// Weapon slots 2, Special Slots 1, Base Armor 75, Base Shields 75, Base Speed 80, Base Maneuverability 5
    /// </summary>
    Corvette,
    /// <summary>
    /// Weapon slots 3, Special Slots 1, Base Armor 1,000, Base Shields 1,000, Base Speed 70, Base Maneuverability 10
    /// </summary>
    Frigate,
    /// <summary>
    /// Weapon slots 4, Special Slots 1, Base Armor 3,000, Base Shields 3,000, Base Speed 60, Base Maneuverability 20
    /// </summary>
    Cruiser,
    /// <summary>
    /// Weapon slots 4, Special Slots 2, Base Armor 10,000, Base Shields 10,000, Base Speed 50, Base Maneuverability 30
    /// </summary>
    Destroyer,
    /// <summary>
    /// Weapon slots 5, Special Slots 2, Base Armor 25,000, Base Shields 25,000, Base Speed 40, Base Maneuverability 40
    /// </summary>
    Capital,
    /// <summary>
    /// Weapon slots 5, Special Slots 3, Base Armor 100,000, Base Shields 100,000, Base Speed 30, Base Maneuverability 50
    /// </summary>
    SuperCapital
}

