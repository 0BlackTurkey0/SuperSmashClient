using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class ClientThread : MonoBehaviour {
    
    public string ip;
    public int port;
    private Socket clientSocket;
    private Thread connectThread, receiveThread;
    private List<Player> players;
    private int playerCount;

    void Start() {
        players = new List<Player>();
        GameObject Cat = Instantiate(Resources.Load("Cat") as GameObject, new Vector3(0, 0, 1), Quaternion.identity);
        players.Add(Cat.GetComponent<Player>());
        playerCount = 1;
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public void StartConnect() {
        connectThread = new Thread(Connect) {
            IsBackground = true
        };
        connectThread.Start();
    }

    private void Connect() {
        clientSocket.Connect(IPAddress.Parse(ip), port);
    }

    public void StopConnect() {
        clientSocket.Close();
    }

    public void Receive() {
        if (receiveThread != null && receiveThread.IsAlive)
            return;
        else {
            receiveThread = new Thread(ReceiveData)
            {
                IsBackground = true
            };
            receiveThread.Start();
        }
    }

    private void ReceiveData() {
        byte[] data = new byte[1024];
        int len = clientSocket.Receive(data);
        char[] chars = new char[len];
        Decoder decoder = Encoding.ASCII.GetDecoder();
        decoder.GetChars(data, 0, len, chars, 0);
        String receiveMSG = new String(chars);
        String[] Message = receiveMSG.Split(' ');
        int ctr = 2;
        if (players[0].ID == -1)
            players[0].ID = int.Parse(Message[0]);
        /*
        ADD A FUNCTION OF DESTROYING DISCONNECTED CLIENT GAMEOBJECT WITH SERVER SOCKET SEND!!
        UNFINISHED!!!!!!!!!!!!!!!!!!
        */
        for (int i = 0; i < int.Parse(Message[1]); i++) {
            Player Target = players.Find(x => x.ID == int.Parse(Message[ctr]));
            if (Target == null) {
                GameObject Cat = Instantiate(Resources.Load("Cat") as GameObject, new Vector3(0, 0, 1), Quaternion.identity);
                Player player = Cat.GetComponent<Player>();
                players.Add(player);
                player.ID = int.Parse(Message[ctr]);
                player.Name = "Player" + Message[ctr];
                playerCount++;
            }
            Target.pos = new Vector3(float.Parse(Message[ctr+1]), float.Parse(Message[ctr+2]), 1f);
            int num = int.Parse(Message[ctr+3]);
            for (int j = ctr+4; j < ctr+num+4; j++) {
                switch (Message[j]) {
                    case "Move_Right":
                        Target.moveRight = true;
                        break;

                    case "Move_Left":
                        Target.moveLeft = true;
                        break;

                    case "Jump":
                        Target.jump = true;
                        break;

                    case "Light_Attack":
                        Target.lightATK = true;
                        break;

                    case "Heavy_Attack":
                        Target.heavyATK = true;
                        break;

                    case "Dodge":
                        Target.dodge = true;
                        break;

                    case "Destroy":
                        players.Remove(Target);
                        Destroy(Target.gameObject);
                        break;

                    default:
                        break;
                }
            }
            ctr += (num+4);
        }
    }

    public void Send() {
        string sendMSG = "";
        if (Input.GetKey(KeyCode.D))
            sendMSG += "Move_Right ";
        if (Input.GetKey(KeyCode.A))
            sendMSG += "Move_Left ";
        if (Input.GetKeyDown(KeyCode.Space))
            sendMSG += "Jump ";
        if (Input.GetMouseButtonDown(0))
            sendMSG += "Light_Attack ";
        clientSocket.Send(Encoding.ASCII.GetBytes(sendMSG));
    }
}