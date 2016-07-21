using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.source.Jobs;

namespace Assets.source.Communicate
{
    public class Responder : IResponder
    {
        public Regex CommandPattern
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public JobRequestBuilder RequestBuilder
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool AcceptsRequest(string command)
        {
            throw new NotImplementedException();
        }

        public IJobResponse RespondTo(string command)
        {
            throw new NotImplementedException();
        }

        public Responder(Regex commandPattern, JobRequestBuilder requestBuilder)
        {

        }
    }

    public class JobRequest : IJobRequest
    {
        public IJobRequest FromJSON(string json)
        {
            throw new NotImplementedException();
        }
    }

    public class HCP
    {
        private Listener m_listener = null;
        public  Listener Listener { get { return m_listener ?? ( m_listener = new Listener() ); } }

        public HCP()
        {
            // Create request builders
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
            this.Listener.AddResponder(new Responder(new Regex(@""), (json) => { return new JobRequest().FromJSON(json); }));
        }
    }
}
