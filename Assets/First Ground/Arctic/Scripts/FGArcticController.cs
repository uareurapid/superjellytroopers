// First Ground - Arctic version: 1.0
// Author: Gold Experience TeamDev (http://www.ge-team.com/)
// Support: geteamdev@gmail.com
// Please direct any bugs/comments/suggestions to geteamdev@gmail.com

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion

/***************
* FGArcticController class.
* 
* 	This class handles
* 		- Switch Camera and Player control
* 		- Init Render Setting
* 		- Display GUIs
* 
***************/

public class FGArcticController : MonoBehaviour
{

#region Variables
	
	public Material m_SkyBoxMaterial = null;
	
	public GameObject m_FirstPerson = null;
	public GameObject m_OrbitCamera = null;
	
#endregion {Variables}
	
// ######################################################################
// MonoBehaviour Functions
// ######################################################################

#region Component Segments

	// Use this for initialization
	void Start ()
	{
		InitCamera();
		InitRenderSetting();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyUp(KeyCode.E))
		{
			SwitchCamera();
		}
	}
	
	void OnTriggerExit(Collider other)
	{		
		// Reset player position when user move it away from terrain
		this.transform.localPosition = new Vector3(0,1,0);
    }
	
	// OnGUI is called for rendering and handling GUI events.
	void OnGUI () {
		
		// Show version number
		GUI.Window(1, new Rect((Screen.width-220), 5, 210, 80), InfoWindow, "Info");
		
		// Show Help GUI window
		GUI.Window(2, new Rect((Screen.width-220), Screen.height-105, 210, 100), HelpWindow, "Help");
		
	}
	
#endregion Component Segments
	
// ######################################################################
// Functions Functions
// ######################################################################

#region Functions
	
	void InitCamera()
	{
		m_FirstPerson.SetActive(true);
		m_OrbitCamera.SetActive(false);
	}
	
	void InitRenderSetting()
	{
		RenderSettings.skybox = m_SkyBoxMaterial;
	}

	void SwitchCamera()
	{
		m_FirstPerson.SetActive(!m_FirstPerson.activeSelf);
		m_OrbitCamera.SetActive(!m_OrbitCamera.activeSelf);
		
		if(m_OrbitCamera.activeSelf==true)
		{
			FGArcticOrbit pFGArcticOrbit = (FGArcticOrbit) Object.FindObjectOfType(typeof(FGArcticOrbit));
			pFGArcticOrbit.TargetLookAt.transform.localPosition = new Vector3(0,0,0);
		}
	}

	// Show Help window
	void HelpWindow(int id)
	{
		//GUI.Label(new Rect(12, 25, 240, 20), "Skybox: " + string.Format("{0:00}/{1:00}",m_CurrentSkyBox+1, m_SkyboxList.Length) + " (" + m_SkyboxList[m_CurrentSkyBox].name +")");
		if(m_FirstPerson.activeSelf==true)
		{
			GUI.Label(new Rect(12, 25, 240, 20), "W/S/A/D: Move player");
			GUI.Label(new Rect(12, 50, 240, 20), "Mouse: Look around");
			GUI.Label(new Rect(12, 75, 240, 20), "E: Switch Camera");
		}
		else if(m_OrbitCamera.activeSelf==true)
		{
			GUI.Label(new Rect(12, 25, 240, 20), "Mouse drags: Orbit");
			GUI.Label(new Rect(12, 50, 240, 20), "Mouse wheel: Zoom");
			GUI.Label(new Rect(12, 75, 240, 20), "E: Switch Camera");
		}
		
	}

	// Show Info window
	void InfoWindow(int id)
	{
		GUI.Label(new Rect(15, 25, 240, 20), "First Ground - Arctic 1.0");
		GUI.Label(new Rect(15, 50, 240, 20), "www.ge-team.com/pages");
	}

#endregion Functions
	
}
