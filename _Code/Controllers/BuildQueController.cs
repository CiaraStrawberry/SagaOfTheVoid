using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildQueController : MonoBehaviour {
    public List<buildorder> ordersinnow = new List<buildorder>();
    public TextMesh[] textmehes = new TextMesh[4];
    public Image image;
    private PositinonRelativeToHeadset posrel;
	// Use this for initialization
	void Start () {
        posrel = GetComponent<PositinonRelativeToHeadset>();
        foreach(TextMesh tex in textmehes)
        {
       //     tex.GetComponent<MeshRenderer>().sortingLayerName = "From Shader";
        }

        InvokeRepeating("Rep",0 ,0.02f);
	}
    void Rep ()
    {
        if (ordersinnow.Count > 0)
        {
            image.fillAmount = (ordersinnow[0].timesofar / ordersinnow[0].TimeTobuild);
            ordersinnow[0].timesofar += Time.deltaTime;
            if (ordersinnow[0].timesofar > ordersinnow[0].TimeTobuild)
            {
                ordernow(ordersinnow[0].buildnum);
                ordersinnow[0].numberofshipsinthisorder--;
                ordersinnow[0].timesofar = 0;
                if (ordersinnow[0].numberofshipsinthisorder == 0) ordersinnow.Remove(ordersinnow[0]);
            }


        }
    
    }

    // Update is called once per frame
    void LateUpdate() {

        if (ordersinnow.Count > 0)
        {
            image.fillAmount = (ordersinnow[0].timesofar / ordersinnow[0].TimeTobuild);
        }
        else
        {
            image.fillAmount = 0;
        }
            if (ordersinnow.Count > 0) textmehes[0].text = ordersinnow[0].buildname + " x" + ordersinnow[0].numberofshipsinthisorder;
        else textmehes[0].text = "";
        if (ordersinnow.Count > 1) textmehes[1].text = ordersinnow[1].buildname + " x" + ordersinnow[1].numberofshipsinthisorder;
        else textmehes[1].text = "";
        if (ordersinnow.Count > 2) textmehes[2].text = ordersinnow[2].buildname + " x" + ordersinnow[2].numberofshipsinthisorder;
        else textmehes[2].text = "";
        if (ordersinnow.Count > 3) textmehes[3].text = ordersinnow[3].buildname + " x" + ordersinnow[3].numberofshipsinthisorder;
        else textmehes[3].text = "";

    }
    void ordernow (int thingtoorder)
    {
        posrel.spawnshiprightnow(thingtoorder);
    }
    public void addbuildorder (buildorder input)
    {
        bool a = false;
        foreach(buildorder bul in ordersinnow)
        {
            if (input.buildnum == bul.buildnum)
            {
                bul.numberofshipsinthisorder += 1;
                a = true;
            }
        }
        if (a == false) ordersinnow.Add(input);
    }

    public class buildorder {
        public string buildname;
        public int buildnum;
        public int numberofshipsinthisorder;
        public float TimeTobuild;
        public float timesofar;
        public buildorder (string buildnamein,int buildnumin, int numberofshipsinthisorderinint, float TimeTobuildin)
        {
            buildname = buildnamein;
            buildnum = buildnumin;
            numberofshipsinthisorder = numberofshipsinthisorderinint;
            TimeTobuild = buildnumin;
            timesofar = 0;
        }

    }

}
