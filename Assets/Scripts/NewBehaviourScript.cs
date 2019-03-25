using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    List<int> group;
	// Use this for initialization
	void Start () {
        group = new List<int>();
        group.Add(1);
        group.Add(2);
        group.Add(3);

        Sda(group);
        print(group.Count);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Sda(List<int> temp) {
        temp.Clear();
    }
}
