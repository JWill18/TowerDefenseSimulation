using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A component that is used to find and attack nearby gameObjects with the AI_Health component
/// </summary>
public class Turret_Sensor : MonoBehaviour {

	/// <summary>
	/// The object that will be spawned when attacking
	/// </summary>
	public GameObject MissilePrefab;

	/// <summary>
	/// The object that will be rotated to aim the turret.
	/// </summary>
	public GameObject TurretHead;

	/// <summary>
	/// The location that will be used to spawn the projectile.
	/// </summary>
	public GameObject FiringPosition;

	/// <summary>
	/// How far to search for targets
	/// </summary>
	public float RadiusRange;

	/// <summary>
	/// How long to wait in between shots.
	/// </summary>
	public float FiringIntervalsInSeconds = 2.0f;

	/// <summary>
	/// The speed that the turret rotates
	/// </summary>
	public float RotationSpeed = 4.0f;

	/// <summary>
	/// The target to shoot at.
	/// </summary>
	private GameObject TargetObject;

	/// <summary>
	/// Can we fire a shot?
	/// </summary>
	private bool CanFire = true;

	/// <summary>
	/// Are we waiting to fire?
	/// </summary>
	private bool IsWaiting = false;

	/// <summary>
	/// Checks to see if the turret head is looking at the target
	/// </summary>
	private bool IsAimingAtTarget {
		get
		{
			var targetDistance = (TargetObject.transform.position - TurretHead.transform.position).normalized;
			var targetAngle = Vector3.Dot(TurretHead.transform.forward, targetDistance);
			return targetAngle > 0.95f;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		FindTargetInRange();

		// If Target has been found
		if(TargetObject != null)
		{
			// See if we need to wait before firing
			StartCoroutine(WaitToFire());

			// If we can go ahead and fire then do so
			if(CanFire && IsAimingAtTarget)
				Fire();
		}
	}

	/// <summary>
	/// Looks within range for targets to focus on
	/// </summary>
	void FindTargetInRange()
	{
		// Get a list of all colliders within range of the turret
		List<Collider> hitColliders = Physics.OverlapSphere(transform.position, RadiusRange).ToList();

		AI_Health enemyHealth = null;

		if (TargetObject != null)
		{
			// Grab the health component of the current Target
			enemyHealth = TargetObject.GetComponent<AI_Health>();
		}

		// If a target does not exist or has gotten out of range of the turret then search for new target within range
		if (enemyHealth == null || !hitColliders.Contains(TargetObject.GetComponent<Collider>()) || enemyHealth.IsDead)
		{
			// Release target reference
			TargetObject = null;

			// Search for new target
			foreach (Collider hit in hitColliders)
			{
				if (hit.gameObject.GetComponent<AI_Health>() != null)
				{
					// Found new target
					TargetObject = hit.gameObject;
				}
			}
		}
		else
		{
			// Look at target

			//var relativePos = enemyHealth.transform.position - TurretHead.transform.position;
			//relativePos.y = 0.0f;
			//TurretHead.transform.rotation = Quaternion.Slerp(TurretHead.transform.rotation, Quaternion.LookRotation(relativePos), Time.deltaTime * 50);

			// Grab the relative position between self and the target node.
			var relativePos = enemyHealth.transform.position - TurretHead.transform.position;
			TurretHead.transform.rotation = Quaternion.Lerp(TurretHead.transform.rotation, Quaternion.LookRotation(relativePos), Time.deltaTime * RotationSpeed);
		}
	}

	/// <summary>
	/// Decide whether to wait before firing or not.
	/// </summary>
	IEnumerator WaitToFire()
	{
		// If it just fired and is not waiting to fire again then wait for X seconds before firing again.
		if (!CanFire && !IsWaiting)
		{
			// Set it to be waiting until the timer is up to reset values
			IsWaiting = true;
			yield return new WaitForSeconds(FiringIntervalsInSeconds);

			// Stop waiting and allow firing.
			CanFire = true;
			IsWaiting = false;
		}
		else
		{
			// Can either start firing or needs to finish waiting.
			yield return null;
		}
	}

	/// <summary>
	/// Create projectile that will be fired towards the target.
	/// </summary>
	void Fire()
	{
		// Set Fire to false
		CanFire = false;

		// Create Missile prefab and assign the target.
		var missile = Instantiate(MissilePrefab, FiringPosition.transform.position, FiringPosition.transform.rotation);
		missile.GetComponent<AI_Projectile>().SetTarget(TargetObject);
	}
}
