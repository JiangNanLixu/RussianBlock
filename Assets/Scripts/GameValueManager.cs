using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameValueManager : MonoBehaviour {
    private int goal;
    public delegate void OnGoalChange(int num);
    public event OnGoalChange GoalEventHandler;
    public int Goal {
        set {
            goal = value;
            GoalEventHandler(goal);
        }
        get {
            return goal;
        }
    }

    public int grader;
    public int initRow;
    private void Awake() {
        GoalEventHandler += GameObject.FindObjectOfType<ChangeGoal>().GoalChangeText;
    }

    // Use this for initialization
    void Start () {
        Goal = 0;
        initRow = 0;
        grader = 0;
	}
	
}
