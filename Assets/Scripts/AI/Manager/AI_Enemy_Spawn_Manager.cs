using UnityEngine;

/// <summary>
/// The manager that determines spawn waves.
/// </summary>
[ExecuteInEditMode]
public class AI_Enemy_Spawn_Manager : MonoBehaviour {

	/// <summary>
	/// The size of the spawn area on the X axis
	/// </summary>
	public float SpawnAreaX = 10;

	/// <summary>
	/// The size of the spawn area on the Y axis
	/// </summary>
	public float SpawnAreaZ = 10;

	/// <summary>
	/// The number of enemies to be spawned next.
	/// </summary>
	public int SpawnQuantity;

	/// <summary>
	/// The maximum number of enemies that can be spawned.
	/// </summary>
	public int MaxSpawnQuantity;
	
	/// <summary>
	/// Timer between each spawn wave
	/// </summary>
	public float SpawnIntervals;

	/// <summary>
	/// The maximum amount of spawn waves.
	/// </summary>
	public int MaxSpawnWaves;

	/// <summary>
	/// Determines if the spawn quantity should increase each wave
	/// </summary>
	public bool IncrementSpawnQuantityPerInterval;

	/// <summary>
	/// The margin of error for the enemies when they get close to each node.
	/// </summary>
	public float EnemyMovementMOE = 5f;

	/// <summary>
	/// The movement speed of the enemy units
	/// </summary>
	public float EnemyMovementSpeed = 5f;

	/// <summary>
	/// The rotation speed of the enemy units
	/// </summary>
	public float EnemyRotationSpeed = 2f;

	/// <summary>
	/// The enemy group to spawn
	/// </summary>
	public GameObject SpawnEnemy;

	/// <summary>
	/// The collider that will be used to represent the area in which to spawn enemies
	/// </summary>
	private BoxCollider SpawnArea;

	/// <summary>
	/// The amount of time between each enemy wave.
	/// </summary>
	private float IntervalTimer;

	/// <summary>
	/// The current spawn wave.
	/// </summary>
	private int CurrentSpawnWave = 1;

	// Use this for initialization
	public void Start()
	{
		// If the game is playing then start up the spawns
		if (Application.isPlaying)
		{
			// Spawn Enemies
			SpawnEnemies();

			// Set interval timer
			IntervalTimer = SpawnIntervals;
		}
	}

	// Update is called once per frame
	public void Update () {

		// If in editor mode, then do some setup.
		if (Application.isEditor)
		{
			CheckNecessaryComponents();

			// Get the box collider component
			SpawnArea = gameObject.GetComponent<BoxCollider>();

			// Set scale of collider and disable the collider.
			gameObject.transform.localScale = new Vector3(SpawnAreaX, 0.05f, SpawnAreaZ);
			SpawnArea.enabled = false;
		}

		// If the game is playing and there are enemies to spawn and the maximum spawn waves has not been met.
		if(Application.isPlaying && SpawnIntervals > 0 && CurrentSpawnWave != MaxSpawnWaves)
		{
			// Count down the timer.
			IntervalTimer -= Time.deltaTime;

			// If the timer runs out, spawn enemies and restart timer.
			if(IntervalTimer <= 0)
			{
				// Increment to the next wave.
				IncrementSpawner();

				SpawnEnemies();

				// Reset timer
				IntervalTimer = SpawnIntervals;
			}
		}
	}

	/// <summary>
	/// Increases the spawn wave and determines how many to spawned.
	/// </summary>
	private void IncrementSpawner()
	{
		// Increment Spawn wave
		CurrentSpawnWave++;

		// If the amount of enemies spawned can increase
		if (IncrementSpawnQuantityPerInterval && SpawnQuantity <= MaxSpawnQuantity)
		{
			// Increase number of enemies spawned
			SpawnQuantity = DetermineNewSpawnQuantity() < MaxSpawnQuantity ? DetermineNewSpawnQuantity() : MaxSpawnQuantity;
		}
	}

	/// <summary>
	/// Checks for any necessary external components that are required to complete functionality.
	/// </summary>
	private void CheckNecessaryComponents()
	{
		// If there is no box collider then add one.
		if (gameObject.GetComponent<BoxCollider>() == null)
		{
			gameObject.AddComponent<BoxCollider>();
		}

		// If there is no renderer than add one.
		if(gameObject.GetComponent<Renderer>() == null)
		{
			gameObject.AddComponent<Renderer>();
		}
	}

	/// <summary>
	/// Used to spawn a wave of enemies
	/// </summary>
	private void SpawnEnemies()
	{
		// Gets the size of the renderer
		var staticRender = GetComponent<Renderer>();
		var minX = transform.position.x - (staticRender.bounds.size.x / 2);
		var maxX = transform.position.x + (staticRender.bounds.size.x / 2);
		var minZ = transform.position.z - (staticRender.bounds.size.z / 2);
		var maxZ = transform.position.z + (staticRender.bounds.size.z / 2);

		// Spawn as many enemies as needed
		for (int i = 0; i < SpawnQuantity; i++)
		{
			// Instantiate the enemy.
			var spawnedEnemy = CreateEnemyAtLocation(new Vector3(Random.Range(minX, maxX), transform.position.y + 1.5f, Random.Range(minZ, maxZ)), transform.rotation);

			// Modify any default values.
			ModifyEnemy(spawnedEnemy);
		}
	}

	/// <summary>
	/// Instantiate an enemy GameObject at the given location
	/// </summary>
	/// <param name="location">The location to place the enemy</param>
	/// <param name="rotation">The rotatino of the enemy upon spawning</param>
	/// <returns></returns>
	private GameObject CreateEnemyAtLocation(Vector3 location, Quaternion rotation)
	{
		return Instantiate(SpawnEnemy, location, rotation);
	}

	/// <summary>
	/// Modifies the default values of the enemy.
	/// </summary>
	/// <param name="enemy">The GameObject that represents the enemy</param>
	private void ModifyEnemy(GameObject enemy)
	{
		// Grab the AI Pathing script
		var enemyPathing = enemy.GetComponent<AI_Pathing>();

		// If the pathing script was found then modify the values.
		if (enemyPathing != null)
		{
			enemyPathing.MoveSpeed = EnemyMovementSpeed;
			enemyPathing.TargetDistanceMOE = EnemyMovementMOE;
			enemyPathing.RotationSpeed = EnemyRotationSpeed;
		}
	}

	/// <summary>
	/// Determines how many more enemies to spawn
	/// </summary>
	/// <returns>The number of enemies to spawn</returns>
	private int DetermineNewSpawnQuantity()
	{
		return SpawnQuantity + Mathf.RoundToInt(SpawnQuantity * 0.5f);
	}
}
