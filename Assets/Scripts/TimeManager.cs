using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {

    public double startTime;
    public int hh;
    public int mm;
    public int ss;
    double temp;

    public Text timerShow;
    // Use this for initialization
    void Start() {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update() {
        temp = Time.time - startTime;
        hh = (int)temp / 3600;
        mm = ((int)temp % 3600) / 60;
        ss = (int)temp % 60;
        timerShow.text = "" + (hh < 10 ? "0" + hh : ""+ hh) + "  " + (mm < 10 ? "0" + mm : ""+mm) + "  " + (ss < 10 ? "0" + ss : "" + 
            ss);
	}
}
