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
    // Thread signal.  
    public static ManualResetEvent allDone = new ManualResetEvent(false);

    public static void StartListening()
    {
        // Establish the local endpoint for the socket.  
        // The DNS name of the computer  
        // running the listener is "host.contoso.com".  
        IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.  
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                UnityEngine.Debug.Log("=======================");
                UnityEngine.Debug.Log("Starting listening Loop");
                // Set the event to nonsignaled state.  
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.  
                UnityEngine.Debug.Log("Inbound connection");
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();
                UnityEngine.Debug.Log("Ending listening Loop");
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
        }

        UnityEngine.Debug.Log("\nServer Stopped");
        Console.Read();

    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        UnityEngine.Debug.Log("AcceptCallback");
        // Signal the main thread to continue.  
        allDone.Set();

        // Get the socket that handles the client request.  
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        ClientHandler clientHandler = new ClientHandler();
        //clientHandler.socket = handler;

        // Create the state object.  
        clientHandler.state = new StateObject();
        clientHandler.state.workSocket = handler;

        ClientsManager.instance.connectedClients.Add(clientHandler);
        UnityEngine.Debug.Log("New client added to list");

        clientHandler.Receive();
    }
}