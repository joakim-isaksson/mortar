using UnityEngine;
using System.Collections;

/**
 * Currently supported keys:
 *		- WASD/QZ (or arrow keys/numpad +-) : move camera
 *		- LMB drag : camera look
 *		- numpad 0 : fire the cannon of the current player
 *		- numpad 1 : teleport camera to the cannon of the current player
 * 
 **/
public class DebugObject : MonoBehaviour
{
	public Transform mainCamera;

	GameManager gameManager;

	bool mouseDown;
	Vector3 lastMousePos;

	void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	void Update()
	{
		// Handle camera move & look
		moveCamera();

		// Fire current cannon
		if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))
		{
			GameManager.Player player = gameManager.CurrentPlayer;
			CannonController cannon = player.Cannon.GetComponent<CannonController>();
			cannon.Fire();
		}

		// Teleport to current cannon
		if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
		{
			GameManager.Player player = gameManager.CurrentPlayer;
			Vector3 pos = player.TeleportLocations[0].Position;
			Vector3 fwd = player.Cannon.transform.forward;

			Vector3 finalPos = pos - fwd * 2;
			finalPos.y += 1;

			mainCamera.transform.position = finalPos;
			mainCamera.transform.LookAt(pos);
		}
	}

	void moveCamera()
	{
		Vector3 vec = Vector2.zero;

		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			vec.z += 1;
		}
		else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			vec.z -= 1;
		}

		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			vec.x -= 1;
		}
		else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			vec.x += 1;
		}

		if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.KeypadPlus))
		{
			vec.y += 1;
		}
		else if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.KeypadMinus))
		{
			vec.y -= 1;
		}

		Transform t = mainCamera.transform;

		t.position += t.forward * vec.z * Time.deltaTime;
		t.position += t.right * vec.x * Time.deltaTime;
		t.position += t.up * vec.y * Time.deltaTime;

		if (Input.GetMouseButton(0))
		{
			Vector3 mousePos = Input.mousePosition;
			if (mouseDown)
			{
				Vector3 dm = mousePos - lastMousePos;

				float x = dm.x / Screen.width;
				float y = -dm.y / Screen.height;

				mainCamera.transform.Rotate(0, x * 45, 0, Space.World);
				mainCamera.transform.Rotate(y * 45, 0, 0);
			}

			mouseDown = true;
			lastMousePos = mousePos;
		}
		else mouseDown = false;

	}
}
