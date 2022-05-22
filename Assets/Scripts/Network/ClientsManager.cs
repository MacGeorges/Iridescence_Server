using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClientsManager : MonoBehaviour
{
    public List<ClientHandler> connectedClients = new List<ClientHandler>();

    public List<ClientHandler> pendingAvatarsAdd = new List<ClientHandler>();

    public Dictionary<ClientHandler, SerializableTransform> pendingAvatarsUpdate = new Dictionary<ClientHandler, SerializableTransform>();

    public NetworkAvatar avatarPrefab;

    public static ClientsManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        List<ClientHandler> pendingAvatarsAddWork = new List<ClientHandler>(pendingAvatarsAdd);
        pendingAvatarsAdd = new List<ClientHandler>();

        foreach (ClientHandler pendingAvatar in pendingAvatarsAddWork)
        {
            if (pendingAvatar.avatarRef) { continue; }

            NetworkAvatar newAvatar = Instantiate(avatarPrefab);
            pendingAvatar.avatarRef = newAvatar;
            newAvatar.clientHandlerRef = pendingAvatar;
        }

        Dictionary<ClientHandler, SerializableTransform> pendingAvatarsUpdateWork = new Dictionary<ClientHandler, SerializableTransform>(pendingAvatarsUpdate);
        pendingAvatarsUpdate = new Dictionary<ClientHandler, SerializableTransform>();

        foreach(KeyValuePair<ClientHandler, SerializableTransform> pendingAvatar in pendingAvatarsUpdateWork)
        {
            if (!pendingAvatar.Key.avatarRef) { continue; }

            pendingAvatar.Key.avatarRef.transform.position = pendingAvatar.Value.position.ToVector3();
            pendingAvatar.Key.avatarRef.transform.rotation = pendingAvatar.Value.rotation.ToQuaternion();
        }
    }

    public void AvatarUpdate(NetworkRequest playerRequest)
    {
        ClientHandler client = connectedClients.Find(c => c.user.userID == playerRequest.sender.userID);

        if(client != null)
        {
            if (!pendingAvatarsUpdate.ContainsKey(client))
            {
                pendingAvatarsUpdate.Add(client, JsonUtility.FromJson<PlayerActionRequest>(playerRequest.serializedRequest).spatialData);
            }
        }
    }
}
