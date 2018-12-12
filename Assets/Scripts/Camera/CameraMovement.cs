using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public float MovementSpeed = 25.0f;
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetAxis("Vertical") != 0)
		{
			transform.Translate(transform.up * Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime);
		}

		if(Input.GetAxis("Horizontal") != 0)
		{
			transform.Translate(transform.right * Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime);
		}
	}
}
