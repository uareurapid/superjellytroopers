using UnityEngine;
using System.Collections;

public class MainPlatformScript : MonoBehaviour {

    public GameObject[] platforms;
	// Use this for initialization
	
	private bool isOpen = false;
	
	void Start () {
	  
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Open() {
	
	 if(!isOpen) {
	 
			foreach(GameObject obj in platforms) {
				PlatformScript platform = obj.GetComponentInChildren<PlatformScript>();
				if(platform!=null) {
					platform.Open();
				}
			}
			
			GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
			if(jelly!=null) {
				JellyScript script = jelly.GetComponent<JellyScript>();
				if(script!=null) {
					script.isFalling = true;
					jelly.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
					MoveScript move = jelly.GetComponent<MoveScript>();
					if(move!=null) {
					  move.enabled = false;
					}
				}
			}
			
			isOpen = true;
	 
	 }
	
	  
	  
	}
	
	public void Close() {
	
	 if(isOpen) {
			foreach(GameObject obj in platforms) {
				PlatformScript platform = obj.GetComponentInChildren<PlatformScript>();
				if(platform!=null) {
					platform.Close();
				}
			}
		isOpen = false;
	 
	 }
		
	}
}
