using UnityEngine;
using System.Collections;

public class FlagController : MonoBehaviour
{
	public MeshRenderer FlagRenderer;

	GameManager gameManager;

	void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	void Update()
	{
		transform.LookAt(gameManager.Wind);
	}
}
