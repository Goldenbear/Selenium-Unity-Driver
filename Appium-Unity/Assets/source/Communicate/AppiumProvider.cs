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
using System.IO;
using SimpleJSON;
using UniRx;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

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
public class AppiumProvider : MonoBehaviour, IDisposable
{
	/****************************** CONSTANTS *******************************/
	
	/***************************** SUB-CLASSES ******************************/
	
	/***************************** GLOBAL DATA ******************************/
    
    // Thread signal.
    private static ManualResetEvent g_httpClientConnected = new ManualResetEvent(false);

	/**************************** GLOBAL METHODS ****************************/

    public class Job
    {
        public bool complete;
        public string request;
        public string result;
    }

    // Accept one client connection asynchronously.
    private static void DoBeginAcceptHttpClient(AppiumProvider provider, HttpListener listener)
    {
        // Set the event to nonsignaled state.
        g_httpClientConnected.Reset();

        // Start to listen for connections from a client.
        Debug.Log("Waiting for a connection...");

        // Accept the connection. 
        IAsyncResult result = listener.BeginGetContext(
            new AsyncCallback(DoAcceptHttpClientCallback), new HttpPair{ provider = provider, listener = listener } );

        Debug.Log("Waiting for request to be processed asyncronously...");

        // Wait until a connection is made and processed before 
        // continuing.
        g_httpClientConnected.WaitOne();

        Debug.Log("Request processed asyncronously.");
    }

    public struct HttpPair
    {
        public AppiumProvider provider;
        public HttpListener listener;
    }

    // Process the client connection.
    private static void DoAcceptHttpClientCallback(IAsyncResult ar) 
    {
        HttpPair dobject = (HttpPair)ar.AsyncState;

        // Get the appium provider that handles the client request.
        AppiumProvider provider = (AppiumProvider) dobject.provider;

        // Get the listener that handles the client request.
        HttpListener listener = (HttpListener) dobject.listener;

        // Call EndGetContext to complete the asynchronous operation.
        HttpListenerContext context = listener.EndGetContext(ar);

        // Obtain the request.
        HttpListenerRequest request = context.Request;

        // Obtain a response object.
        HttpListenerResponse response = context.Response;
        string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";

        Debug.Log("Request received");

        if(request.RawUrl.StartsWith("/alive"))
        {
            // Construct a response.
            responseString = "Appium-HCP Socket Server Ready";
        }
        else if(request.RawUrl.StartsWith("/action"))
            // https://github.com/SeleniumHQ/selenium/wiki/JsonWireProtocol
        {
            string text;
            using (var reader = new StreamReader(request.InputStream,
                                                    request.ContentEncoding))
            {
                text = reader.ReadToEnd();
            }

            Job job = new Job { complete = false, request = text, result = "" };
            provider.jobs.Enqueue(job);

            while(job.complete == false)
            {
                // spin
                // TODO: timeout
            }

            responseString = job.result;
        }

        // Construct a response.
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
    private static void Run(AppiumProvider provider)
    {
        if (!HttpListener.IsSupported)
        {
            throw new InvalidOperationException("The HttpListener class is unsupported!  I will not be able to provide Appium with data.");
        }

        HttpListener server = null;
        try
        {
            string[] prefixes = {
                provider.m_sUri + "/alive/",
                provider.m_sUri + "/action/" };    // TODO: Append endpoints

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
                DoBeginAcceptHttpClient(provider, server);
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
	/// @brief Returns a collection of "exposed" game objects.  These are game 
    /// objects that have a "UniqueId" associated to them.
	//////////////////////////////////////////////////////////////////////////
    private UniqueId[] GetExposedGameObjects()
    {
        return GameObject.FindObjectsOfType<UniqueId>();
    }

    Queue<Job> jobs;

	//////////////////////////////////////////////////////////////////////////
	/// @brief Initialise class after construction.
	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
        jobs = new Queue<Job>();
        m_listenerThread = new Thread( () => Run(this) );
	}

    

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Everything is awake, script is about to start running.
	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
        UniqueId[] gameobjects = this.GetExposedGameObjects();
        m_listenerThread.Start();
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief 	Update one time step.
	//////////////////////////////////////////////////////////////////////////
	private void Update()
	{
        if(jobs.Count > 0)
        {
            Job job = jobs.Dequeue();

            doJob(job);
        }
	}

    protected void doJob(Job job)
    {
        try
        { 
            // TODO: Place in method
            UniqueId[] gameobjects = this.GetExposedGameObjects();

            Debug.Log(job.request);


            var data = JSON.Parse(job.request);

            Debug.Log(data);

            string command = data["cmd"].Value;

            if(command == "action")
            {
                var parameters = data["params"];

                switch(data["action"].Value)
                {
                    case "find":
                        job.result = this.doJob_find(parameters);
                        break;

                    case "element:click":
                        job.result = this.doJob_element_click(parameters);
                        break;
                }
            }
        }
        finally
        {
            job.complete = true;
        }
    }

    public void Dispose()
    {
        Debug.Log("Disposing");
        m_listenerThread.Join();
    }
        
    protected string doJob_find(JSONNode data)
    {
        string result = "error";

        if(data["strategy"].Value == "name")
        {
            string name = data["selector"].Value;

            var go = GameObject.Find(name);
            if(go != null)
            {
                result = go.GetComponent<UniqueId>().AsJson();
            }
        }

        return result;
    }

    protected string doJob_element_click(JSONNode data)
    {
        string id = data["elementId"].Value;

        UniqueId[] things = GameObject.FindObjectsOfType<UniqueId>();
        var toClick = from item in things
                           where item.m_sUniqueGuid == id
                           select item;
                
        var ptr = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(toClick.First().gameObject, ptr, ExecuteEvents.submitHandler);

        return "ok";
    }
}