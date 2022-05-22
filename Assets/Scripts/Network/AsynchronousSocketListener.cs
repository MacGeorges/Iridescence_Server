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
    public static ManualResetEvent allDone = new ManualResetEvent(false);

    public static void StartListening()
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                allDone.Reset();

                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                allDone.WaitOne();
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
        }
        ShutdownSocket(listener);
        UnityEngine.Debug.Log("\nServer Stopped");
    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        UnityEngine.Debug.Log("AcceptCallback");
        allDone.Set();

        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        ClientHandler clientHandler = new ClientHandler();

        clientHandler.user = new NetworkUser();
        clientHandler.user.userType = UserType.client;

        clientHandler.state = new StateObject();
        clientHandler.state.workSocket = handler;

        ClientsManager.instance.connectedClients.Add(clientHandler);
        ClientsManager.instance.pendingAvatarsAdd.Add(clientHandler);
        UnityEngine.Debug.Log("\nNew client connected");

        //clientHandler.Receive();
        UnityEngine.Debug.Log("Requesting user Login");

        NetworkRequest request = new NetworkRequest();
        request.sender = ServerManager.instance.server;
        request.requestType = RequestType.login;

        clientHandler.Send(request);
        clientHandler.Receive();
    }

    public static void ShutdownSocket(Socket socket)
    {
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}