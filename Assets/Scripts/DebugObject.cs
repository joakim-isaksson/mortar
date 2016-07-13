using UnityEngine;
using System.Collections;

/**
 * Currently supported keys:
 *		- WASD/QZ (or arrow keys/numpad +-) : move camera
 *		- LMB drag : camera look
 *		- numpad 0 : fire the current cannon
 *		- numpad 1 : teleport camera to the current cannon
 *		- IJKL : rotate the current cannon
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
		controlCamera();

		// Handle cannon controls
		controlCannon();
	}

	void controlCannon()
	{
		// Rotate cannon
		int x = 0, y = 0;

		if (Input.GetKey(KeyCode.I)) y = 1;
		else if (Input.GetKey(KeyCode.K)) y = -1;

		if (Input.GetKey(KeyCode.J)) x = -1;
		else if (Input.GetKey(KeyCode.L)) x = 1;

		if (x != 0 || y != 0)
		{
			// FIXME needs a better way for figuring out which rotator is which
			CannonRotator[] rotators = gameManager.CurrentPlayer.Cannon.GetComponentsInChildren<CannonRotator>();
			if (x != 0) rotators[1].Rotate(360 * x * Time.deltaTime);
			if (y != 0) rotators[0].Rotate(360 * y * Time.deltaTime);
		}

		// Fire current cannon
		if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))
		{
			GameManager.Player player = gameManager.CurrentPlayer;
			CannonController cannon = player.Cannon.GetComponent<CannonController>();
			cannon.Fire();
		}
	}

	void controlCamera()
	{
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

		// Camera movement
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

		// Camera rotation
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
