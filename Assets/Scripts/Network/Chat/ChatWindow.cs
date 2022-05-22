using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using TMPro;

public class ChatWindow : MonoBehaviour
{
    public ClientHandler userRef;

    public ChatMessage chatMessagePrefab;
    public Transform messagesRoot;

    public TMP_InputField inputField;

    public List<NetworkRequest> pendingMessages;

    private void Update()
    {
        foreach (NetworkRequest pendingRequest in pendingMessages)
        {
            DisplayMessage(pendingRequest.serializedRequest);
        }
        pendingMessages = new List<NetworkRequest>();
    }

    public void DisplayMessage(string message, bool self = false)
    {
        ChatMessage newMessage = Instantiate(chatMessagePrefab, messagesRoot);
        newMessage.messageText.text = message;

        if (!self)
        {
            newMessage.background.color = Color.green;
        }
    }

    public void SendMessage()
    {
        NetworkRequest request = new NetworkRequest();
        request.sender = ServerManager.instance.server;
        request.requestType = RequestType.chat;
        request.serializedRequest = inputField.text;

        userRef.Send(request);
        DisplayMessage(inputField.text, true);
        inputField.text = string.Empty;
    }
}
