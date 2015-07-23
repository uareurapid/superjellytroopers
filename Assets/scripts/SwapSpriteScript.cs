using UnityEngine;
using System.Collections;

public class SwapSpriteScript : MonoBehaviour {

	public float swapInterval = 2.0f;
	public Sprite[] sprites;
	private int lastUsedSprite = 0;
	public bool canSwap = true;
	private float lastSwapTime=0;
	
	// Use this for initialization
	void Start () {
		lastUsedSprite = 0;
		lastSwapTime=0;
	}
	
	// Update is called once per frame
	void Update () {
	
		lastSwapTime += Time.deltaTime;
		
		if( canSwap && (lastSwapTime >= swapInterval) ) {
			//time to swap images
			
			lastUsedSprite+=1;
			if(lastUsedSprite==sprites.Length) {
			  lastUsedSprite = 0;
			}

			SwapSprites();
			lastSwapTime = 0f;
		}
	}
	
	void SwapSprites() {
		
		SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
		renderer.sprite = sprites[lastUsedSprite];
		
	}
	
	//call directly
	public void SwapSprites(int index) {
		
		if(index<sprites.Length) {
			SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
			renderer.sprite = sprites[index];
		}
		
		
	}
	
	public bool CanSwap() {
	
	     return canSwap;
	}
	
	public void BlockSwap(bool block) {
		
		canSwap = block;
	}
}
