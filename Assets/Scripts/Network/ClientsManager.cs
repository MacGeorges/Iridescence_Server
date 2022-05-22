using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClientsManager : MonoBehaviour
{
    public List<ClientHandler> connectedClients = new List<ClientHandler>();

    public List<ClientHandler> pendingAvatars = new List<ClientHandler>();

    public NetworkAvatar avatarPrefab;

    public static ClientsManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (pendingAvatars.Count > 0)
        {
            foreach (ClientHandler pendingAvatar in pendingAvatars)
            {
                if (pendingAvatar.avatarRef) { continue; }

                NetworkAvatar newAvatar = Instantiate(avatarPrefab);
                pendingAvatar.avatarRef = newAvatar;
                newAvatar.clientHandlerRef = pendingAvatar;
            }

            pendingAvatars = new List<ClientHandler>();
        }
    }
}
