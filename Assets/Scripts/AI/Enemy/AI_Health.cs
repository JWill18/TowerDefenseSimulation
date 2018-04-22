using UnityEngine;

/// <summary>
/// The AI health component that controls how much health there is and if the game object is already dead or not.
/// </summary>
public class AI_Health : MonoBehaviour {

	/// <summary>
	/// The maximum amount of health that the AI will start with
	/// </summary>
	public float MaxHealth;

	/// <summary>
	/// The current amount of health that the AI currently has
	/// </summary>
	private float CurrentHealth;

	/// <summary>
	/// Is the AI dead or not?
	/// </summary>
	public bool IsDead;

	// Use this for initialization
	void Start () {
		CurrentHealth = MaxHealth;
		IsDead = false;
	}

	/// <summary>
	/// Used to deal damage to self
	/// </summary>
	/// <param name="amount">How much damage is being taken.</param>
	public void TakeDamage(float amount)
	{
		// Decrease health
		CurrentHealth -= amount;

		// If the AI is dead, then play death animation
		if(CurrentHealth <= 0)
		{
			IsDead = true;
			Death();
		}
	}

	/// <summary>
	/// Used to do any necessary work to destroy object
	/// </summary>
	public void Death()
	{
		// Destroy object
		Destroy(gameObject);
	}
}
