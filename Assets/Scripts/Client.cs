using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour {

    private ClientThread clientThread;

    void Start() {
        Application.runInBackground = true;
        clientThread = GetComponent<ClientThread>();
	}
	
	void Update() {
        if (clientThread.connected) {
            clientThread.Send();
            clientThread.Receive();
        }
    }

    void OnApplicationQuit() {
        clientThread.StopConnect();
    }
}
