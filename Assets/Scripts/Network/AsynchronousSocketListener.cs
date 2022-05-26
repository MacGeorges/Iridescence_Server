using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// State object for reading client data asynchronously  
public class StateObject
{
    // Size of receive buffer.  
    public const int BufferSize = 1024;

    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];

    // Received data string.
    public StringBuilder sb = new StringBuilder();

    // Client socket.
    public Socket workSocket = null;
}

public class AsynchronousSocketListener
{
    public static UdpClient udpServer;

    public static ManualResetEvent allDone = new ManualResetEvent(false);

    public static void StartListening()
    {
        udpServer = new UdpClient(11000);

        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 11000);

        while (true)
        {
            byte[] data = udpServer.Receive(ref remoteEP); // listen on port 11000

            string message = Encoding.ASCII.GetString(data);

            //UnityEngine.Debug.Log("receive data from " + remoteEP.ToString() + " : " + message);

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

                message = message.Replace("<EOR>", string.Empty);
                newClient.HandleRequest(UnityEngine.JsonUtility.FromJson<NetworkRequest>(message));

                newClient.StartListening();
                //UnityEngine.Debug.Log("Handler is now listening");
            }

            //byte[] byteData = Encoding.ASCII.GetBytes("Ok boomer");

            //udpServer.Send(byteData, byteData.Length, remoteEP); // reply back
        }
    }

    //public static void AcceptCallback(IAsyncResult ar)
    //{
    //    UnityEngine.Debug.Log("AcceptCallback");
    //    allDone.Set();

    //    Socket listener = (Socket)ar.AsyncState;
    //    Socket handler = listener.EndAccept(ar);

    //    ClientHandler clientHandler = new ClientHandler();

    //    clientHandler.user = new NetworkUser();
    //    clientHandler.user.userType = UserType.client;

    //    clientHandler.state = new StateObject();
    //    clientHandler.state.workSocket = handler;

    //    ClientsManager.instance.connectedClients.Add(clientHandler);
    //    ClientsManager.instance.pendingAvatarsAdd.Add(clientHandler);
    //    UnityEngine.Debug.Log("\nNew client connected");

    //    //clientHandler.Receive();
    //    UnityEngine.Debug.Log("Requesting user Login");

    //    NetworkRequest request = new NetworkRequest();
    //    request.sender = ServerManager.instance.server;
    //    request.requestType = RequestType.login;

    //    clientHandler.Send(request);
    //    clientHandler.Receive();
    //}

    //public static void ShutdownSocket(Socket socket)
    //{
    //    socket.Shutdown(SocketShutdown.Both);
    //    socket.Close();
    //}
}