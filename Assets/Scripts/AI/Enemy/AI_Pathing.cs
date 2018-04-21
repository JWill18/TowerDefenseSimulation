using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI_Pathing : MonoBehaviour {

    /// <summary>
    ///  Margin of Error for Distance from Node
    /// </summary>
	private const float TargetDistanceMOE = 2f;

    /// <summary>
    /// Speed for moving self.
    /// </summary>
	public float MoveSpeed;

    /// <summary>
    /// Speed for rotating self.
    /// </summary>
	public float RotationSpeed;

    /// <summary>
    /// List of Nodes that will be followed for navigation.
    /// </summary>
	public List<AI_PathNodes> PathNodes = new List<AI_PathNodes>();

    /// <summary>
    /// Node to move towards.
    /// </summary>
    private AI_PathNodes TargetNode;

    /// <summary>
    /// Used to select a Node to move towards from the Node List
    /// </summary>
	private int NodeIndex = 0;

	// Use this for initialization
	void Start () {

        // If there are no path nodes assigned at the start of the game then try to find some.
        if (PathNodes.Count == 0)
        {
            // Finds all of the path nodes 
            PathNodes = FindObjectsOfType<AI_PathNodes>().ToList();
        }

        // If there are path nodes to follow
        if(PathNodes.Count > 0)
        {
            // Assign first Node
            TargetNode = PathNodes[NodeIndex];
        }
	}
	
	// Update is called once per frame
	void Update () {

        // Check to see if at the target node.
		if (AtTargetNode())
		{
			if (NodeIndex != PathNodes.Count - 1)
			{
                // Go to the next node
				NodeIndex++;
                TargetNode = PathNodes[NodeIndex];
			}
			else
			{
                // Destroy self once all nodes have been reached
				Destroy(gameObject);
			}
		}

		// Keep moving from one node to another
		MoveToTargetNode();
	}

	/// <summary>
	/// Used to determine if the AI has reached the target node.
	/// </summary>
	/// <returns>If the Ai has reached the target node.</returns>
	private bool AtTargetNode()
	{
        // If we are within range of the target node.
		return Vector3.Distance(transform.position, TargetNode.transform.position) <= TargetDistanceMOE;
	}

	/// <summary>
	/// Rotates the AI towards the target node and moves towards it.
	/// </summary>
	private void MoveToTargetNode()
	{
        // Grab the relative position between self and the target node.
		Vector3 relativePos = TargetNode.transform.position - transform.position;

        // Lerp the rotation to look at the target node
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(relativePos), Time.deltaTime * RotationSpeed);

        // Move forward at a constant rate.
		transform.Translate(0, 0, MoveSpeed * Time.deltaTime);
	}
}
