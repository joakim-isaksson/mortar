using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Teleport
{
	public class TeleportLocation
	{
		public Vector3 Position = Vector3.zero;
		public Quaternion Rotation = Quaternion.identity;
		public bool ForceRotation;
	}

	enum Action
	{
		NOTHING, NEXT, PREVIOUS
	}

	private const float DEADZONE = 0.1f;

	GameManager gameManager;

	public Teleport(GameManager gameManager)
	{
		this.gameManager = gameManager;
	}

	Transform reference
	{
		get
		{
			var top = SteamVR_Render.Top();
			return (top != null) ? top.origin : null;
		}
	}

	private Action checkInput()
	{
		var leftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
		var rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

		List<int> indices = new List<int>();
		if (leftIndex != -1) indices.Add(leftIndex);
		if (rightIndex != -1) indices.Add(rightIndex);

		foreach (int index in indices)
		{
			if (SteamVR_Controller.Input(index).GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
			{
				// FIXME implement previous/next
				/*
				Vector2 touchLoc = SteamVR_Controller.Input(index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

				if (touchLoc.x <= -DEADZONE) return Action.PREVIOUS;
				else if (touchLoc.x >= DEADZONE) return Action.NEXT;
				*/

				return Action.NEXT;
			}
		}
		return Action.NOTHING;
	}

	public void TeleportTo(TeleportLocation location)
	{
		Debug.Log("teleporting to " + location.Position.ToString("f4"));

		var t = reference;
		if (t == null) return;

		//Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
		//t.rotation = location.rotation;
		t.position = location.Position;
		if (location.ForceRotation)
		{
			float eyeAngle = SteamVR_Render.Top().head.eulerAngles.y;
			float targetAngle = location.Rotation.eulerAngles.y;
			t.Rotate(0, targetAngle - eyeAngle, 0);
		}
		else
		{
			t.eulerAngles = Vector3.zero;
		}
		//t.position = position.position - new Vector3(t.GetChild(0).localPosition.x, 0f, t.GetChild(0).localPosition.z) - headPosOnGround;
	}

	// Needs to be called every frame
	public void Update()
	{
		int inc = 0;

		switch (checkInput())
		{
			case Action.PREVIOUS:
				inc = -1;
				break;
			case Action.NEXT:
				inc = 1;
				break;
		}

		if (inc != 0)
		{
			GameManager.Player player = gameManager.CurrentPlayer;
			int index = mod(player.CurrentTeleportIndex + inc, player.TeleportLocations.Count);
			player.CurrentTeleportIndex = index;

			TeleportTo(player.TeleportLocations[index]);
		}
	}

	private int mod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }
}
