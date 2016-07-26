using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;

using UnityEngine;

namespace HCP.Requests
{
    public class FindElementRequest : JobRequest
    {
        public string Strategy { get { return Data["strategy"]; } }
        public string Selector { get { return Data["selector"]; } }
        public string Context { get { return Data["context"]; } }
        public bool Multiple { get { return Data["multiple"].AsBool; } }

        public FindElementRequest(string json) : base(json)
        {
        }

        protected Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null) return type;
            }
            return null ;
        }
        
        protected JobResponse ProcessSingle()
        {
            string elementId = null;
            
            switch(this.Strategy)
            {
                case "name":
                    elementId = GameObject.Find(this.Selector).GetComponent<Element>().Id;
                    break;
                case "id":
                    elementId = GetElementById(this.Selector).Id;
                    break;
                case "tag name":
                    elementId = GameObject.FindGameObjectWithTag(this.Selector).GetComponent<Element>().Id;
                    break;
                case "class name":
                    var type = this.GetType(this.Selector);
                    elementId = ((GameObject)GameObject.FindObjectOfType(type)).GetComponent<Element>().Id;
                    break;
                case "xpath":
                    throw new NotImplementedException("FindElement - Do not currently support xpath");

                default:
                    throw new ArgumentException("Find strategy type unsupported: " + this.Strategy);
            }

            return Responses.JSONResponse.FromObject(new { ELEMENT = elementId });
        }

        protected JobResponse ProcessMultiple()
        {
            string[] elementIds = null;
            
            switch(this.Strategy)
            {
                case "name":
                    elementIds = GameObject.FindObjectsOfType<Element>().Where(e => e.gameObject.name == this.Selector).Select(e => e.Id).ToArray();
                    break;
                case "id":
                    throw new ArgumentException("FindElement - You cannot find multiple of the same id");
                case "tag name":
                    elementIds = GameObject.FindGameObjectsWithTag(this.Selector).Select(g => g.GetComponent<Element>().Id).ToArray();
                    break;
                case "class name":
                    var type = this.GetType(this.Selector);
                    elementIds = GameObject.FindObjectsOfType(type).Select(o => ((GameObject)o).GetComponent<Element>().Id).ToArray();
                    break;
                case "xpath":
                    throw new ArgumentException("FindElement - You cannot find multiple of the same xpath");

                default:
                    throw new ArgumentException("Find strategy type unsupported: " + this.Strategy);
            }

            // The JSON class I'm using kinda sucks, so just building this out as a string ATM
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            for(int i = 0; i < elementIds.Length; i++)
            {
                builder.Append(String.Format("{ ELEMENT: %1 }%2", elementIds[i], i < elementIds.Length-1 ? "," : ""));
            }
            builder.Append("]");

            return new Responses.StringResponse(builder.ToString());
        }


        public override JobResponse Process()
        {
            if(this.Multiple)
            {
                return this.ProcessMultiple();
            }
            else
            {
                return this.ProcessSingle();
            }
        }
    }
}
