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

        public static JSONResponse FromArray(object[] input)
        {
            return new JSONResponse(input);
        }


        protected JSONNode m_sValue;

        public JSONResponse()
        {
            this.m_sValue = new JSONClass();
        }

        protected JSONResponse(object o) : this()
        {
            Type type = o.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(o, null);

                m_sValue.Add(prop.Name, prop.GetValue(o, null).ToString());
            }
        }

        public override string ToString()
        {
            return this.m_sValue.ToString();
        }
    }
}
