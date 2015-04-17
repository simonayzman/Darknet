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

 var vertical = Input.GetAxis("Vertical");
 var horizontal = Input.GetAxis("Horizontal");
 var prev = 0;	
//Walking Animation

/*7*/	 if (vertical > 0 && horizontal < 0){			
			animator.SetInteger("Twalk", 7);
			prev = 7;
			}
/*9*/    else if (vertical > 0 && horizontal > 0){			
			animator.SetInteger("Twalk", 9);
			prev = 9;
			}
/*1*/ 	 else if (vertical < 0 && horizontal < 0){			
			animator.SetInteger("Twalk", 1);
			prev = 1;
			}
/*3*/ 	 else if (vertical < 0 && horizontal > 0){			
			animator.SetInteger("Twalk", 3);
			prev = 3;
			}

/* 6 */	 else if (horizontal > 0){
			animator.SetInteger("Twalk", 6);
		    prev = 6;
		}
/* 4 */	 else if (horizontal < 0){
			animator.SetInteger("Twalk", 4);
			prev = 4;
			}
/* 8 */	 else if (vertical > 0){
			animator.SetInteger("Twalk", 8);
			prev = 8;
			}
/* 5 */	 else if (vertical > 0){			
			animator.SetInteger("Twalk", 5);
			prev = 5;
			}
		

 if( prev == 7){
     animator.SetInteger("Tidle", 7);
    }
 else if( prev == 9){
     animator.SetInteger("Tidle", 9);
    }
 else if( prev == 1){
     animator.SetInteger("Tidle", 1);
	}
 else if( prev == 3){
     animator.SetInteger("Tidle", 3);
	}
	
 else if( prev == 6){
     animator.SetInteger("Tidle", 6);
	}
 else if( prev == 4){
     animator.SetInteger("Tidle", 4);
	}
 else if( prev == 8){
     animator.SetInteger("Tidle", 8);
	}
 else if( prev == 5){
     animator.SetInteger("Tidle", 5);	
	}		

}




		

		
		
	
}