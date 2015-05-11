using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	public float speed;

	private Animator animator;

	// Use this for initialization
	void Start () {
	
		animator = GetComponent<Animator>();
	
	}
	
	// Update is called once per frame
	public void InputMovement() {

		//modified conditions for built in key map. default keys for axes are wasd and arrows.

		animator.SetInteger("Direction", 0);


		if (Input.GetAxisRaw ("Horizontal") > 0 && Input.GetAxisRaw ("Vertical") > 0)
		{
		    animator.SetInteger("Direction", 9);
			transform.position += new Vector3 (speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}
		//if (Input.GetKey (KeyCode.A))
		/*4*/
		if (Input.GetAxisRaw ("Horizontal") > 0 && Input.GetAxisRaw ("Vertical") < 0)
		{
			animator.SetInteger("Direction", 2);
			transform.position -= new Vector3 (speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}
		//if (Input.GetKey (KeyCode.W))
		/*8*/	
		if (Input.GetAxisRaw ("Horizontal") < 0 && Input.GetAxisRaw ("Vertical") > 0)
		{
			animator.SetInteger("Direction", 7);
			transform.position += new Vector3 (-speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}
		//if (Input.GetKey (KeyCode.S))
		/*5*/
		if (Input.GetAxisRaw ("Horizontal") < 0 && Input.GetAxisRaw ("Vertical") < 0){
			animator.SetInteger("Direction", 1);
			transform.position -= new Vector3 (speed * Time.deltaTime, -speed * Time.deltaTime, 0.0f);
		}





		//if (Input.GetKey (KeyCode.D))
		/*6*/
		if (Input.GetAxisRaw ("Horizontal") > 0)
		{
		    animator.SetInteger("Direction", 6);
			transform.position += new Vector3 (speed * Time.deltaTime, 0.0f, 0.0f);
		}
		//if (Input.GetKey (KeyCode.A))
		/*4*/
		if (Input.GetAxisRaw ("Horizontal") < 0)
		{
			animator.SetInteger("Direction", 4);
			transform.position -= new Vector3 (speed * Time.deltaTime, 0.0f, 0.0f);
		}
		//if (Input.GetKey (KeyCode.W))
		/*8*/	
		if (Input.GetAxisRaw ("Vertical") > 0)
		{
			animator.SetInteger("Direction", 8);
			transform.position += new Vector3 (0.0f, speed * Time.deltaTime, 0.0f);
		}
		//if (Input.GetKey (KeyCode.S))
		/*5*/
		if (Input.GetAxisRaw ("Vertical") < 0){
			animator.SetInteger("Direction", 5);
			transform.position -= new Vector3 (0.0f, speed * Time.deltaTime, 0.0f);
		}

	}

}






