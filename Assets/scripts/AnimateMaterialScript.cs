using UnityEngine;
using System.Collections;

public class AnimateMaterialScript : MonoBehaviour {
	
	public float scrollSpeedU = 0.0f;//0.25
	public float scrollSpeedV = 0.25f;//2
	
	public float amplitudeU = 0.0f;//1
	public float amplitudeV = 0.0f;//0
	
	void Update() {
		float offsetU = 0.0f;
		float offsetV = 0.0f;
		
		if(amplitudeU > 0.0f) {
			offsetU = amplitudeU * Mathf.Sin(scrollSpeedU * Time.time);
		} else {
			offsetU = Time.time * scrollSpeedU;
		}
		
		if(amplitudeV > 0.0f) {
			offsetV = amplitudeV * Mathf.Sin(scrollSpeedV * Time.time);
		} else {
			offsetV = Time.time * scrollSpeedV;
		}
		
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(offsetU, offsetV));
	}
}
