using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditScript : MonoBehaviour {

    public int levelPackID;
    public string fillOutText;
    Text displayText;

	// Use this for initialization
	void Start () {
        displayText = GetComponent<Text>();
        displayText.text += fillOutText + levelPackID;
	}
	
	// Update is called once per frame
	void Update () {

    }
}
