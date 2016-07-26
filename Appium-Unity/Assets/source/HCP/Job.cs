using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HCP
{
    public class Job
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

        public virtual void Process()
        {
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
    }
}
