using UnityEngine;
using System.Collections;

public class TK_Ani : MonoBehaviour {

private Animator animator;	
// Use this for initialization
void Start(){
 animator = this.GetComponent<Animator>();
}



// Update is called once per frame
void Update () {

//Idle Animation
/* 6 */	 if (Input.GetKeyDown (KeyCode.D))
			animator.SetInteger("Tidle", 6);
			
/* 4 */	 if (Input.GetKeyDown (KeyCode.A))
			animator.SetInteger("Tidle", 4);
			
/* 8 */	 if (Input.GetKeyDown (KeyCode.W))
			animator.SetInteger("Tidle", 8);
			
/* 5 */	 if (Input.GetKeyDown (KeyCode.S))			
			animator.SetInteger("Tidle", 5);
			
/*7*/	 if (Input.GetKeyDown (KeyCode.A) && Input.GetKey(KeyCode.W))			
			animator.SetInteger("Tidle", 7);
			
/*9*/    if (Input.GetKeyDown (KeyCode.D) && Input.GetKey(KeyCode.W))			
			animator.SetInteger("Tidle", 9);
			
/*1*/ 	 if (Input.GetKeyDown (KeyCode.A) && Input.GetKey(KeyCode.S))			
			animator.SetInteger("Tidle", 1);
			
/*3*/ 	 if (Input.GetKeyDown (KeyCode.D) && Input.GetKey(KeyCode.S))			
			animator.SetInteger("Tidle", 3);
		

		
		
//Walking Animation
/* 6 */	 if (Input.GetKey (KeyCode.D))
			animator.SetInteger("Twalk", 6);
			
/* 4 */	 if (Input.GetKey (KeyCode.A))
			animator.SetInteger("Twalk", 4);
			
/* 8 */	 if (Input.GetKey (KeyCode.W))
			animator.SetInteger("Twalk", 8);
			
/* 5 */	 if (Input.GetKey (KeyCode.S))			
			animator.SetInteger("Twalk", 5);
			
/*7*/	 if (Input.GetKey (KeyCode.A) && Input.GetKey(KeyCode.W))			
			animator.SetInteger("Twalk", 7);
			
/*9*/    if (Input.GetKey (KeyCode.D) && Input.GetKey(KeyCode.W))			
			animator.SetInteger("Twalk", 9);
			
/*1*/ 	 if (Input.GetKey (KeyCode.A) && Input.GetKey(KeyCode.S))			
			animator.SetInteger("Twalk", 1);
			
/*3*/ 	 if (Input.GetKey (KeyCode.D) && Input.GetKey(KeyCode.S))			
			animator.SetInteger("Twalk", 3);		
		


	}




		

		
		
	
}