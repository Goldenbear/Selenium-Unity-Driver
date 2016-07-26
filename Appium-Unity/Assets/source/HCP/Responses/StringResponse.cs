using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;

namespace HCP.Responses
{
    public class StringResponse : JobResponse
    {
        protected string m_sValue;

        public StringResponse()
        {
            this.m_sValue = "";
        }

        public StringResponse(string value)
        {
            this.m_sValue = value;
        }

        public override string ToString()
        {
            return this.m_sValue.ToString();
        }
    }
}
