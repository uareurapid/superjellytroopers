using UnityEngine;
using System.Collections;


public static class RendererExtensions {

	//Extension class to check if a plan is outside the camera
	public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
	}

	//We will call this method on the leftmost object of the infinite layer.
}
