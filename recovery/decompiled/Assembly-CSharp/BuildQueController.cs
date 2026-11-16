using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildQueController : MonoBehaviour
{
	public class buildorder
	{
		public string buildname;

		public int buildnum;

		public int numberofshipsinthisorder;

		public float TimeTobuild;

		public float timesofar;

		public buildorder(string buildnamein, int buildnumin, int numberofshipsinthisorderinint, float TimeTobuildin)
		{
			buildname = buildnamein;
			buildnum = buildnumin;
			numberofshipsinthisorder = numberofshipsinthisorderinint;
			TimeTobuild = buildnumin;
			timesofar = 0f;
		}
	}

	public List<buildorder> ordersinnow = new List<buildorder>();

	public TextMesh[] textmehes = new TextMesh[4];

	public Image image;

	private PositinonRelativeToHeadset posrel;

	private void Start()
	{
		posrel = GetComponent<PositinonRelativeToHeadset>();
		TextMesh[] array = textmehes;
		foreach (TextMesh textMesh in array)
		{
		}
		InvokeRepeating("Rep", 0f, 0.02f);
	}

	private void Rep()
	{
		if (ordersinnow.Count <= 0)
		{
			return;
		}
		image.fillAmount = ordersinnow[0].timesofar / ordersinnow[0].TimeTobuild;
		ordersinnow[0].timesofar += Time.deltaTime;
		if (ordersinnow[0].timesofar > ordersinnow[0].TimeTobuild)
		{
			ordernow(ordersinnow[0].buildnum);
			ordersinnow[0].numberofshipsinthisorder--;
			ordersinnow[0].timesofar = 0f;
			if (ordersinnow[0].numberofshipsinthisorder == 0)
			{
				ordersinnow.Remove(ordersinnow[0]);
			}
		}
	}

	private void LateUpdate()
	{
		if (ordersinnow.Count > 0)
		{
			image.fillAmount = ordersinnow[0].timesofar / ordersinnow[0].TimeTobuild;
		}
		else
		{
			image.fillAmount = 0f;
		}
		if (ordersinnow.Count > 0)
		{
			textmehes[0].text = ordersinnow[0].buildname + " x" + ordersinnow[0].numberofshipsinthisorder;
		}
		else
		{
			textmehes[0].text = string.Empty;
		}
		if (ordersinnow.Count > 1)
		{
			textmehes[1].text = ordersinnow[1].buildname + " x" + ordersinnow[1].numberofshipsinthisorder;
		}
		else
		{
			textmehes[1].text = string.Empty;
		}
		if (ordersinnow.Count > 2)
		{
			textmehes[2].text = ordersinnow[2].buildname + " x" + ordersinnow[2].numberofshipsinthisorder;
		}
		else
		{
			textmehes[2].text = string.Empty;
		}
		if (ordersinnow.Count > 3)
		{
			textmehes[3].text = ordersinnow[3].buildname + " x" + ordersinnow[3].numberofshipsinthisorder;
		}
		else
		{
			textmehes[3].text = string.Empty;
		}
	}

	private void ordernow(int thingtoorder)
	{
		posrel.spawnshiprightnow(thingtoorder);
	}

	public void addbuildorder(buildorder input)
	{
		bool flag = false;
		foreach (buildorder item in ordersinnow)
		{
			if (input.buildnum == item.buildnum)
			{
				item.numberofshipsinthisorder++;
				flag = true;
			}
		}
		if (!flag)
		{
			ordersinnow.Add(input);
		}
	}
}
