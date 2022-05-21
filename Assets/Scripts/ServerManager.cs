using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public NetworkUser server;

    public static ServerManager instance;

    void Start()
    {
        instance = this;

        Thread listenerThread = new Thread(AsynchronousSocketListener.StartListening);
        listenerThread.Start();
    }
}
