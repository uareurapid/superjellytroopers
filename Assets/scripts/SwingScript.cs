using UnityEngine;
using System.Collections;

public class SwingScript : MonoBehaviour {


  public float swingStartDelay = 2.0f;
	public float swingDirection = 1.0f; //right, -1.0 left
	
	private bool swingStarted = false;
	
	/*public float speed = 2.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    Quaternion localRotation = transform.localRotation;
		localRotation.z = Mathf.Sin(Time.time * speed) * 0.2f;
		transform.localRotation = localRotation;
	}*/
	
	public float angle = 40.0f;
	public float speed = 1.5f;

	public string rotationAxis = "z";

	private Vector3 rotateAroundZVector = Vector3.forward;//0,0,1
	private Vector3 rotateAroundXVector = Vector3.right;//1.0.0
	private Vector3 rotateAroundYVector = Vector3.up;//0,1,0

	
	Quaternion qStart, qEnd;
	
	void Start () {
		
		swingStarted = false;
		Invoke("StartSwing",swingStartDelay);
		
	}
	
	public void StartSwing() {
		Vector3 rotationAngle = rotateAroundZVector;

		 if(rotationAxis.Equals("x")) {
		  rotationAngle = rotateAroundXVector;
		}
		else if(rotationAxis.Equals("y")) {
		  rotationAngle = rotateAroundYVector;
		}
		qStart = Quaternion.AngleAxis ( angle, rotationAngle);
		qEnd   = Quaternion.AngleAxis (-angle, rotationAngle);//Vector3.forward
		
		//swipe them
		if(swingDirection<0) {
			Quaternion aux = qEnd;
			qEnd = qStart;
			qStart = aux;
		}
		
		swingStarted = true;
	}
	
	
	void Update () {
	 if(swingStarted) {
		transform.localRotation = Quaternion.Lerp (qStart, qEnd, (Mathf.Sin(Time.time * speed) + 1.0f) / 2.0f);
	 }
		
	}
	
	public void StopSwing() {
		swingStarted = false;
	}
}
