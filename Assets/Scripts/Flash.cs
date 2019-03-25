using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour {
    private Text point;
    private bool show = false;

	// Use this for initialization
	void Start () {
        point = GetComponent<Text>();

        StartCoroutine(flash());
	}

    IEnumerator flash() {
        yield return new WaitForSeconds(0.8f);
        show = !show;
        point.enabled = show;
        StartCoroutine(flash());
    }
}
