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

    public void StartListening()
    {
        remoteEP = new IPEndPoint(user.userIP, user.userPort);
        Debug.Log("EndPoint created : " + remoteEP);

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

    public void Send(NetworkRequest request)
    {
        byte[] byteData = Encoding.ASCII.GetBytes(JsonUtility.ToJson(request) + "<EOR>");

        //Debug.Log("Sending message to client : " + JsonUtility.ToJson(request));

        AsynchronousSocketListener.udpServer.Send(byteData, byteData.Length, remoteEP);
    }

    private void HandleRequest(NetworkRequest request)
    {
        //Debug.Log("Recieved " + request.requestType);
        switch (request.requestType)
        {
            case RequestType.ping:
                Send(request);
                break;
            case RequestType.login:
                user.userID = request.sender.userID;
                authenticated = true;
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
