using UnityEngine;
using System.Collections;

public class RotateBackAndForthScript : MonoBehaviour {

	public float angle;
	public float period;

	private float time;

	// Use this for initialization
	void Start () {
		transform.LookAt(Camera.main.transform);
	}
	
	// Update is called once per frame
	void Update () {
		time = time + Time.deltaTime;
    	float phase = Mathf.Sin(time / period);
    	transform.localRotation = Quaternion.Euler( new Vector3(0, phase * angle, 0));
	}
}
