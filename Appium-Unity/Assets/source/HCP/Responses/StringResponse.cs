using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;

namespace HCP.Responses
{
    public class StringResponse : JobResponse
    {
        public StringResponse() : base("")
        {
        }

        public StringResponse(string value) : base(value)
        {
        }
    }
}
