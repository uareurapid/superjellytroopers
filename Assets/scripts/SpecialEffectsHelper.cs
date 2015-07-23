using UnityEngine;

/// <summary>
/// Creating instance of particles from code with no effort
/// </summary>
public class SpecialEffectsHelper : MonoBehaviour
{
	/// <summary>
	/// Singleton
	/// </summary>
	private static SpecialEffectsHelper instance;
	
	public ParticleSystem hitEffect;
	public ParticleSystem landEffect;
	public ParticleSystem soulEffect;
	public ParticleSystem waterSplashEffect;
	
	public ParticleSystem parachuteReleaseEffect;
	public ParticleSystem laserExplosionEffect;
	public ParticleSystem smokeTrailEffect;
	
	public ParticleSystem explosionEffect;

	public ParticleSystem bloodSplaterEffect;	

	public ParticleSystem lavaSplashEffect;

	public ParticleSystem reserveParachuteEffect;

	
	void Awake()
	{
		// Register the singleton
		if(instance!=null) {
			Debug.Log("There is another instance special effects running");
		}
		else {
			instance = this;
		}
		
	}
	
	/// <summary>
	/// Create an explosion at the given location
	/// </summary>
	/// <param name="position"></param>
	/*public void Explosion(Vector3 position)
	{
		// Smoke on the water
		instantiate(smokeEffect, position);
		
		// Tu tu tu, tu tu tudu
		
		// Fire in the sky
		instantiate(fireEffect, position);


		instantiate(boomExplosion, position);


	}*/

	public void PlayJellyHitDeadEffect(Vector3 position) {
		instantiate(hitEffect, position);
	}

	public void PlayBloodSplaterEffect(Vector3 position) {
		instantiate(bloodSplaterEffect, position);
	}
		
	
	public void PlayJellyLandedEffect(Vector3 position) {
		instantiate(landEffect, position);
	}
	
	public void PlayJellySoulEffect(Vector3 position) {
		instantiate(soulEffect, position);
	}
	
	public void PlayParachuteReleaseEffect(Vector3 position) {
		instantiate(parachuteReleaseEffect, position);
	}
	
	public void PlayWaterSplashEffect(Vector3 position) {
		instantiate(waterSplashEffect, position);
	}

	public void PlayLavaSplashEffect(Vector3 position) {
		instantiate(lavaSplashEffect, position);
	}

	public void PlayReserveParachuteEffect(Vector3 position) {
		instantiate(reserveParachuteEffect, position);
	}
	
	public void PlayLaserExplosionEffect(Vector3 position) {
		instantiate(laserExplosionEffect, position);
	}
	
	public void PlaySmokeTrailEffect(Vector3 position) {
		instantiate(smokeTrailEffect, position);
	}
	
	public void PlayExplosionEffect(Vector3 position) {
		instantiate(explosionEffect, position);
	}

	
	
	public static SpecialEffectsHelper Instance {
		get
		{
			if (instance == null)
			{
				instance = (SpecialEffectsHelper)FindObjectOfType(typeof(SpecialEffectsHelper));
				if (instance == null)
					instance = (new GameObject("SpecialEffectsHelper")).AddComponent<SpecialEffectsHelper>();
			}
			return instance;
		}
	}

	//public void ThunderboltEffect(Vector3 position) {
	//	instantiate(thunderbolt, position);
	//}
	
	/// <summary>
	/// Instantiate a Particle system from prefab
	/// </summary>
	/// <param name="prefab"></param>
	/// <returns></returns>
	private ParticleSystem instantiate(ParticleSystem prefab, Vector3 position)
	{
		ParticleSystem newParticleSystem = Instantiate(
			prefab,
			position,
			Quaternion.identity
			) as ParticleSystem;
		
		// Make sure it will be destroyed
		Destroy(
			newParticleSystem.gameObject,
			newParticleSystem.startLifetime
			);
		
		return newParticleSystem;
	}
	
	private Transform instantiateTransform(Transform prefab, Vector3 position)
	{
		Transform newTransform = Instantiate(
			prefab,
			position,
			Quaternion.identity
			) as Transform;
		
		// Make sure it will be destroyed
		Destroy(
			newTransform.gameObject,
			3f
			);
		
		return newTransform;
	}
	
	//Note: Because we can have multiple particles in the scene at the same time, 
	//we are forced to create a new prefab each time. 
	//If we were sure that only one system was used at a time, 
	//we would have kept the reference and use the same everytime.

}
