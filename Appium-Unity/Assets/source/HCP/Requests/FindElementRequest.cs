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

        public FindElementRequest(JSONClass json) : base(json)
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
                    elementId = Resources.FindObjectsOfTypeAll<Element>().First(e => e.gameObject.name == this.Selector).Id;
                    break;
                case "id":
                    elementId = GetElementById(this.Selector).Id;
                    break;
                case "tag name":
                    elementId = Resources.FindObjectsOfTypeAll<Element>().First(e => e.gameObject.tag == this.Selector).Id;
                    break;
                case "class name":
                    var type = this.GetType(this.Selector);
                    elementId = Resources.FindObjectsOfTypeAll<Element>().First(e => e.GetComponent(type) != null).Id;
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
                    elementIds = Resources.FindObjectsOfTypeAll<Element>().Where(e => e.gameObject.name == this.Selector).Select(e => e.Id).ToArray();
                    break;
                case "id":
                    throw new ArgumentException("FindElement - You cannot find multiple of the same id");
                case "tag name":
                    elementIds = Resources.FindObjectsOfTypeAll<Element>().Where(e => e.gameObject.tag == this.Selector).Select(e => e.Id).ToArray();
                    break;
                case "class name":
                    var type = this.GetType(this.Selector);
                    elementIds = Resources.FindObjectsOfTypeAll<Element>().Where(e => e.GetComponent(type) != null).Select(e => e.Id).ToArray();
                    break;
                case "xpath":
                    throw new NotImplementedException("FindElement - Do not currently support xpath");

                default:
                    throw new ArgumentException("Find strategy type unsupported: " + this.Strategy);
            }
            
            return Responses.JSONResponse.FromArray(elementIds, (item) => { return new { ELEMENT = item }; });
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
