using UnityEngine;

public class ShipInfoController : MonoBehaviour
{
	public GameObject Weapons;

	public GameObject Equipment;

	public GameObject Specials;

	private TextMesh WeaponsText;

	private TextMesh EquipmentText;

	private TextMesh SpecialsText;

	public GameObject ShipTracker;

	public Transform CurrentShip;

	private MainMenuShipController shiptrackerscript;

	private void Start()
	{
		WeaponsText = Weapons.GetComponent<TextMesh>();
		EquipmentText = Equipment.GetComponent<TextMesh>();
		SpecialsText = Specials.GetComponent<TextMesh>();
		shiptrackerscript = ShipTracker.GetComponent<MainMenuShipController>();
		InvokeRepeating("rep", 0f, 1f);
	}

	private void rep()
	{
		CurrentShip = shiptrackerscript.ShipList[shiptrackerscript.countthrough].ship.transform;
		Transform transform = CurrentShip.Find("Weapons");
		string text = null;
		foreach (Transform item in transform)
		{
			WeaponBaseClass weaponBaseClass = null;
			if (item.childCount > 0)
			{
				weaponBaseClass = item.GetChild(0).GetComponent<WeaponBaseClass>();
			}
			if (item.childCount > 0 && (bool)weaponBaseClass && !weaponBaseClass.equipment && !weaponBaseClass.special)
			{
				text = text + "\n" + item.GetChild(0).name;
			}
		}
		WeaponsText.text = text;
		string text2 = null;
		foreach (Transform item2 in transform)
		{
			WeaponBaseClass weaponBaseClass2 = null;
			if (item2.childCount > 0)
			{
				weaponBaseClass2 = item2.GetChild(0).GetComponent<WeaponBaseClass>();
			}
			if (item2.childCount > 0 && (bool)weaponBaseClass2 && weaponBaseClass2.equipment)
			{
				text2 = text2 + "\n" + item2.GetChild(0).name;
			}
		}
		EquipmentText.text = text2;
		string text3 = null;
		foreach (Transform item3 in transform)
		{
			WeaponBaseClass weaponBaseClass3 = null;
			if (item3.childCount > 0)
			{
				weaponBaseClass3 = item3.GetChild(0).GetComponent<WeaponBaseClass>();
			}
			if (item3.childCount > 0 && (bool)weaponBaseClass3 && weaponBaseClass3.special)
			{
				text3 = text3 + "\n" + item3.GetChild(0).name;
			}
		}
		SpecialsText.text = text3;
	}
}
