using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    void Start()
    {
        Thread listenerThread = new Thread(AsynchronousSocketListener.StartListening);
        listenerThread.Start();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClientsManager.instance.connectedClients[0].Send("Server just pressed a key!<EOF>");
        }
    }
}
