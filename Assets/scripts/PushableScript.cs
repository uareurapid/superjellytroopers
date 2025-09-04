using UnityEngine;
using System.Collections;
using RescueJelly;

public class PushableScript : MonoBehaviour { 

  private Vector2 position;
  private bool pushLeft = false;
  private bool moving = false;

  void Start() {
    position = transform.position;
  }

  void Awake() {


  }

  void Update() {

   if(moving) {
     if(pushLeft) {
     //moving left
     }
     else {
     //move right
     }
   }
   else {

   }
  }

  void OnTriggerEnter2D(Collider2D otherCollider) {
		
  }

  //handle the collision with another sprite (not other trigger)
  void OnCollisionEnter2D(Collision2D collision) {
    PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
    if(player!=null) {
      Vector2 playerPosition = collision.gameObject.transform.position;
      //check the position
    }
  }
}

