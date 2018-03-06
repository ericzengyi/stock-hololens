using UnityEngine;
using System.Collections;

public class BallMover : MonoBehaviour {

	public CharacterController controller;
	// Use this for initialization1
	void Start () {
		print ("BallMover starting");
		controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		float move3rdAxis = Input.GetAxis("Mouse ScrollWheel");
		Vector3 movement = new Vector3 (moveHorizontal, 10 * move3rdAxis, moveVertical);
		controller.Move( movement );
	}
}
