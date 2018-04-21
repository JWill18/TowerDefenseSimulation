using UnityEngine;

/// <summary>
/// A component that launches itself into the air and lands at its designated target or last known position.
/// </summary>
public class AI_Projectile : MonoBehaviour {

	/// <summary>
	/// Represents how much damage will be done to the target once it hits
	/// </summary>
	public float Damage;

	/// <summary>
	/// How long it will take to reach the destination from start location.
	/// </summary>
	private float timeTravel;

	/// <summary>
	/// The target that will be used for destination
	/// </summary>
	private GameObject Target;

	/// <summary>
	/// The current amount of time that has passed while moving to the destination
	/// </summary>
	private float time = 0f;

	/// <summary>
	/// The spawn location of the object
	/// </summary>
	private Vector3 origin;

	/// <summary>
	/// The destination location to travel to. If the target is already destroyed then it is the last known position of the object.
	/// </summary>
	private Vector3 destination;

	// Use this for initialization
	void Start () {
		// Grabs the spawn location
		origin = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		// If Target is not empty and the time travel is not 0
		if(Target != null && timeTravel == 0)
		{
			// Calculate the amount of time to travel
			timeTravel = CalcTravelTime();
			time = timeTravel; // in seconds
		}

		// Move along the projectile arc.
		MoveAlongArc();
	}

	/// <summary>
	/// Moves projectile along the curve of the arc.
	/// </summary>
	private void MoveAlongArc()
	{
		// Decrease time
		time -= Time.deltaTime;

		// If the target has not been destroyed, grab new location.
		if (Target != null)
		{
			destination = Target.transform.position;
		}
		
		// Find out percentage of time that has passed by. Returns value between 0 and 1.
		float progress = Mathf.InverseLerp(timeTravel, 0f, time);

		// set projectile base position
		var newPosition = Vector3.Lerp(origin, destination, progress);

		// Adjust projectile height to create arc curve
		newPosition.y += Mathf.Cos(Mathf.Lerp(-Mathf.PI * 0.5f, Mathf.PI * 0.5f, progress)) * 5;

		// Set new position
		transform.position = newPosition;

		// If the destination is reached without hitting anything, destroy self.
		if(transform.position == destination)
		{
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		// If the collision object has a AI_Health component, do damage to it
		if(collision.gameObject.GetComponent<AI_Health>() != null)
		{
			var enemyHealth = collision.gameObject.GetComponent<AI_Health>();
			enemyHealth.TakeDamage(Damage);
		}

		// Destory self on contact
		Destroy(gameObject);
	}

	/// <summary>
	/// Tries to calculate an amount of time to reach the target by taking the distance into accoung.
	/// </summary>
	/// <returns>How long it should take to reach the target.</returns>
	private float CalcTravelTime()
	{
		return Vector3.Distance(gameObject.transform.position, Target.transform.position)/20;
	}

	/// <summary>
	/// Sets the target that will be hit.
	/// </summary>
	/// <param name="newTarget">The object to hit</param>
	public void SetTarget(GameObject newTarget)
	{
		Target = newTarget;
	}
}
