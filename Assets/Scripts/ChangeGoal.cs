using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeGoal : MonoBehaviour {

    public void GoalChangeText(int num) {
        this.GetComponent<Text>().text = "" + num;
    }
}
