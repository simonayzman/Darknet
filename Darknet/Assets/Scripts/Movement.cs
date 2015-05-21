// Darknet - Massively Multiplayer Online Role-Plaing Game (MMORPG) - CS Capstone 2015
// Simon Ayzman, Cammie Storey, Slavisa Djukic, Raymond Liang, Christian Diaz

//Christian Diaz
//Allows for movement across the world.
//Called iby GCtrller

using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	
	//Controls how fast a character will move
	public float speed;
	//Declares Animator component, for reference in movement animation
	private Animator animator;
	
	//Declares the GameObj's animator at start time	
	void Start () {
		animator = this.GetComponent<Animator>();
	}

	public void InputMovement() {
		
		//When not in motion, set the animator interger "Direction" to 0. 0 indcates an Idle state. Direction
		//is a varaible in the animator that is used to indicate which animation to play. 
		//The Direction is to all 8 possible directions, and can be easily depicted by looking at a simple numpad.
		//5 is the direction of the character facing south. 4 is used when the character is facing west. 
		//6 is used if the character is facing east. 8 is used if the character is facing north.
		//7 is used when the character is facing north-west. 9 is used when the character is facing north-east.
		//1 is used when the character is facing south-west. 3 is used when the charcther is facing south-east.
		//2 is not used.
		if(animator) animator.SetInteger("Direction", 0);

		//The diagonal input is placed before the cardinal directions in this code.
		//if the diagonal is placed under cardinal directions, the diagonal code will never be read.
		//Each if/else if statement checks for an axis. If the state is true, set the integer variable "Direction"
		//in the animator to the current number and transform the GammeObj in that direction. 	

			
		/*9*/
		if (Input.GetAxisRaw ("Horizontal") > 0 && Input.GetAxisRaw ("Vertical") > 0)
		{
		    if(animator) animator.SetInteger("Direction", 9);
			transform.position += new Vector3 (speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}
		/*3*/
		else if (Input.GetAxisRaw ("Horizontal") > 0 && Input.GetAxisRaw ("Vertical") < 0)
		{
			if(animator)animator.SetInteger("Direction", 3);
			transform.position -= new Vector3 (-speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}
		/*7*/	
		else if (Input.GetAxisRaw ("Horizontal") < 0 && Input.GetAxisRaw ("Vertical") > 0)
		{
			if(animator)animator.SetInteger("Direction", 7);
			transform.position += new Vector3 (-speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}
		/*1*/
		else if (Input.GetAxisRaw ("Horizontal") < 0 && Input.GetAxisRaw ("Vertical") < 0){
			if(animator)animator.SetInteger("Direction", 1);
			transform.position -= new Vector3 (speed * Time.deltaTime, speed * Time.deltaTime, 0.0f);
		}

		/*6*/
		else if (Input.GetAxisRaw ("Horizontal") > 0)
		{
		    if(animator)animator.SetInteger("Direction", 6);
			transform.position += new Vector3 (speed * Time.deltaTime, 0.0f, 0.0f);
		}
		/*4*/
		else if (Input.GetAxisRaw ("Horizontal") < 0)
		{
			if(animator)animator.SetInteger("Direction", 4);
			transform.position -= new Vector3 (speed * Time.deltaTime, 0.0f, 0.0f);
		}
		/*8*/	
		else if (Input.GetAxisRaw ("Vertical") > 0)
		{
			if(animator)animator.SetInteger("Direction", 8);
			transform.position += new Vector3 (0.0f, speed * Time.deltaTime, 0.0f);
		}
		/*5*/
		else if (Input.GetAxisRaw ("Vertical") < 0){
			if(animator)animator.SetInteger("Direction", 5);
			transform.position -= new Vector3 (0.0f, speed * Time.deltaTime, 0.0f);
		}
	}
}
