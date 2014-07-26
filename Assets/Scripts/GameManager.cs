using UnityEngine;
using System.Collections;

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
	private enum Faces {Front, Left, Right, Up, Down, Back};

	// Prefab for the paddle
	public GameObject playerPrefab;

	// Cube dimensions
	private const float CUBE_SIZE = 5.0f; // 5x5x5 cube
	private const float CS2 = CUBE_SIZE/2;
	private const float CAM_DEPTH = 15.0f; // how far back the camera is

	// Basis vectors for player movement for each face. 
	// For each face, we give the two vectors that define what is right movement and what is up movement 
	// respectively from perspective of the player
	private static Vector3[,] playerVelocityBases = new Vector3[NUM_FACES,2] {
		{new Vector3(1.0f,0.0f,0.0f), new Vector3(0.0f,1.0f,0.0f)}, // F
		{new Vector3(0.0f,0.0f,-1.0f), new Vector3(0.0f,1.0f,0.0f)}, // L
		{new Vector3(0.0f,0.0f,1.0f), new Vector3(0.0f,1.0f,0.0f)}, // R
		{new Vector3(1.0f,0.0f,0.0f), new Vector3(0.0f,0.0f,1.0f)}, // U
		{new Vector3(1.0f,0.0f,0.0f), new Vector3(0.0f,1.0f,-1.0f)}, // D
		{new Vector3(-1.0f,0.0f,0.0f), new Vector3(0.0f,1.0f,0.0f)}  // B
	};

	// Swawn points for players (initial position for player prefab)
	// and swawn rotations (initial rotation for player prefab - to align flat on cube face)
	private static Vector3[] playerSpawnPositions = new Vector3[NUM_FACES] {
		new Vector3(0.0f,0.0f,-CS2), // F
		new Vector3(-CS2,0.0f,0.0f), // L
		new Vector3(CS2,0.0f,0.0f),  // R
		new Vector3(0.0f,CS2,0.0f),  // U
		new Vector3(0.0f,-CS2,0.0f), // D
		new Vector3(0.0f,0.0f,CS2)   // B
	};
	// TODO: fix these quaternions
	private static Quaternion[] playerSpawnRotations = new Quaternion[NUM_FACES] {
		Quaternion.identity, // F
		Quaternion.identity, // L
		Quaternion.identity, // R
		Quaternion.identity, // U
		Quaternion.identity, // D
		Quaternion.identity  // B 
	};

	// TODO: fix these camera positions
	private static Vector3[] cameraPositions = new Vector3[NUM_FACES] {
		new Vector3(0.0f,0.0f,-CAM_DEPTH), // F
		new Vector3(-CAM_DEPTH,0.0f,0.0f), // L
		new Vector3(CAM_DEPTH,0.0f,0.0f),  // R
		new Vector3(0.0f,CAM_DEPTH,0.0f),  // U
		new Vector3(0.0f,-CAM_DEPTH,0.0f), // D
		new Vector3(0.0f,0.0f,CAM_DEPTH)   // B 
	};
	// TODO: fix these camera rotations
	private static Quaternion[] cameraRotations = new Quaternion[NUM_FACES] {
		Quaternion.LookRotation(new Vector3(0.0f,0.0f,1.0f),new Vector3(0.0f,1.0f,0.0f)), // F
		Quaternion.LookRotation(new Vector3(1.0f,0.0f,0.0f),new Vector3(0.0f,1.0f,0.0f)), // L
		Quaternion.LookRotation(new Vector3(-1.0f,0.0f,0.0f),new Vector3(0.0f,1.0f,0.0f)), // R
		Quaternion.LookRotation(new Vector3(0.0f,-1.0f,0.0f),new Vector3(0.0f,0.0f,1.0f)), // U
		Quaternion.LookRotation(new Vector3(0.0f,1.0f,0.0f),new Vector3(0.0f,0.0f,-1.0f)), // D
		Quaternion.LookRotation(new Vector3(0.0f,0.0f,-1.0f),new Vector3(0.0f,1.0f,0.0f))  // B 
	};


	// Ingame Data
	// -----------
	private UnityEngine.Object[] playerObjects = new UnityEngine.Object[MAX_PLAYERS];
	private int[] playerHealths = new int[MAX_PLAYERS];
	private bool[] playerIsConnected = new bool[MAX_PLAYERS] { false, false, false, false, false, false };
	private enum GameStatus {Waiting, InProgress, Finished};
	private GameStatus status = GameStatus.Waiting;
	private int numPlayers = 0;

	
	void Start() {
	}

	public void SpawnPlayer() {
		if (NewPlayerCanJoin()) {
			int newPlayerIdx = numPlayers++;
			playerHealths[newPlayerIdx] = 100;
			playerIsConnected[newPlayerIdx] = true;
			playerObjects[newPlayerIdx] = Network.Instantiate(playerPrefab, 
			                                       playerSpawnPositions[newPlayerIdx], 
			                                       playerSpawnRotations[newPlayerIdx], 
			                                       0);

		} else {
			Debug.Log ("Player cannot join, game is full");
		}
	}

	bool NewPlayerCanJoin() {
		return numPlayers < MAX_PLAYERS;
	}
	
	// Update is called once per frame
	void Update() {
	
	}
}
