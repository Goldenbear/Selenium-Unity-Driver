using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.source.Communicate
{
    public class Listener : IListener
    {
        public event ListenerStartedEventHandler Started;
        public event LIstenerStoppedEventHandler Stopped;

        public void AddResponder(IResponder responder)
        {
            throw new NotImplementedException();
        }

        public void Start(string uri)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
