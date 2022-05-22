using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClientsManager : MonoBehaviour
{
    public List<ClientHandler> connectedClients = new List<ClientHandler>();

    public static ClientsManager instance;

    private void Awake()
    {
        instance = this;
    }
}
