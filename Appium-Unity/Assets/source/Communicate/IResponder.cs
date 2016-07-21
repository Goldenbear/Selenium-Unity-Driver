using Assets.source.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets.source.Communicate
{
    public delegate IJobRequest JobRequestBuilder(string requestBody);

    public interface IResponder
    {
        Regex CommandPattern { get; }
        JobRequestBuilder RequestBuilder { get; }

        bool AcceptsRequest(string command);
        IJobResponse RespondTo(string command);
    }
}
