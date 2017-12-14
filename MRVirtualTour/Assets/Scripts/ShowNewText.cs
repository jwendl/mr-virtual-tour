using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNewText : MonoBehaviour {

    public GameObject newtext;

	// Use this for initialization
	void Start () {
        Instantiate(newtext);
        newtext.transform.position = new Vector3(0, 0, 2);
        TextMesh tm = newtext.GetComponent<TextMesh>();
        tm.text = "This was set with code";
        tm.fontSize = 256;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
