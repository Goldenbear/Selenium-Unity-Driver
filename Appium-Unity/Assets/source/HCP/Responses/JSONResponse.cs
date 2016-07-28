using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using HCP;
using HCP.SimpleJSON;

namespace HCP.Responses
{
    public class JSONResponse : JobResponse
    {
        public static JSONResponse FromObject(object input)
        {
            return new JSONResponse(input);
        }

        public static JSONResponse FromArray(object[] input, ArrayEntryFormatter builder = null)
        {
            return new JSONResponse(input, builder);
        }

        protected JSONResponse(object o) : base(o)
        {
        }

        protected JSONResponse(object[] a, ArrayEntryFormatter builder) : base(a, builder)
        {
        }
    }
}
