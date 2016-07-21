using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.source.Jobs
{
    interface IJobRequestQueue
    {
        Queue<IJobRequest> Requests { get; }
    }
}
