using UnityEngine;
using System.Collections;

public class AnimateTextureScript : MonoBehaviour {

    public float scrollSpeed = 0.5F;
  
    void Start() {

    }
    void Update() {
        float offset = Time.time * scrollSpeed;
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
