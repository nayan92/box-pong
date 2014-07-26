using UnityEngine;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
	private const string typeName = "Box_Pong";
	private const int maxPlayers = 6;

	private string roomName = "Room Name";
	private string playerName = "Player Name";
	private int playerNumber;
	private List<string> playerNames = new List<string>();
	private HostData[] hostList;
	private bool gameStarted = false;

	public GameManager gameManager;

	void OnGUI() {
		if (!Network.isClient && !Network.isServer) {
			playerName = GUI.TextArea(new Rect(Screen.width / 2 - 125, 10, 250, 20), playerName);
			roomName = GUI.TextArea(new Rect(100, 75, 250, 20), roomName);
			if (GUI.Button(new Rect(100, 100, 250, 100), "Host Room")) {
				StartServer();
			}
			
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts")) {
				RefreshHostList();
			}
			
			if (hostList != null) {
				for (int i = 0; i < hostList.Length; i++) {
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName)) {
						JoinServer(hostList[i]);
					}
				}
			}

		} else if(!gameStarted) {
			GUI.Label(new Rect (2, 2, 150, 20), playerNumber.ToString());
			GUI.Label(new Rect (2, 20, 150, 120), string.Join("\n", playerNames.ToArray()));
			if (Network.isServer && playerNames.Count > 1 && GUI.Button(new Rect(100, 100, 250, 100), "Start Game")) {
				networkView.RPC("StartGame", RPCMode.AllBuffered); 
			}
		}
	}
	
	private void StartServer() {
		Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, roomName);
	}
	
	void OnServerInitialized() {
		SendName();
	}

	void OnPlayerConnected(NetworkPlayer player) {
		if(playerNames.Count < maxPlayers) {
			networkView.RPC("ServerAccepted", player);
		} else {
			networkView.RPC("ServerFull", player);
		}
	}
	
	private void RefreshHostList() {
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)	{
		if (msEvent == MasterServerEvent.HostListReceived) {
			hostList = MasterServer.PollHostList();
		}
	}
	
	private void JoinServer(HostData hostData) {
		Network.Connect(hostData);
	}

	private void SendName() {
		playerNumber = playerNames.Count;
		networkView.RPC("InsertPlayer", RPCMode.AllBuffered, playerName); 
	}
	
	[RPC]
	public void InsertPlayer(string name) {
		playerNames.Add(name);
	}

	[RPC]
	public void StartGame() {
		gameStarted = true;
		gameManager.SpawnPlayer(playerNumber);
	}

	[RPC]
	public void ServerAccepted() {
		SendName();
	}

	[RPC]
	public void ServerFull() {
		Network.Disconnect();
	}
}
