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
		public List<Teleport.TeleportLocation> TeleportLocations = new List<Teleport.TeleportLocation>();
		public int CurrentTeleportIndex;
	}

	const int NUM_PLAYERS = 2;

	public GameObject CannonPrefab;
	public Renderer Ground;
	public Text StatusText;

	List<Player> players = new List<Player>();
	int currentPlayerIndex;
	bool gameOver;

	Teleport teleport;

	public Player CurrentPlayer
	{
		get { return players[currentPlayerIndex]; }
	}

	void Awake()
	{
		teleport = new Teleport(this);
	}

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
		teleport.Update();
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
		if (gameOver) return;

		currentPlayerIndex = (currentPlayerIndex + 1) % NUM_PLAYERS;
		Player player = players[currentPlayerIndex];
		player.CurrentTeleportIndex = 0;

		showStatusText(player.ColorName + " Player's Turn", 5);

		teleport.TeleportTo(player.TeleportLocations[0]);
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
			gameOver = true;
			showStatusText("Draw!", 10);
		}
		else if (numDestroyedPlayers == NUM_PLAYERS - 1)
		{
			gameOver = true;
			showStatusText(undestroyedPlayer.ColorName + " Player Won!", 10);
		}

		if (gameOver)
		{
			StartCoroutine(resetGameCoroutine());
		}
	}

	private IEnumerator resetGameCoroutine()
	{
		yield return new WaitForSeconds(15);
		resetGame();
	}

	private void resetGame()
	{
		Debug.Log("resetGame");

		// Clear game area
		foreach (Player p in players)
		{
			if (p.Cannon != null)
			{
				Object.Destroy(p.Cannon);
			}
		}
		players.Clear();

		List<Color> colors = new List<Color> { Color.red, Color.blue, Color.green, Color.yellow };
		List<string> colorNames = new List<string> { "Red", "Blue", "Green", "Yellow" };

		var bounds = Ground.bounds;
		for (int i = 0; i < NUM_PLAYERS; i++)
		{
			// TODO better placement randomization
			float x = Random.Range(bounds.min.x, bounds.max.x);
			float z = Random.Range(bounds.min.z, bounds.max.z);
			float dir = Random.Range(0, 360);

			GameObject cannon = (GameObject)Instantiate(CannonPrefab, new Vector3(x, 0, z), Quaternion.Euler(0, dir, 0));

			Player player = new Player();
			player.Id = i;
			player.Cannon = cannon;

			int colorIndex = Random.Range(0, colors.Count);
			player.Color = colors[colorIndex];
			player.ColorName = colorNames[colorIndex];

			colors.RemoveAt(colorIndex);
			colorNames.RemoveAt(colorIndex);

			// Generate waypoints for player
			{
				Teleport.TeleportLocation t = new Teleport.TeleportLocation();
				t.Position = cannon.transform.position - cannon.transform.forward;
				player.TeleportLocations.Add(t);
			}

			player.CurrentTeleportIndex = 0;
			players.Add(player);

			Debug.Log("created player [" + player.Id + ", " + player.ColorName + "] cannon at " + player.Cannon.transform.position.ToString("f4") + ", rot: " + player.Cannon.transform.eulerAngles.ToString("f4"));
		}

		currentPlayerIndex = 0;
		Debug.Log("player [" + players[currentPlayerIndex].Id + ", " + players[currentPlayerIndex].ColorName + "] starts");

		gameOver = false;

		showStatusText(players[currentPlayerIndex].ColorName + " Player's Turn", 10);

		teleport.TeleportTo(players[currentPlayerIndex].TeleportLocations[0]);
	}

}
