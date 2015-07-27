using UnityEngine;
using System.Collections;

public class LaserRotate : MonoBehaviour {

public float speedx = 0.0f;
public float  speedy = 0.0f;
public float  speedz = 0.0f;

private bool UseCenter = false;

void Start(){
	if(UseCenter){
		transform.position = GetComponent<Renderer>().bounds.center+transform.position;
	}
}

void Update() {
	transform.Rotate(speedx * Time.deltaTime, speedy * Time.deltaTime, speedz * Time.deltaTime);
//print(transform.position);
}

}