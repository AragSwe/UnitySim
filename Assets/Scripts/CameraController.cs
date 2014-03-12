using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public float CameraSpeed = 10.0f;
	
	private float cameraSpeedDelta
	{
		get { return CameraSpeed * Time.deltaTime; }
	}
	
	// Update is called once per frame
	void Update ()
	{
		Camera.main.transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * cameraSpeedDelta, Space.World);
		Camera.main.transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * cameraSpeedDelta, Space.World);
		
		Camera.main.transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * 5, Space.Self);
	}
}
