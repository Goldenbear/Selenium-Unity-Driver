using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;

namespace HCP.Responses
{
    public class ErrorResponse : JobResponse
    {
        public ErrorResponse() : base("")
        {
            this.Status = EStatus.UnhandledError;
        }        
    }
}
