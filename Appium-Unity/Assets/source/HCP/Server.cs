using HCP.SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

using UnityEngine;

namespace HCP
{
    public delegate void ListenerStartedEventHandler();
    public delegate void LIstenerStoppedEventHandler();


    //////////////////////////////////////////////////////////////////////////
    /// @brief	Listener class.  Creates a threaded HttpListener to 
    /// respond to requests to provide Unity GamObject data.  Was originally 
    /// a lighter-weight TCP socket listener, but a Unity bug cause it to be 
    /// redesigned:
    /// 
    /// See: https://issuetracker.unity3d.com/issues/debug-running-project-with-attached-debugger-causes-socket-exception-if-socket-is-in-another-thread
    ///            
    //////////////////////////////////////////////////////////////////////////
    [AddComponentMenu("HCP/Server")]
    public class Server : MonoBehaviour
    {
        /****************************** CONSTANTS *******************************/
	
	    /***************************** SUB-CLASSES ******************************/
	
	    /***************************** GLOBAL DATA ******************************/
    
        // Thread signal.
        private static ManualResetEvent g_httpClientConnected = new ManualResetEvent(false);

	    /**************************** GLOBAL METHODS ****************************/
        
        //////////////////////////////////////////////////////////////////////////
        /// @brief  Accept one client connection asynchronously.
        //////////////////////////////////////////////////////////////////////////
        private static void DoBeginAcceptHttpClient(Server server)
        {
            // Set the event to nonsignaled state.
            g_httpClientConnected.Reset();

            // Start to listen for connections from a client.
            Console.WriteLine("Waiting for a connection...");

            // Accept the connection. 
            IAsyncResult result = server.Listener.BeginGetContext(
                new AsyncCallback(DoAcceptHttpClientCallback), server );

            Console.WriteLine("Waiting for request to be processed asyncronously...");

            // Wait until a connection is made and processed before continuing.
            g_httpClientConnected.WaitOne();

            Console.WriteLine("Request processed asyncronously.");
        }
        
        //////////////////////////////////////////////////////////////////////////
        /// @brief  Process the client connection.
        //////////////////////////////////////////////////////////////////////////
        private static void DoAcceptHttpClientCallback(IAsyncResult ar) 
        {
            Server server = (Server)ar.AsyncState;

            // Get the listener that handles the client request.
            HttpListener listener = (HttpListener) server.Listener;

            // Call EndGetContext to complete the asynchronous operation.
            HttpListenerContext context = listener.EndGetContext(ar);

            // Obtain the request.
            HttpListenerRequest request = context.Request;

            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            string responseString = null;

            Console.WriteLine("Request received");

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

                try
                {
                    Job job = server.QueueActionRequest(text);

                    while(
                        job.State == Job.EState.IDLE ||
                        job.State == Job.EState.RUNNING)
                    {
                        // spin
                    }

                    responseString = job.Response;
                }
                catch
                {
                }
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
        private static void Run(Server server)
        {
            if (!HttpListener.IsSupported)
            {
                throw new InvalidOperationException("The HttpListener class is unsupported!  I will not be able to provide Appium with data.");
            }

            HttpListener listener = server.Listener;

            try
            {
                // Start listening for client requests.
                listener.Start();            

                while(true)
                {
                    DoBeginAcceptHttpClient(server);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Unknown Exception: {0}", e));
            }
            finally
            {
                // Stop listening for new clients.
                listener.Close();
                listener.Stop();
            }
        }

        /***************************** PUBLIC DATA ******************************/
        public event ListenerStartedEventHandler Started;
        public event LIstenerStoppedEventHandler Stopped;
        
        public HttpListener Listener { get { return m_listener; } }
        public string ListenerURI { get { return m_sListenerURI; } }


	    /***************************** PRIVATE DATA *****************************/	
        private HttpListener m_listener;
        [SerializeField] private string m_sListenerURI;
        private Thread m_listenerThread;

        protected Dictionary<string, Type> m_requestCommands;
        protected Queue<Job> m_requestJobs;



        /***************************** PROPERTIES *******************************/

        /***************************** PUBLIC METHODS ***************************/
        #region API

        //////////////////////////////////////////////////////////////////////////
	    /// @brief 	The interface to queue a job to run on the main thread
	    //////////////////////////////////////////////////////////////////////////
	    public Job QueueActionRequest(string task)
        {
            Job job = new Job();

            var data = JSON.Parse(task);
            string command = data["cmd"].Value;
            if(command == "action")
            {
                string actionCommand = data["action"].Value;
                JSONNode parameters = data["params"];
                job.Request = (JobRequest)Activator.CreateInstance(m_requestCommands[actionCommand], parameters);
            }
            else
            {
                throw new ArgumentException("Cannot queue an action of unknown command type: " + command);
            }

            m_requestJobs.Enqueue(job);
            return job;
        }
        #endregion

        /**************************** PRIVATE METHODS ***************************/

        #region Utility
        private void AddActionHandler(string requestCommand, Type requestType)
        {
            if(requestType == typeof(JobRequest))
            {
                this.m_requestCommands.Add(requestCommand, requestType);
            }
            else
            {
                throw new ArgumentException("requestType must be of type JobRequest");
            }
        }
        #endregion


        #region Monobehavior Lifecycle
        //////////////////////////////////////////////////////////////////////////
        /// @brief Initialise class after construction.
        //////////////////////////////////////////////////////////////////////////
        private void Awake()
	    {
            // Create request builders
            this.AddActionHandler("find", typeof(Requests.FindElementRequest));
            this.AddActionHandler("element:getAttribute", typeof(Requests.GetElementAttributeRequest));
            this.AddActionHandler("element:setText", typeof(Requests.SetElementTextRequest));
            this.AddActionHandler("element:getText", typeof(Requests.GetElementTextRequest));
            this.AddActionHandler("element:click", typeof(Requests.ClickElementRequest));
            this.AddActionHandler("element:getLocation", typeof(Requests.GetElementLocationRequest));
            this.AddActionHandler("element:getSize", typeof(Requests.GetElementSizeRequest));
            this.AddActionHandler("element:touchLongClick", typeof(Requests.TouchLongClickElementRequest));
            this.AddActionHandler("element:touchDown", typeof(Requests.TouchDownElementRequest));
            this.AddActionHandler("element:touchUp", typeof(Requests.TouchUpElementRequest));
            this.AddActionHandler("element:click", typeof(Requests.ClickElementRequest));
            this.AddActionHandler("click", typeof(Requests.ComplexTapRequest)); 
            this.AddActionHandler("source", typeof(Requests.PageSourceRequest)); 

            // Prepare jobs queue
            m_requestJobs = new Queue<Job>();

            // Prepare request listener
            m_listener = new HttpListener();
            m_listener.Prefixes.Add(this.ListenerURI + "/alive/");
            m_listener.Prefixes.Add(this.ListenerURI + "/action/");

            m_listenerThread = new Thread( () => Run(this) );
	    }    

	    //////////////////////////////////////////////////////////////////////////
	    /// @brief	Everything is awake, script is about to start running.
	    //////////////////////////////////////////////////////////////////////////
	    private void Start()
	    {
            m_listenerThread.Start();
            if(this.Started != null) this.Started();
	    }

	    //////////////////////////////////////////////////////////////////////////
	    /// @brief 	Update one time step.
	    //////////////////////////////////////////////////////////////////////////
	    private void Update()
	    {
            if(m_requestJobs.Count > 0)
            {
                Job job = m_requestJobs.Peek();
                job.Process();

                if(job.IsComplete)
                {
                    m_requestJobs.Dequeue();
                }
            }
	    }

        //////////////////////////////////////////////////////////////////////////
	    /// @brief 	Called when destroyed.
	    //////////////////////////////////////////////////////////////////////////
        private void OnDestroy()
        {
            if(m_listenerThread != null) m_listenerThread.Join();
            if(this.Stopped != null) this.Stopped();
        }

        #endregion
    }
}
