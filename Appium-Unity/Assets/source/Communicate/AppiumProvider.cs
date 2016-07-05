/////////////////////////////////////////////////////////////////////////
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
/// @brief	AppiumProvider class.  Creates a threaded HttpListener to 
/// respond to requests to provide Unity GamObject data.  Was originally 
/// a lighter-weight TCP socket listener, but a Unity bug cause it to be 
/// redesigned:
/// 
/// See: https://issuetracker.unity3d.com/issues/debug-running-project-with-attached-debugger-causes-socket-exception-if-socket-is-in-another-thread
/// 
/// Supported endpoints:
/// /api/guids - returns a JSON formatted list of all appium objects with
///              a guid.
///            
//////////////////////////////////////////////////////////////////////////
public class AppiumProvider : MonoBehaviour
{
	/****************************** CONSTANTS *******************************/
	
	/***************************** SUB-CLASSES ******************************/
	
	/***************************** GLOBAL DATA ******************************/
    
    // Thread signal.
    private static ManualResetEvent g_httpClientConnected = new ManualResetEvent(false);

	/**************************** GLOBAL METHODS ****************************/

    // Accept one client connection asynchronously.
    private static void DoBeginAcceptHttpClient(HttpListener listener)
    {
        // Set the event to nonsignaled state.
        g_httpClientConnected.Reset();

        // Start to listen for connections from a client.
        Console.WriteLine("Waiting for a connection...");

        // Accept the connection. 
        IAsyncResult result = listener.BeginGetContext(
            new AsyncCallback(DoAcceptHttpClientCallback), listener);

        Debug.Log("Waiting for request to be processed asyncronously.");

        // Wait until a connection is made and processed before 
        // continuing.
        g_httpClientConnected.WaitOne();

        Debug.Log("Request processed asyncronously.");
    }

    // Process the client connection.
    private static void DoAcceptHttpClientCallback(IAsyncResult ar) 
    {
        // Get the listener that handles the client request.
        HttpListener listener = (HttpListener) ar.AsyncState;

        // Call EndGetContext to complete the asynchronous operation.
        HttpListenerContext context = listener.EndGetContext(ar);

        // Obtain the request.
        HttpListenerRequest request = context.Request;

        // Obtain a response object.
        HttpListenerResponse response = context.Response;

        // Construct a response.
        string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

        // Get a response stream and write the response to it.
        response.ContentLength64 = buffer.Length;
        System.IO.Stream output = response.OutputStream;
        output.Write(buffer,0,buffer.Length);

        // You must close the output stream.
        output.Close();

        // Signal the calling thread to continue.
        g_httpClientConnected.Set();
    }

    // The main thread loop
    private static void Run(string uri)
    {
        if (!HttpListener.IsSupported)
        {
            throw new InvalidOperationException("The HttpListener class is unsupported!  I will not be able to provide Appium with data.");
        }

        HttpListener server = null;
        try
        {
            string[] prefixes = { uri };    // TODO: Append endpoints

            // TcpListener server = new TcpListener(port);
            server = new HttpListener();

            // Add the prefixes.
            foreach (string s in prefixes)
            {
                server.Prefixes.Add(s);
            }

            // Start listening for client requests.
            server.Start();            

            while(true)
            {
                DoBeginAcceptHttpClient(server);
            }
        }
        catch (Exception e)
        {
            Debug.Log(String.Format("Unknown Exception: {0}", e));
        }
        finally
        {
            // Stop listening for new clients.
            server.Close();
            server.Stop();
        }
    }

    /***************************** PUBLIC DATA ******************************/
    public string m_sUri;


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
        m_listenerThread = new Thread( () => Run(m_sUri) );
	}

    

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Everything is awake, script is about to start running.
	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
        m_listenerThread.Start();
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief 	Update one time step.
	//////////////////////////////////////////////////////////////////////////
	private void Update()
	{
	}
}