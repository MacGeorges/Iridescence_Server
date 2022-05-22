using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

[System.Serializable]
public class ClientHandler
{
    public NetworkUser user;
    public StateObject state;

    public NetworkAvatar avatarRef;

    public bool authenticated;

    public void Send(NetworkRequest request)
    {
        byte[] byteData = Encoding.ASCII.GetBytes(JsonUtility.ToJson(request) + "<EOF>");

        //Debug.Log("Sending message to client : " + JsonUtility.ToJson(request));

        state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), state.workSocket);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            int bytesSent = state.workSocket.EndSend(ar);
            //Debug.Log("Sent " + bytesSent + " bytes to client.");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void Receive()
    {
        try
        {
            state.workSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            string message = string.Empty;

            int bytesRead = state.workSocket.EndReceive(ar);

            if (bytesRead > 0)
            {
                message = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);

                if (message.Contains("<EOF>"))
                {
                    string[] messages = message.Split("<EOF>");

                    foreach (string subMessage in messages)
                    {
                        string cleanSubMessage = subMessage.Replace("<EOF>", "");

                        if (string.IsNullOrEmpty(cleanSubMessage)) { continue; }

                        HandleRequest(JsonUtility.FromJson<NetworkRequest>(cleanSubMessage));
                    }

                    state.buffer = new byte[StateObject.BufferSize];
                    state.workSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);

                    return;
                }

                // Not all data received. Get more.  
                state.workSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);

            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
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
