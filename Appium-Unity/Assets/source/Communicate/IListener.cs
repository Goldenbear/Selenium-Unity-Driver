using Assets.source.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets.source.Communicate
{
    public delegate void ListenerStartedEventHandler();
    public delegate void LIstenerStoppedEventHandler();

    public interface IListener
    {
        void AddResponder(IResponder responder);

        void Start(string uri);
        void Stop();
        
        event ListenerStartedEventHandler Started;
        event LIstenerStoppedEventHandler Stopped;
    }
}
