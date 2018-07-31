using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace RottenVisions.Networking.MasterServer
{

	public class Rooms
	{
		public string name;
		public Dictionary<string, MasterMsgTypes.Room> ServerList = new Dictionary<string, MasterMsgTypes.Room>();

		public bool AddHost(string gameName, string comment, string hostIp, int hostPort, int connectionId)
		{
			if (ServerList.ContainsKey(gameName))
			{
				return false;
			}

			MasterMsgTypes.Room room = new MasterMsgTypes.Room();
			room.name = gameName;
			room.comment = comment;
			room.hostIp = hostIp;
			room.hostPort = hostPort;
			room.connectionId = connectionId;
			ServerList[gameName] = room;

			return true;
		}

		public MasterMsgTypes.Room[] GetRooms()
		{
			return ServerList.Values.ToArray();
		}
	}

	public class NetworkMasterServer : MonoBehaviour
	{
		public int MasterServerPort;

		// map of gameTypeNames to rooms of that type
		public Dictionary<string, Rooms> ServerListGameType = new Dictionary<string, Rooms>();

		public void InitializeServer()
		{
			if (NetworkServer.active)
			{
				Debug.LogError("Already Initialized");
				return;
			}

			NetworkServer.Listen(MasterServerPort);

			// system msgs
			NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnect);
			NetworkServer.RegisterHandler(MsgType.Disconnect, OnServerDisconnect);
			NetworkServer.RegisterHandler(MsgType.Error, OnServerError);

			// application msgs
			NetworkServer.RegisterHandler(MasterMsgTypes.RegisterHostId, OnServerRegisterHost);
			NetworkServer.RegisterHandler(MasterMsgTypes.UnregisterHostId, OnServerUnregisterHost);
			NetworkServer.RegisterHandler(MasterMsgTypes.RequestListOfHostsId, OnServerListHosts);

			Debug.Log(string.Format("... Initialized ~Ascension Master Server~ on Port: {0} ...", MasterServerPort));

			//if(ServerConsole.Created)
			//    ServerConsole.ConsoleWrite(string.Format("... Initialized ~Ascension Master Server~ on Port: {0} ...", MasterServerPort), System.ConsoleColor.Gray);

			DontDestroyOnLoad(gameObject);
		}

		public void ResetServer()
		{
			NetworkServer.Reset();
		}

		public void StopServer()
		{
			NetworkServer.Shutdown();
		}

		Rooms EnsureRoomsForGameType(string gameTypeName)
		{
			if (ServerListGameType.ContainsKey(gameTypeName))
			{
				return ServerListGameType[gameTypeName];
			}

			Rooms newRooms = new Rooms();
			newRooms.name = gameTypeName;
			ServerListGameType[gameTypeName] = newRooms;
			return newRooms;
		}

		// --------------- System Handlers -----------------

		void OnServerConnect(NetworkMessage netMsg)
		{
			Debug.Log(string.Format("Master received client: {0}|{1} - Chan: {2}", netMsg.conn.connectionId,
				netMsg.conn.address, netMsg.channelId));
		}

		void OnServerDisconnect(NetworkMessage netMsg)
		{
			Debug.Log(string.Format("Master lost client: {0}|{1} - Chan:{2}", netMsg.conn.connectionId,
				netMsg.conn.address, netMsg.channelId));

			// remove the associated host
			foreach (var rooms in ServerListGameType.Values)
			{
				foreach (var room in rooms.ServerList.Values)
				{
					if (room.connectionId == netMsg.conn.connectionId)
					{
						// tell other players?

						// remove room
						rooms.ServerList.Remove(room.name);

						Debug.Log(string.Format("Room {0} [{1}:{2}] closed due to host leaving.", room.name,
							room.hostIp, room.hostPort));
						break;
					}
				}
			}

		}

		void OnServerError(NetworkMessage netMsg)
		{
			Debug.Log("Server Error from Master");
		}

		// --------------- Application Handlers -----------------

		void OnServerRegisterHost(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MasterMsgTypes.RegisterHostMessage>();
			var rooms = EnsureRoomsForGameType(msg.gameTypeName);

			int result = (int) MasterMsgTypes.NetworkMasterServerEvent.RegistrationSucceeded;
			if (!rooms.AddHost(msg.gameName, msg.comment, netMsg.conn.address, msg.hostPort, netMsg.conn.connectionId))
			{
				result = (int) MasterMsgTypes.NetworkMasterServerEvent.RegistrationFailedGameName;
				Debug.Log(string.Format("Failed to register host: Name: {0} : Comment: {1} : Addr: {2}:{3} Conn: {4}",
					msg.gameName, msg.comment, netMsg.conn.address, msg.hostPort, netMsg.conn.connectionId));
			}
			else
			{
				Debug.Log(string.Format("Registered host: Name: {0} : Comment: {1} : Addr: {2}:{3} Conn: {4}",
					msg.gameName, msg.comment, netMsg.conn.address, msg.hostPort, netMsg.conn.connectionId));
			}

			var response = new MasterMsgTypes.RegisteredHostMessage();
			response.resultCode = result;
			netMsg.conn.Send(MasterMsgTypes.RegisteredHostId, response);
		}



		void OnServerUnregisterHost(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MasterMsgTypes.UnregisterHostMessage>();

			// find the room
			var rooms = EnsureRoomsForGameType(msg.gameTypeName);
			if (!rooms.ServerList.ContainsKey(msg.gameName))
			{
				//error
				Debug.Log("OnServerUnregisterHost game not found: " + msg.gameName);
				Debug.Log(string.Format("Failed to unregister host: Name: {0} : Addr: {1} Conn: {2}", msg.gameName,
					netMsg.conn.address, netMsg.conn.connectionId));
				return;
			}

			var room = rooms.ServerList[msg.gameName];
			if (room.connectionId != netMsg.conn.connectionId)
			{
				//error
				Debug.Log("OnServerUnregisterHost connection mismatch:" + room.connectionId);
				Debug.Log(string.Format("Failed to unregister host: Name: {0} : Addr: {1} Conn: {2}", msg.gameName,
					netMsg.conn.address, netMsg.conn.connectionId));
				return;
			}

			rooms.ServerList.Remove(msg.gameName);

			Debug.Log(string.Format("Unregistered host: Name: {0} : Addr: {1} Conn: {2}", msg.gameName,
				netMsg.conn.address, netMsg.conn.connectionId));
			// tell other players?

			var response = new MasterMsgTypes.RegisteredHostMessage();
			response.resultCode = (int) MasterMsgTypes.NetworkMasterServerEvent.UnregistrationSucceeded;
			netMsg.conn.Send(MasterMsgTypes.UnregisteredHostId, response);
		}

		void OnServerListHosts(NetworkMessage netMsg)
		{
			Debug.Log("Listing hosts...");
			var msg = netMsg.ReadMessage<MasterMsgTypes.RequestHostListMessage>();
			if (!ServerListGameType.ContainsKey(msg.gameTypeName))
			{
				var err = new MasterMsgTypes.ListOfHostsMessage();
				err.resultCode = -1;
				netMsg.conn.Send(MasterMsgTypes.ListOfHostsId, err);
				return;
			}

			var rooms = ServerListGameType[msg.gameTypeName];
			var response = new MasterMsgTypes.ListOfHostsMessage();
			response.resultCode = 0;
			response.hosts = rooms.GetRooms();
			netMsg.conn.Send(MasterMsgTypes.ListOfHostsId, response);
		}


		void OnGUI()
		{
			if (NetworkServer.active)
			{
				GUI.Label(new Rect(400, 0, 200, 20), "Online port:" + MasterServerPort);
				if (GUI.Button(new Rect(400, 20, 200, 20), "Reset  Master Server"))
				{
					ResetServer();
				}
			}
			else
			{
				if (GUI.Button(new Rect(400, 20, 200, 20), "Init Master Server"))
				{
					InitializeServer();
				}
			}

			int y = 100;
			foreach (var rooms in ServerListGameType.Values)
			{
				GUI.Label(new Rect(400, y, 200, 20), "GameType:" + rooms.name);
				y += 22;
				foreach (var room in rooms.ServerList.Values)
				{
					GUI.Label(new Rect(420, y, 200, 20),
						"Game:" + room.name + " addr:" + room.hostIp + ":" + room.hostPort);
					y += 22;
				}
			}
		}
	}
}