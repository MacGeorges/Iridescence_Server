using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.tvOS;

public class UDP_Client
{
    public NetworkUser user
    { get; private set; }

    private UdpClient udpClient;
    private IPEndPoint remoteEP;

    public ManualResetEvent allDone = new ManualResetEvent(false);

    public void Init(ServerConnection serverConnection)
    {
        user = serverConnection.networkUser;
        udpClient = new UdpClient();
        remoteEP = new IPEndPoint(user.userIP, user.userPort);
    }
    
    public void StartListening()
    {
        //udpClient = new UdpClient(11000);

        //IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 11000);

        while (true)
        {
            byte[] data = udpClient.Receive(ref remoteEP); // listen on port 11000

            string message = Encoding.ASCII.GetString(data);

            UnityEngine.Debug.Log("receive data from " + remoteEP.ToString() + " : " + message);

            if (message.Contains("<EOR>"))
            {
                ClientHandler newClient = ClientsManager.instance.connectedClients.Find(c => (c.user.userIP == remoteEP.Address.Address) && (c.user.userPort == remoteEP.Port));

                if (newClient == null)
                {
                    UnityEngine.Debug.Log("New client!");

                    newClient = new ClientHandler();
                    newClient.user = new NetworkUser();
                    newClient.user.userType = UserType.client;
                    newClient.user.userIP = remoteEP.Address.Address;
                    newClient.user.userPort = remoteEP.Port;
                    //UnityEngine.Debug.Log("Handler Created");

                    ClientsManager.instance.connectedClients.Add(newClient);
                    ClientsManager.instance.pendingAvatarsAdd.Add(newClient);
                    //UnityEngine.Debug.Log("Handler referenced");

                    newClient.Init();

                    message = message.Replace("<EOR>", string.Empty);
                    newClient.HandleRequest(UnityEngine.JsonUtility.FromJson<NetworkRequest>(message));

                    newClient.StartListening();

                    //UnityEngine.Debug.Log("Handler is now listening");
                }

                //byte[] byteData = Encoding.ASCII.GetBytes("Ok boomer");

                //udpServer.Send(byteData, byteData.Length, remoteEP); // reply back
            }
        }
    }
}
