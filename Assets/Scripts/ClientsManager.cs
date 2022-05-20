using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientsManager : MonoBehaviour
{
    public List<ClientHandler> connectedClients;

    public static ClientsManager instance;

    private void Awake()
    {
        instance = this;
    }
}
