using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	public float speed;
	private Animator animator;

	//Declares the animator at start time	
	void Start () {
		animator = this.GetComponent<Animator>();
	}
	

	public void InputMovement() {
	
	animator.SetInteger("Direction", 0);
		
		/*9*/
		if (Input.GetAxisRaw ("Horizontal") > 0 && Input.GetAxisRaw ("Vertical") > 0)
		{
		    animator.SetInteger("Direction", 9);
			transform.position += new Vector3 (speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}
		/*3*/
		else if (Input.GetAxisRaw ("Horizontal") > 0 && Input.GetAxisRaw ("Vertical") < 0)
		{
			animator.SetInteger("Direction", 3);
			transform.position -= new Vector3 (-speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}
		/*7*/	
		else if (Input.GetAxisRaw ("Horizontal") < 0 && Input.GetAxisRaw ("Vertical") > 0)
		{
			animator.SetInteger("Direction", 7);
			transform.position += new Vector3 (-speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}
		/*1*/
		else if (Input.GetAxisRaw ("Horizontal") < 0 && Input.GetAxisRaw ("Vertical") < 0){
			animator.SetInteger("Direction", 1);
			transform.position -= new Vector3 (speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}



		/*6*/
		else if (Input.GetAxisRaw ("Horizontal") > 0)
		{
		    animator.SetInteger("Direction", 6);
			transform.position += new Vector3 (speed * Time.deltaTime, 0.0f, 0.0f);
		}
		/*4*/
		else if (Input.GetAxisRaw ("Horizontal") < 0)
		{
			animator.SetInteger("Direction", 4);
			transform.position -= new Vector3 (speed * Time.deltaTime, 0.0f, 0.0f);
		}
		/*8*/	
		else if (Input.GetAxisRaw ("Vertical") > 0)
		{
			animator.SetInteger("Direction", 8);
			transform.position += new Vector3 (0.0f, speed * Time.deltaTime, 0.0f);
		}
		/*5*/
		else if (Input.GetAxisRaw ("Vertical") < 0){
			animator.SetInteger("Direction", 5);
			transform.position -= new Vector3 (0.0f, speed * Time.deltaTime, 0.0f);
		}
	
	
	
	}


}
