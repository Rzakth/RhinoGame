using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;

public class Chat : MonoBehaviour, IChatClientListener
{
    [HideInInspector]
    public string Username;
    public ChatClient ChatClient;
    public InputField InputField;
    public Text ChatContent;

    void Start()
    {
        ChatClient = new ChatClient(this);
    }

    void Update()
    {
        ChatClient.Service();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("Chat - " + level + " - " + message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("Chat - OnChatStateChange: " + state);
    }

    public void OnConnected()
    {
        Debug.Log("Chat - User: " + Username + "is connected");
        ChatClient.Subscribe(PhotonNetwork.CurrentRoom.Name, creationOptions: new ChannelCreationOptions() { PublishSubscribers = true });
    }

    public void OnDisconnected()
    {
        Debug.Log("Chat - User: " + Username + "is diconnected");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        ChatChannel currentChat;
        if(ChatClient.TryGetChannel(PhotonNetwork.CurrentRoom.Name, out currentChat))
        {
            ChatContent.text = currentChat.ToStringMessages();
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
       
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for(int i = 0; i < channels.Length; i++)
        {
            if(results[i])
            {
                Debug.Log("Chat - Subscribed to " + channels[i] + "channel");
                ChatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, "says hi!");
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }

    public void SendMessage()
    {
        ChatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, InputField.text);
        InputField.text = "";
    }
}
