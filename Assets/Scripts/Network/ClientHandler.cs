using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

[Serializable]
public class ClientHandler
{
    public NetworkUser user;
    //public StateObject state;

    IPEndPoint remoteEP;

    public NetworkAvatar avatarRef;

    public bool authenticated;

    public void Init()
    {
        remoteEP = new IPEndPoint(user.userIP, user.userPort);
    }

    public void StartListening()
    {
        while (true)
        {           
            byte[] data = AsynchronousSocketListener.udpServer.Receive(ref remoteEP);

            string message = Encoding.ASCII.GetString(data);

            //Debug.Log("receive data from " + remoteEP.ToString() + " : " + message);

            if (message.Contains("<EOR>"))
            {

                message = message.Replace("<EOR>", "");

                HandleRequest(JsonUtility.FromJson<NetworkRequest>(message));
            }
        }
    }

    public void Send(NetworkRequest request, bool reliable = false)
    {
        byte[] byteData = Encoding.ASCII.GetBytes(JsonUtility.ToJson(request) + "<EOR>");

        //Debug.Log("Sending message to client : " + JsonUtility.ToJson(request));

        AsynchronousSocketListener.udpServer.Send(byteData, byteData.Length, remoteEP);
    }

    public void HandleRequest(NetworkRequest request)
    {
        //Debug.Log("Recieved request : " + request.requestType);
        switch (request.requestType)
        {
            case RequestType.ping:
                Send(request);
                break;
            case RequestType.login:
                user = JsonUtility.FromJson<NetworkUser>(request.serializedRequest);

                NetworkRequest responseRequest = new NetworkRequest();
                responseRequest.sender = ServerManager.instance.server;
                responseRequest.requestType = RequestType.login;
                responseRequest.serializedRequest = JsonUtility.ToJson(user);
                Send(responseRequest);
                avatarRef.shouldInit = true;
                break;
            case RequestType.regionChange:
                break;
            case RequestType.objectUpdate:
                break;
            case RequestType.chat:
                ChatManager.instance.ChatRecieved(request);
                break;
            case RequestType.playerAction:
                ClientsManager.instance.AvatarUpdate(request);
                break;
            default:
                break;
        }
    }
}
