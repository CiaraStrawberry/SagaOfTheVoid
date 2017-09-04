using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInfoController : MonoBehaviour {
    public GameObject Weapons;
    public GameObject Equipment;
    public GameObject Specials;
    private TextMesh WeaponsText;
    private TextMesh EquipmentText;
    private TextMesh SpecialsText;
    public GameObject ShipTracker;
    public Transform CurrentShip;
    private MainMenuShipController shiptrackerscript;
	// Use this for initialization
	void Start () {
        WeaponsText = Weapons.GetComponent<TextMesh>();
        EquipmentText = Equipment.GetComponent<TextMesh>();
        SpecialsText = Specials.GetComponent<TextMesh>();
        shiptrackerscript = ShipTracker.GetComponent<MainMenuShipController>();
        InvokeRepeating("rep", 0, 1);
     }

    // Update is called once per frame
    void rep() {
        CurrentShip = shiptrackerscript.ShipList[shiptrackerscript.countthrough].ship.transform;
        Transform WeaponHolder = CurrentShip.Find("Weapons");
        string weaponsstring = null;
       
        foreach (Transform t in WeaponHolder) {
            WeaponBaseClass weapon = null;
            if (t.childCount > 0) weapon = t.GetChild(0).GetComponent<WeaponBaseClass>();
            if (t.childCount > 0 && weapon && weapon.equipment == false && weapon.special == false)
                weaponsstring = weaponsstring + "\n" + t.GetChild(0).name;
        }
        WeaponsText.text = weaponsstring;
        string equipmentstring = null;
        foreach (Transform t in WeaponHolder)
        {
            WeaponBaseClass weapon = null;
            if (t.childCount > 0) weapon = t.GetChild(0).GetComponent<WeaponBaseClass>();
            if (t.childCount > 0 && weapon && weapon.equipment == true)
                equipmentstring = equipmentstring + "\n" + t.GetChild(0).name;
        }
        EquipmentText.text = equipmentstring;
        string specialstring = null;
        foreach (Transform t in WeaponHolder)
        {
            WeaponBaseClass weapon = null;
            if (t.childCount > 0) weapon = t.GetChild(0).GetComponent<WeaponBaseClass>();
            if (t.childCount > 0 && weapon && weapon.special == true)
                specialstring = specialstring + "\n" + t.GetChild(0).name;
        }



        SpecialsText.text = specialstring;
    }
}
