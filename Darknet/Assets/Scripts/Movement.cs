using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	public float speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//modified conditions for built in key map. default keys for axes are wasd and arrows.

		//if (Input.GetKey (KeyCode.D))
		if (Input.GetAxisRaw ("Horizontal") > 0)
			transform.position += new Vector3 (speed * Time.deltaTime, 0.0f, 0.0f);
		//if (Input.GetKey (KeyCode.A))
		if (Input.GetAxisRaw ("Horizontal") < 0)
			transform.position -= new Vector3 (speed * Time.deltaTime, 0.0f, 0.0f);
		//if (Input.GetKey (KeyCode.W))
		if (Input.GetAxisRaw ("Vertical") > 0)
			transform.position += new Vector3 (0.0f, speed * Time.deltaTime, 0.0f);
		//if (Input.GetKey (KeyCode.S))
		if (Input.GetAxisRaw ("Vertical") < 0)
			transform.position -= new Vector3 (0.0f, speed * Time.deltaTime, 0.0f);
	

	}
}
