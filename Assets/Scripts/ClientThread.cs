using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientThread : MonoBehaviour {
    
    public string ip;
    public int port;
    private Socket clientSocket;
    private Thread connectThread, receiveThread;
    private Player player;
    private GameObject Cat;

    void Start() {
        Cat = Instantiate(Resources.Load("Cat") as GameObject, new Vector3(0, 0, 1), Quaternion.identity);
        player = Cat.GetComponent<Player>();
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

    public void Send() {
        string sendMSG = "";
        byte[] data = new byte[1024];
        if (Input.GetKey(KeyCode.D))
            sendMSG += "Move_Right ";
        if (Input.GetKey(KeyCode.A))
            sendMSG += "Move_Left ";
        if (Input.GetKeyDown(KeyCode.Space))
            sendMSG += "Jump ";
        if (Input.GetMouseButtonDown(0))
            sendMSG += "Light_Attack ";
        data = Encoding.ASCII.GetBytes(sendMSG);
        clientSocket.Send(data);
    }
}