using UnityEngine;
using System.Collections;

public class ParticlePlayerScript : MonoBehaviour {

  
  public float playInterval = 20f; 
  public float startDelay = 5f;
	// Use this for initialization
	void Start () {

	  if(playInterval>0f) {
	    InvokeRepeating("PlayParticle",startDelay,playInterval);
	  }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PlayParticle() {

	 if(GetComponent<ParticleSystem>()!=null) {
		GetComponent<ParticleSystem>().Play();
	 }
	 else {
		foreach(ParticleSystem particle in GetComponentsInChildren<ParticleSystem>()) {
			particle.Play();
	    }
	 }


	}
}
