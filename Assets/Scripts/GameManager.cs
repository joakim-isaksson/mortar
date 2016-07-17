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

	[HideInInspector]
	public Vector3 Wind = Vector3.zero;
	public float MaxWindForce;

	public GameObject CannonPrefab;
	public Text StatusText;

	public GameMap[] GameMaps;

	public float TurnChangeDelay = 5;
	public float ResetDelay = 15;

	public float GenMinDistance = 100;
	public float GenMaxDistance = 500;
	public float GenMinSceneryDistance = 10;

	[HideInInspector]
	public GameMap CurrentGameMap;

	List<Player> players = new List<Player>();
	int currentPlayerIndex = -1; // -1, so that the first changeTurn() will increment it to zero
	bool gameOver;

	Teleport teleport;

	public Player CurrentPlayer
	{
		get { return players[currentPlayerIndex]; }
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

	void showStatusText(string text, float timeOnScreen)
	{
		StartCoroutine(statusTextCoroutine(text, timeOnScreen));
	}

	IEnumerator statusTextCoroutine(string text, float timeOnScreen)
	{
		StatusText.text = text;
		StatusText.CrossFadeAlpha(1.0f, 1.0f, false);
		yield return new WaitForSeconds(timeOnScreen);
		StatusText.CrossFadeAlpha(0.0f, 1.0f, false);
	}

	void changeTurn()
	{
		currentPlayerIndex = (currentPlayerIndex + 1) % NUM_PLAYERS;
		Player player = players[currentPlayerIndex];

		Debug.Log("player " + player.Id + " turn");

		showStatusText(player.ColorName + " Player's Turn", 5);
		teleport.TeleportTo(player.TeleportLocations[0]);
		player.CurrentTeleportIndex = 0;
		player.Cannon.GetComponent<CannonController>().FiringEnabled = true;

		// Update wind direction and force
		Wind = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward * Random.Range(0, MaxWindForce);
	}

	IEnumerator changeTurnDelayed()
	{
		yield return new WaitForSeconds(TurnChangeDelay);

		if (gameOver) yield break;
		changeTurn();
	}

	IEnumerator resetGameCoroutine()
	{
		yield return new WaitForSeconds(ResetDelay);
		resetGame();
	}

	void resetGame()
	{
		Debug.Log("resetGame");

		// Clear game area
		if (CurrentGameMap != null)
		{
			Destroy(CurrentGameMap);
		}
		players.Clear();

		// Pick a random map
		CurrentGameMap = Instantiate(GameMaps[Random.Range(0, GameMaps.Length)]);

		List<Color> colors = new List<Color> { Color.red, Color.blue, Color.green, Color.yellow };
		List<string> colorNames = new List<string> { "Red", "Blue", "Green", "Yellow" };

		for (int i = 0; i < NUM_PLAYERS; i++)
		{
			Vector3 pos = CurrentGameMap.GetSpawnPosition(GenMinSceneryDistance, GenMinDistance, GenMaxDistance, CurrentGameMap.CannonPlacementMap);
			// TODO implement dynamic cannon height (0 - 0.39)
			pos.y -= 0.39f;
			float dir = Random.Range(0, 360);
			GameObject cannon = (GameObject)Instantiate(CannonPrefab, pos, Quaternion.Euler(0, dir, 0));
			cannon.transform.parent = CurrentGameMap.WorldContainer.transform;

			Player player = new Player();
			player.Id = i;
			player.Cannon = cannon;

			int colorIndex = Random.Range(0, colors.Count);
			player.Color = colors[colorIndex];
			player.ColorName = colorNames[colorIndex];

			FlagController flag = cannon.GetComponentInChildren<FlagController>();
			flag.gameObject.GetComponent<MeshRenderer>().material.color = player.Color;

			CannonController cannonController = cannon.GetComponentInChildren<CannonController>();
			cannonController.OnCannonFired = delegate { cannonController.FiringEnabled = false; };
			cannonController.OnCannonExploded = delegate { OnCannonDestroyed(player.Cannon); };
			cannonController.OnMissileExploded = delegate { StartCoroutine(changeTurnDelayed()); };
			cannonController.Player = player;

			colors.RemoveAt(colorIndex);
			colorNames.RemoveAt(colorIndex);

			player.CurrentTeleportIndex = 0;
			players.Add(player);

			Debug.Log("created player [" + player.Id + ", " + player.ColorName + "] cannon at " + player.Cannon.transform.position.ToString("f4") + ", rot: " + player.Cannon.transform.eulerAngles.ToString("f4"));
		}

		// Generate waypoints for players
		foreach (Player p in players)
		{
			// Cannon location
			Teleport.TeleportLocation t = new Teleport.TeleportLocation();
			t.Position = p.Cannon.transform.position;
			// TODO account for dynamic height once such is implemented
			t.Position.y += 0.39f;
			p.TeleportLocations.Add(t);

			// Bird view locations, one per opponent
			foreach (Player p2 in players)
			{
				if (p != p2)
				{
					t = new Teleport.TeleportLocation();
					Vector3 p2p = p2.Cannon.transform.position - p.Cannon.transform.position;

					Vector3 perp = Vector3.Cross(p2p, new Vector3(0, 1, 0)).normalized;

					t.Position = p.Cannon.transform.position + p2p * 0.5f + perp * (p2p.magnitude / 2);
					t.Position.y = CurrentGameMap.Terrain.SampleHeight(new Vector3(t.Position.x, 0, t.Position.z)) + 80;

					Vector3 toCannon = p.Cannon.transform.position - t.Position;
					toCannon.y = 0;
					toCannon.Normalize();

					t.Rotation = Quaternion.LookRotation(toCannon);
					t.ForceRotation = true;

					p.TeleportLocations.Add(t);
				}
			}
		}

		// Spawn scenery objects
		CurrentGameMap.SpawnSceneryObjects();

		gameOver = false;
		changeTurn();
	}
}
