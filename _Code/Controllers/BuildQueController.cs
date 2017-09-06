using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class attaches to the build menu display script and holds build orders whilst they are built.
/// </summary>
public class BuildQueController : MonoBehaviour {
    // a list of build order referances pending.
    public List<buildorder> ordersinnow = new List<buildorder>();
    // all textmeshes to display build orders on.
    public TextMesh[] textmehes = new TextMesh[4];
    // the bar to fill up relevant to the 
    public Image image;
    // the PositionRelativeToHeadset class referance.
    private PositinonRelativeToHeadset posrel;

	// Use this for initialization
	void Start () {
        posrel = GetComponent<PositinonRelativeToHeadset>();
        InvokeRepeating("Rep",0 ,0.02f);
	}

    /// <summary>
    /// The Repeating Function That Keeps Track of the Orders currently being built.
    /// </summary>
    void Rep ()
    {
        if (ordersinnow.Count > 0)
        {
            image.fillAmount = (ordersinnow[0].timesofar / ordersinnow[0].TimeTobuild);
            ordersinnow[0].timesofar += Time.deltaTime;
            if (ordersinnow[0].timesofar > ordersinnow[0].TimeTobuild)
            {
                posrel.spawnshiprightnow(ordersinnow[0].buildnum);
                ordersinnow[0].numberofshipsinthisorder--;
                ordersinnow[0].timesofar = 0;
                if (ordersinnow[0].numberofshipsinthisorder == 0) ordersinnow.Remove(ordersinnow[0]);
            }
        }
    
    }

    /// <summary>
    /// The Update Function used to update the Build order que. 
    /// </summary>
    void LateUpdate() {
        if (ordersinnow.Count > 0) image.fillAmount = (ordersinnow[0].timesofar / ordersinnow[0].TimeTobuild);
        else  image.fillAmount = 0;
        for (int i = 0; i < 4; i++)UpdateOrderTxt(i);
    }

    /// <summary>
    ///  Update the Text UI showing the next item in the buildQue.
    /// </summary>
    /// <param name="TextMeshnum"> Build que index</param>
    void UpdateOrderTxt  (int TextMeshnum)
    {
        if (ordersinnow.Count > TextMeshnum) textmehes[TextMeshnum].text = ordersinnow[TextMeshnum].buildname + " x" + ordersinnow[TextMeshnum].numberofshipsinthisorder;
        else textmehes[TextMeshnum].text = "";
    }

    /// <summary>
    /// Add a build order to the build order que
    /// </summary>
    /// <param name="input">the container class for the build order</param>
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


    /// <summary>
    /// The Class containing the relevant data for the buildorder.
    /// </summary>
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
