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
		if (gameManager.Wind != Vector3.zero) transform.rotation = Quaternion.LookRotation(gameManager.Wind.normalized, Vector3.up);
	}
}
