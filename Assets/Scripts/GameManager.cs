using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public class Player
	{
		public int Id;
		public Color Color;
		public string ColorName;
		public GameObject Cannon;
		public bool destroyed;

	}

	private const int NUM_PLAYERS = 2;

	public GameObject CannonPrefab;
	public Renderer Ground;
	public Text StatusText;

	private List<Player> players = new List<Player>();
	private int currentPlayerIndex;

	// Use this for initialization
	void Start()
	{
		StatusText.text = "";
		StatusText.CrossFadeAlpha(0.0f, 0, false);

		resetGame();
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void showStatusText(string text, float timeOnScreen)
	{
		StartCoroutine(statusTextCoroutine(text, timeOnScreen));
	}

	private IEnumerator statusTextCoroutine(string text, float timeOnScreen)
	{
		StatusText.text = text;
		StatusText.CrossFadeAlpha(1.0f, 1.0f, false);
		yield return new WaitForSeconds(timeOnScreen);
		StatusText.CrossFadeAlpha(0.0f, 1.0f, false);
	}

	public void OnTurnChange()
	{
		currentPlayerIndex = (currentPlayerIndex + 1) % NUM_PLAYERS;
		Player player = players[currentPlayerIndex];

		showStatusText(player.ColorName + " Player's Turn", 5);
	}

	public void OnCannonDestroyed(GameObject cannon)
	{
		// Mark player as destroyed
		foreach (Player p in players)
		{
			if (p.Cannon == cannon)
			{
				p.destroyed = true;
				break;
			}
		}

		// Check game over condition
		int numDestroyedPlayers = 0;
		Player undestroyedPlayer = null;
		foreach (Player p in players)
		{
			if (p.destroyed) numDestroyedPlayers++;
			else undestroyedPlayer = p;
		}

		if (numDestroyedPlayers == NUM_PLAYERS)
		{
			showStatusText("Draw!", 10);
		}
		else if (numDestroyedPlayers == NUM_PLAYERS - 1)
		{
			showStatusText(undestroyedPlayer.ColorName + " Player Won!", 10);
		}
	}

	private void resetGame()
	{
		List<Color> colors = new List<Color> { Color.red, Color.blue, Color.green, Color.yellow };
		List<string> colorNames = new List<string> { "Red", "Blue", "Green", "Yellow" };

		var bounds = Ground.bounds;
		for (int i = 0; i < NUM_PLAYERS; i++)
		{
			// TODO better placement randomization
			float x = Random.Range(bounds.min.x, bounds.max.x);
			float z = Random.Range(bounds.min.z, bounds.max.z);
			float dir = Random.Range(0, 360);

			Player player = new Player();
			player.Id = i;
			player.Cannon = (GameObject)Instantiate(CannonPrefab, new Vector3(x, 0, z), Quaternion.Euler(0, dir, 0));

			int colorIndex = Random.Range(0, colors.Count);
			player.Color = colors[colorIndex];
			player.ColorName = colorNames[colorIndex];

			colors.RemoveAt(colorIndex);
			colorNames.RemoveAt(colorIndex);

			players.Add(player);

			Debug.Log("created player [" + player.Id + ", " + player.ColorName + "] cannon at " + player.Cannon.transform.position.ToString("f4") + ", rot: " + player.Cannon.transform.eulerAngles.ToString("f4"));
		}

		currentPlayerIndex = 0;
		Debug.Log("player [" + players[currentPlayerIndex].Id + ", " + players[currentPlayerIndex].ColorName + "] starts");

		showStatusText(players[currentPlayerIndex].ColorName + " Player's Turn", 10);
	}

}
