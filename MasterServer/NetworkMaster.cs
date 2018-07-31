using System;
using UnityEngine;
using RottenVisions.Tools;

namespace RottenVisions.Networking.MasterServer
{

    public class NetworkMaster : Singleton<NetworkMaster>
    {
        NetworkMasterServer masterSever;

        public NetworkMasterServer MastServer
        {
            get { return masterSever; }
        }

        protected override void Awake()
        {
            base.Awake();

            masterSever = GetComponent<NetworkMasterServer>();
        }

        protected void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            if (masterSever == null)
            {
                Debug.LogError("Failed to Initialize Master Server: NetworkMasterServer not found!!!");
                return;
            }

            masterSever.InitializeServer();
        }

        public void InitializeServer()
        {
            masterSever.InitializeServer();
        }

        public void Reset()
        {
            masterSever.ResetServer();
        }

        public void Stop()
        {
            masterSever.StopServer();
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void ListHosts()
        {
            foreach (var rooms in masterSever.ServerListGameType.Values)
            {
                //Debug.Log("Game Type:" + rooms.name);

                foreach (var room in rooms.ServerList.Values)
                {
                    //Debug.Log("Game:" + room.name + " Address:" + room.hostIp + ":" + room.hostPort);
                    Debug.Log(string.Format("{0} - {1}:{2} | {3} - Players: {4} - Protected: {5} - Comment: {6}",
                        room.name, room.connectionId, room.hostIp, room.hostPort, room.playerLimit,
                        room.passwordProtected, room.comment));
                }
            }
        }

        public int GetHostsCount()
        {
            int count = 0;
            foreach (var rooms in masterSever.ServerListGameType.Values)
            {
                foreach (var room in rooms.ServerList.Values)
                {
                    count++;
                }
            }

            return count;
        }

        public void SetPort(int port)
        {
            masterSever.MasterServerPort = port;
        }

        public int GetPort()
        {
            return masterSever.MasterServerPort;
        }
    }
}