using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	// Constants
	// ---------
	// Singmaster notation: assigns unique letter to a face of a cube (used commonly for Rubik's cubes)
	// F = front, L = left, R = right, U = up/top, D = down/bottom, B = back

	// Assume master camera is at (0,0,-z) looking at cube at (0,0,0), for some z > 0
	// Axis look like:
	//  ^ y
	//  |
	//  x--> x    
	private const int NUM_FACES = 6;
	private const int MAX_PLAYERS = NUM_FACES; // an alias 

	// Prefab for the paddle.
	public GameObject playerPrefab;

	// Cube dimensions.
	private const float CUBE_SIZE = 10.0f; // 10x10x10 cube
	private const float CS2 = CUBE_SIZE / 2;
	private const float CAM_DEPTH = 15.0f; // how far back the camera is

	// Camera object.
	public GameObject camera;

	// Id to reference ourself
	public int playerId;

	// Game state
	public bool gameStarted = false;

	// The number of lives for players.
	private const int INITIAL_HEALTH = 50;
	private int[] playerLives = new int[NUM_FACES];

	// Spawn points for players (initial position for player prefab).
	private static Vector3[] playerSpawnPositions = new Vector3[NUM_FACES] {
		new Vector3(0.0f,0.0f,-CS2), // F
		new Vector3(-CS2,0.0f,0.0f), // L
		new Vector3(CS2,0.0f,0.0f),  // R
		new Vector3(0.0f,CS2,0.0f),  // U
		new Vector3(0.0f,-CS2,0.0f), // D
		new Vector3(0.0f,0.0f,CS2)   // B
	};

	// Spawn rotations for players (initial rotation for player prefab - to align flat on cube face.
	private static Quaternion[] playerSpawnRotations = new Quaternion[NUM_FACES] {
		Quaternion.LookRotation(new Vector3(0.0f,0.0f,1.0f),new Vector3(0.0f,1.0f,0.0f)), // F
		Quaternion.LookRotation(new Vector3(1.0f,0.0f,0.0f),new Vector3(0.0f,1.0f,0.0f)), // L
		Quaternion.LookRotation(new Vector3(-1.0f,0.0f,0.0f),new Vector3(0.0f,1.0f,0.0f)), // R
		Quaternion.LookRotation(new Vector3(0.0f,-1.0f,0.0f),new Vector3(0.0f,0.0f,1.0f)), // U
		Quaternion.LookRotation(new Vector3(0.0f,1.0f,0.0f),new Vector3(0.0f,0.0f,-1.0f)), // D
		Quaternion.LookRotation(new Vector3(0.0f,0.0f,-1.0f),new Vector3(0.0f,1.0f,0.0f))  // B  
	};

	// Camera position for players.
	private static Vector3[] cameraPositions = new Vector3[NUM_FACES] {
		new Vector3(0.0f,0.0f,-CAM_DEPTH), // F
		new Vector3(-CAM_DEPTH,0.0f,0.0f), // L
		new Vector3(CAM_DEPTH,0.0f,0.0f),  // R
		new Vector3(0.0f,CAM_DEPTH,0.0f),  // U
		new Vector3(0.0f,-CAM_DEPTH,0.0f), // D
		new Vector3(0.0f,0.0f,CAM_DEPTH)   // B 
	};

	// Camera rotations for players.
	private static Quaternion[] cameraRotations = new Quaternion[NUM_FACES] {
		Quaternion.LookRotation(new Vector3(0.0f,0.0f,1.0f),new Vector3(0.0f,1.0f,0.0f)), // F
		Quaternion.LookRotation(new Vector3(1.0f,0.0f,0.0f),new Vector3(0.0f,1.0f,0.0f)), // L
		Quaternion.LookRotation(new Vector3(-1.0f,0.0f,0.0f),new Vector3(0.0f,1.0f,0.0f)), // R
		Quaternion.LookRotation(new Vector3(0.0f,-1.0f,0.0f),new Vector3(0.0f,0.0f,1.0f)), // U
		Quaternion.LookRotation(new Vector3(0.0f,1.0f,0.0f),new Vector3(0.0f,0.0f,-1.0f)), // D
		Quaternion.LookRotation(new Vector3(0.0f,0.0f,-1.0f),new Vector3(0.0f,1.0f,0.0f))  // B 
	};

	void Awake() {
		initialiseHealth ();
	}

	// Update is called once per frame
	void Update() {
	}

	void OnGUI() {
		if (gameStarted) {
			for (int i = 0; i < playerLives.Length; i++) {
				GUI.Label (new Rect (2, 2 + (20 * i), 40, 20), i.ToString() + ": " + playerLives [i]);
				if (i == playerId) {
					GUI.Label (new Rect (42, 2 + (20 * i), 30, 20), "<<<<");
				}
			}
		}
	}

	// Called once by each new player (including host).
	public void SpawnPlayer(int playerNum) {
		// Create paddle (Note: Network.Instantiate is an RPC call).
		Network.Instantiate(playerPrefab,
		                    playerSpawnPositions[playerNum], 
			                playerSpawnRotations[playerNum],
		                    0);

		/// Position camera
		camera.transform.position = cameraPositions [playerNum];
		camera.transform.rotation = cameraRotations [playerNum];

		gameStarted = true;
		playerId = playerNum;
	}

	public void handleWallHit(Faces wallFace) {
		if (Network.isServer && gameStarted) {
			int playerNum = getPlayerId(wallFace);
			networkView.RPC("decreaseHealth", RPCMode.AllBuffered, playerNum);
		}
	}

	private int getPlayerId(Faces wall) {
		// Important: depends on the order of the enums in Faces.
		return (int) wall;
	}
	
	private void initialiseHealth() {
		for (int i = 0; i < playerLives.Length; i++) {
			playerLives [i] = INITIAL_HEALTH;
		}
	}

	[RPC]
	public void decreaseHealth(int playerNum) {
		playerLives [playerNum] -= 5;
	}
}
