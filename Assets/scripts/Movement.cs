using UnityEngine;
using System.Collections;


/// <summary>
/// Player controller and behavior
/// </summary>
public class Movement : MonoBehaviour
{

private Vector3 vClickPos;
private bool bMoving = false;
private bool bMouseDown = false;

void Update () {
	/*
	if(bMouseDown == false)
	{
		if(Input.GetMouseButtonDown(0) == true)
		{
			RaycastHit hit ;
			if(collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), hit, Mathf.Infinity))
			{
				bMoving = true;
				vClickPos = Input.mousePosition;
			}
			
			bMouseDown = true;
		}
	}
	else
	{
		if(Input.GetMouseButton(0) == false)
		{
			bMouseDown = false;
			bMoving = false;
		}
			
		if(bMoving == true)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			transform.localPosition = ray.GetPoint(12.0f);
		}
		
		
	}*/
 }

}