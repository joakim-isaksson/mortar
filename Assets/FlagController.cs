using UnityEngine;
using System.Collections;

public class FlagController : MonoBehaviour
{
	public Color FlagColor;
	public MeshRenderer FlagRenderer;

	GameManager gameManager;

	void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	void Awake()
	{
		FlagRenderer.material.color = FlagColor;
	}

	void Update()
	{
		transform.LookAt(gameManager.Wind);
	}
}
