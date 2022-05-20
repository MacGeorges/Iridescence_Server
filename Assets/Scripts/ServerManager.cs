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
}
