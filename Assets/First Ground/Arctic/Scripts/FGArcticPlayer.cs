// First Ground - Arctic version: 1.0
// Author: Gold Experience TeamDev (http://www.ge-team.com/)
// Support: geteamdev@gmail.com
// Please direct any bugs/comments/suggestions to geteamdev@gmail.com

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion

/***************
* FGArcticPlayer class.
* 
* 	This class handles
* 		- Reset player position to (0,0,0) when it walks off from the Terrain
* 
* 	More info:
* 
* 		Skybox
* 		http://docs.unity3d.com/Documentation/Components/class-Skybox.html
* 
* 		Render Settings
* 		http://docs.unity3d.com/Documentation/Components/class-RenderSettings.html
* 
* 		Box Collider
* 		https://docs.unity3d.com/Documentation/Components/class-BoxCollider.html
*
*		MonoBehaviour.OnTriggerExit(Collider)
*		http://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerExit.html
* 
***************/

public class FGArcticPlayer : MonoBehaviour
{

#region Variables
	
#endregion {Variables}
	
// ######################################################################
// MonoBehaviour Functions
// ######################################################################

#region Component Segments	
	
	void OnTriggerExit(Collider other)
	{		
		// Reset player position when user move it away from terrain
		this.transform.localPosition = new Vector3(0,1,0);
    }
	
#endregion Component Segments
	
}
