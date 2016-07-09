using UnityEngine;
using System.Collections;

public class FlagController : MonoBehaviour
{
	[HideInInspector]
	public MeshRenderer FlagRenderer;

	GameManager gameManager;

	void Start()
	{
		FlagRenderer = GetComponent<MeshRenderer>();
		gameManager = FindObjectOfType<GameManager>();
	}

	void Update()
	{
		Debug.Log("wind: " + gameManager.Wind.ToString("f4"));
		transform.rotation = Quaternion.LookRotation(gameManager.Wind.normalized, Vector3.up);
		Debug.Log("transform: " + transform.forward.ToString("f4"));
	}
}
