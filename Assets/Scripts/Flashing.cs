using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashing : MonoBehaviour {

    // Use this for initialization
    void Start() {
        StartCoroutine(Flash());
    }

    IEnumerator Flash() {
        transform.localScale = transform.localScale == Vector3.zero ? new Vector3(1, 1, 1) : Vector3.zero;
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(Flash());
    }
}
