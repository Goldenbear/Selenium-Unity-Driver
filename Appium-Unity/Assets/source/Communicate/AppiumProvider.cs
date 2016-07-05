//////////////////////////////////////////////////////////////////////////
/// @file	AppiumProvider.cs
///
/// @author
///
/// @brief	Unity C# script template. To install copy this file into /Applications/<Unity Folder>/Unity.app>/Contents/Resources/ScriptTemplates.
///
/// @note 	Copyright 2015 Hutch Games Ltd. All rights reserved.
//////////////////////////////////////////////////////////////////////////

/************************ EXTERNAL NAMESPACES ***************************/

using UnityEngine;																// Unity 			(ref http://docs.unity3d.com/Documentation/ScriptReference/index.html)
using System;																	// String / Math 	(ref http://msdn.microsoft.com/en-us/library/system.aspx)
using System.Collections;														// Queue 			(ref http://msdn.microsoft.com/en-us/library/system.collections.aspx)
using System.Collections.Generic;												// List<> 			(ref http://msdn.microsoft.com/en-us/library/system.collections.generic.aspx)

using System.Net;
using System.Net.Sockets;                                                       // TcpListener      (ref https://msdn.microsoft.com/en-us/library/system.net.sockets.tcplistener.aspx)
using System.Threading;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

/************************ REQUIRED COMPONENTS ***************************/

/************************** THE SCRIPT CLASS ****************************/

//////////////////////////////////////////////////////////////////////////
/// @brief	AppiumProvider class.
//////////////////////////////////////////////////////////////////////////
public class AppiumProvider : MonoBehaviour
{
	/*****************************S* CONSTANTS *******************************/
	
	/***************************** SUB-CLASSES ******************************/
	
	/***************************** GLOBAL DATA ******************************/
    
    // Thread signal.
    public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

	/**************************** GLOBAL METHODS ****************************/

    // Accept one client connection asynchronously.
    public static void DoBeginAcceptTcpClient(TcpListener listener)
    {
        // Set the event to nonsignaled state.
        tcpClientConnected.Reset();

        // Start to listen for connections from a client.
        Console.WriteLine("Waiting for a connection...");

        // Accept the connection. 
        // BeginAcceptSocket() creates the accepted socket.
        listener.BeginAcceptTcpClient(
            new AsyncCallback(DoAcceptTcpClientCallback), 
            listener);

        // Wait until a connection is made and processed before 
        // continuing.
        tcpClientConnected.WaitOne();
    }

    // Process the client connection.
    public static void DoAcceptTcpClientCallback(IAsyncResult ar) 
    {
        // Get the listener that handles the client request.
        TcpListener listener = (TcpListener) ar.AsyncState;

        // End the operation and display the received data on 
        // the console.
        TcpClient client = listener.EndAcceptTcpClient(ar);

        // Process the connection here. (Add the client to a
        // server table, read data, etc.)
        Console.WriteLine("Client connected completed");

        // Signal the calling thread to continue.
        tcpClientConnected.Set();

    }

    public static void Run(string ip, int port)
    {
        TcpListener server = null;
        try
        {
            // TcpListener server = new TcpListener(port);
            server = new TcpListener(IPAddress.Parse(ip), port);

            // Start listening for client requests.
            server.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            // Enter the listening loop.
            while (true)
            {
             //   Debug.Log("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
              //  Debug.Log("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Debug.Log(String.Format("Received: {0}", data));

                    // Process the data sent by the client.
                    data = data.ToUpper();

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Debug.Log(String.Format("Sent: {0}", data));
                }

                // Shutdown and end connection
                client.Close();
            }
        }
        catch (SocketException e)
        {
            Debug.Log(String.Format("SocketException: {0}", e));
        }
        finally
        {
            // Stop listening for new clients.
            server.Stop();
        }
    }

    /***************************** PUBLIC DATA ******************************/
    public string m_ip;
    public Int32 m_port;


	/***************************** PRIVATE DATA *****************************/	
    private Thread m_listenerThread;


    /***************************** PROPERTIES *******************************/

    /***************************** PUBLIC METHODS ***************************/

    /**************************** PRIVATE METHODS ***************************/

	//////////////////////////////////////////////////////////////////////////
	/// @brief Initialise class after construction.
	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
        m_listenerThread = new Thread( () => Run(m_ip, m_port) );
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Everything is awake, script is about to start running.
	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
        if (!HttpListener.IsSupported)
        {
            Debug.Log ("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            return;
        }

        // See: https://issuetracker.unity3d.com/issues/debug-running-project-with-attached-debugger-causes-socket-exception-if-socket-is-in-another-thread
        m_listenerThread.Start();
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief 	Update one time step.
	//////////////////////////////////////////////////////////////////////////
	private void Update()
	{
	}
}

