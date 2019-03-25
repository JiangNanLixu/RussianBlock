using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBlockIndex : MonoBehaviour {
    private TextMesh ui;

    private void Start() {
        ui = this.GetComponent<TextMesh>();
    }
    // Use this for initialization
    void Update () {
        ui.text =  "" + MyGridManager.TransformConvertToIndex(transform.parent);
	}
	
}
