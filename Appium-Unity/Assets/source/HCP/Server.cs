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
    public delegate void ListenerStartedEventHandler ();
    public delegate void LIstenerStoppedEventHandler ();


    //////////////////////////////////////////////////////////////////////////
    /// @brief	Server class.  Creates a threaded HttpListener to 
    /// respond to requests to provide Unity GamObject data.  Was originally 
    /// a lighter-weight TCP socket listener, but a Unity bug cause it to be 
    /// redesigned:
    /// 
    /// See: https://issuetracker.unity3d.com/issues/debug-running-project-with-attached-debugger-causes-socket-exception-if-socket-is-in-another-thread
    ///            
    /// 
    /// The server spins a new thread and listens for a HTTPRequest.  When its
    /// received it makes sure its either an alive or action endpoint.
    /// Alive will always return its alive string, and is used by appium drivers
    /// to signal that actual HCP commands are ready.  You can also use it to
    /// determine when Unity is ready.
    /// Action has a command (cmd) and parameter (params) structure.  Commands
    /// are simple strings that are matched.  On match, they spawn a Job which 
    /// has a status, JobRequest, and JobResponse.  Jobs are added to the server
    /// job queue so that they can execute on the Main thread.  Jobs may take
    /// several frames to complete.  The server waits for its status to be
    /// in a complete state prior to responding to the HTTPRequest.
    //////////////////////////////////////////////////////////////////////////
    [AddComponentMenu ("HCP/Server")]
    public class Server : MonoBehaviour
    {
        /****************************** CONSTANTS *******************************/
	
        /***************************** SUB-CLASSES ******************************/
	
        /***************************** GLOBAL DATA ******************************/
    
        /**************************** GLOBAL METHODS ****************************/
        
                

        /***************************** PUBLIC DATA ******************************/
        public event ListenerStartedEventHandler Started;
        public event LIstenerStoppedEventHandler Stopped;

        public HttpListener Listener { get { return m_listener; } }

        public string ListenerURI { get { return m_sListenerURI; } }

        public bool ActiveAndEnabled { get { return m_bActiveAndEnabled; } }

        public bool AutoAddElementsOnStart = false;

        //////////////////////////////////////////////////////////////////////////
        /// @brief  The appium app expects iOS data to be scaled based on 
        /// whether or not it is a retina screen.  This is really silly but
        /// it is easier to deal with here than in their codebase.  So
        /// position data could be scaled when on an iOS device.  If we didn't do
        /// this then sending touch data to iOS would require two sets of coordinate
        /// systems to be maintained: HCP-Mode and Apium-Mode.  We instead choose
        /// to emulate Appium as much as possible
        //////////////////////////////////////////////////////////////////////////
        public static float DeviceScreenScalar
        {
            get {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    // This is taken from the AppiumInspectorScreenshotView.m file of Appium-Dot-App
                    // check for retina devices
                    if (Screen.width == 640 && Screen.height == 960)
                    {
                        // portrait 3.5" iphone with retina display
                        return 2.0f;
                    }
                    else if (Screen.width == 960 && Screen.height == 640)
                    {
                        // landscape 3.5" iphone with retina display
                        return 2.0f;
                    }
                    else if (Screen.width == 640 && Screen.height == 1136)
                    {
                        // portrait 4" iphone with retina display
                        return 2.0f;
                    }
                    else if (Screen.width == 1136 && Screen.height == 640)
                    {
                        // landscape 4" iphone with retina display
                        return 2.0f;
                    }
                    else if (Screen.width == 750 && Screen.height == 1334)
                    {
                        // portrait iphone 6
                        return 2.0f;
                    }
                    else if (Screen.width == 1334 && Screen.height == 750)
                    {
                        // landscape iphone 6
                        return 2.0f;
                    }
                    else if (Screen.width == 1242 && Screen.height == 2208)
                    {
                        // portrait iphone 6 plus
                        return 3.0f;
                    }
                    else if (Screen.width == 2208 && Screen.height == 1242)
                    {
                        // landscape iphone 6 plus
                        return 3.0f;
                    }
                    else if (Screen.width == 1536 && Screen.height == 2048)
                    {
                        // portrait ipad with retina display
                        return 2.0f;
                    }
                    else if (Screen.width == 2048 && Screen.height == 1536)
                    {
                        // landscape ipad with retina display
                        return 2.0f;
                    }
                }
                return 1;
            }
        }


        /***************************** PRIVATE DATA *****************************/
        private HttpListener m_listener;
        [SerializeField] private string m_sListenerURI;
        private Thread m_listenerThread;
        private bool m_bActiveAndEnabled;

        protected Dictionary<string, Type> m_requestCommands;
        protected Queue<Job> m_requestJobs;




        /***************************** PROPERTIES *******************************/

        /***************************** PUBLIC METHODS ***************************/

        #region API

        //////////////////////////////////////////////////////////////////////////
        /// @brief 	The interface to queue a job to run on the main thread
        //////////////////////////////////////////////////////////////////////////
        public Job QueueActionRequest (string task)
        {
            Job job = new Job ();

            var data = JSON.Parse (task);
            string command = data ["cmd"].Value;
            if (command == "action")
            {
                string actionCommand = data ["action"].Value;
                JSONNode parameters = data ["params"];

                var actionType = m_requestCommands [actionCommand];
                job.Request = (JobRequest)Activator.CreateInstance (actionType, parameters);
            }
            else
            {
                throw new ArgumentException ("Cannot queue an action of unknown command type: " + command);
            }

            m_requestJobs.Enqueue (job);
            return job;
        }

        #endregion

        /**************************** PRIVATE METHODS ***************************/

        //////////////////////////////////////////////////////////////////////////
        /// @brief  Process the client connection.
        //////////////////////////////////////////////////////////////////////////
        private void AcceptContext (HttpListenerContext context)
        {
            // Obtain the request.
            HttpListenerRequest request = context.Request;

            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            string responseString = new Responses.ErrorResponse ().ToJSON (0);

            Debug.Log ("Request received");

            if (request.RawUrl.StartsWith ("/alive"))
            {
                // Construct a response.
                responseString = "Appium-HCP Socket Server Ready";
            }
            else if (request.RawUrl.StartsWith ("/action"))
                // https://github.com/SeleniumHQ/selenium/wiki/JsonWireProtocol
            {
                string text;
                using (var reader = new StreamReader (request.InputStream,
                                        request.ContentEncoding))
                {
                    text = reader.ReadToEnd ();
                }

                var job = this.QueueActionRequest (text);
                job.Await ();
                responseString = job.Response.ToJSON (0);
            }

            // Construct a response.
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes (responseString);

            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write (buffer, 0, buffer.Length);

            // You must close the output stream.
            output.Close ();
        }

        private void Stop ()
        {
            Debug.Log ("Stopping HCP Server");
            this.Listener.Stop ();
            if (m_listenerThread != null)
            {
                m_listenerThread.Join ();
            }
            if (this.Stopped != null)
                this.Stopped ();
        }

        private void Close ()
        {
            Debug.Log ("Closing HCP Server");
            this.Listener.Close ();
        }

        // The main thread loop
        private void Run ()
        {
            Debug.Log ("Starting HCP Server");

            if (!HttpListener.IsSupported)
            {
                throw new InvalidOperationException ("The HttpListener class is unsupported!  I will not be able to provide Appium with data.");
            }

            try
            {
                // Start listening for client requests.
                this.Listener.Start ();

                while (this.ActiveAndEnabled)
                {
                    AcceptContext (this.Listener.GetContext ());
                }
            }
            catch (ObjectDisposedException)
            {
                // Intentionally not doing anything with the exception.
            }
            catch (Exception e)
            {
                Debug.LogException (e);
            }
            finally
            {
                // Stop listening for new clients.
                this.Listener.Stop ();
            }
        }

        #region Utility

        private void AddActionHandler (string requestCommand, Type requestType)
        {
            if (requestType.IsSubclassOf (typeof(JobRequest)))
            {
                this.m_requestCommands.Add (requestCommand, requestType);
            }
            else
            {
                throw new ArgumentException ("requestType must be of type JobRequest");
            }
        }

        #endregion


        #region Monobehavior Lifecycle

        //////////////////////////////////////////////////////////////////////////
        /// @brief Initialise class after construction.
        //////////////////////////////////////////////////////////////////////////
        private void Awake ()
        {
            // Create request builders
            this.m_requestCommands = new Dictionary<string, Type> ();
            this.AddActionHandler ("element:clearText", typeof(Requests.ClearElementTextRequest));
            this.AddActionHandler ("element:click", typeof(Requests.ClickElementRequest));
            this.AddActionHandler ("click", typeof(Requests.ComplexTapRequest)); 
            this.AddActionHandler ("find", typeof(Requests.FindElementRequest));
            this.AddActionHandler ("element:getAttribute", typeof(Requests.GetElementAttributeRequest));
            this.AddActionHandler ("element:getLocation", typeof(Requests.GetElementLocationRequest));
            this.AddActionHandler ("element:getSize", typeof(Requests.GetElementSizeRequest));
            this.AddActionHandler ("element:getText", typeof(Requests.GetElementTextRequest));
            this.AddActionHandler ("source", typeof(Requests.PageSourceRequest)); 
            this.AddActionHandler ("element:setText", typeof(Requests.SetElementTextRequest));

            // I don't think these touch handlers are needed.
            this.AddActionHandler ("element:touchDown", typeof(Requests.TouchDownElementRequest));
            this.AddActionHandler ("element:touchLongClick", typeof(Requests.TouchLongClickElementRequest));
            this.AddActionHandler ("element:touchMove", typeof(Requests.TouchMoveElementRequest));
            this.AddActionHandler ("element:touchUp", typeof(Requests.TouchUpElementRequest));

            // Prepare jobs queue
            m_requestJobs = new Queue<Job> ();

            // Prepare request listener
            m_listener = new HttpListener ();
            m_listener.Prefixes.Add (this.ListenerURI + "/alive/");
            m_listener.Prefixes.Add (this.ListenerURI + "/action/");


            m_bActiveAndEnabled = true;
        }

        //////////////////////////////////////////////////////////////////////////
        /// @brief	Everything is awake, script is about to start running.
        //////////////////////////////////////////////////////////////////////////
        private void Start ()
        {
            if (AutoAddElementsOnStart)
            {
                // Here we find all element compatible components and add elements to 
                // them automatically
                Type[] compatibleTypes = {
                    typeof(UnityEngine.UI.Text),
                    typeof(UnityEngine.UI.Button),
                    typeof(UnityEngine.UI.Toggle),
                    typeof(UnityEngine.UI.Slider),
                    typeof(UnityEngine.UI.InputField),
                    typeof(UnityEngine.Canvas),
                };

                compatibleTypes.All (type =>
                {
                    Resources.FindObjectsOfTypeAll (type).All (o =>
                    {
                        var c = (Component)o;

                        if (c.GetComponent<Element> () == null)
                        {
                            var e = c.gameObject.AddComponent<Element> ();
                        }
                        return true;
                    });
                    return true;
                });
            }
        }

        //////////////////////////////////////////////////////////////////////////
        /// @brief	
        //////////////////////////////////////////////////////////////////////////
        private void OnEnable ()
        {
            m_listenerThread = new Thread (() => Run ());
            m_listenerThread.Start ();
            if (this.Started != null)
                this.Started ();
        }

        //////////////////////////////////////////////////////////////////////////
        /// @brief	
        //////////////////////////////////////////////////////////////////////////
        private void OnDisable ()
        {
            this.Stop ();
        }

        //////////////////////////////////////////////////////////////////////////
        /// @brief 	Update one time step.
        //////////////////////////////////////////////////////////////////////////
        private void Update ()
        {
            m_bActiveAndEnabled = this.isActiveAndEnabled;  
            // Duplicate to access outside of main thread

            if (m_requestJobs.Count > 0)
            {
                var job = m_requestJobs.Peek ();

                try
                { 
                    job.Process ();
                }
                catch (Exception e)
                {
                    job.State = Job.EState.ERROR;
                }
                finally
                {
                    if (job.IsComplete)
                    {
                        m_requestJobs.Dequeue ();
                        job.Dispose ();
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        /// @brief 	Called when destroyed.
        //////////////////////////////////////////////////////////////////////////
        private void OnDestroy ()
        {
            this.Close ();
        }

        #endregion
    }
}
