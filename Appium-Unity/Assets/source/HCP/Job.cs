using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HCP
{
    public class Job : IDisposable
    {
        public enum EState
        {
            IDLE,
            RUNNING,
            COMPLETE,
            ERROR,
        };

        public JobRequest Request { get; set; }
        public JobResponse Response { get; set; }
        public EState State { get; set; }
        public bool IsComplete { get { return this.State == EState.COMPLETE || this.State == EState.ERROR; } }
        public ManualResetEvent m_processReset = new ManualResetEvent(false);

        public void Await()
        {
            m_processReset.WaitOne();
        }

        public virtual void Process()
        {
            if(State == EState.IDLE)
            {
                m_processReset.Reset();
            }
            State = EState.RUNNING;

            try
            {
                Response = Request.Process();

                if (Response != null)
                    // A process will return a response when it is complete, null otherwise
                    // I don't really like this but it will have to do for now.  
                {
                    State = EState.COMPLETE;
                    Response.Status = JobResponse.EStatus.Success;
                    m_processReset.Set();
                }
            }
            catch(Exception e)
            {
                State = EState.ERROR;
                Response = new Responses.StringResponse(e.Message);
                Response.Status = JobResponse.EStatus.UnhandledError;
            }
            finally
            {
            }
        }

        public void Dispose()
        {
            this.m_processReset.Set();
            this.m_processReset.Close();
        }
    }
}
