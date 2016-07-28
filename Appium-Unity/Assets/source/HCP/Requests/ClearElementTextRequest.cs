using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;

namespace HCP.Requests
{
    public class ClearElementTextRequest : JobRequest
    {
        public string Id { get { return Data["elementId"]; } }
        
        public ClearElementTextRequest(JSONClass json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var element = JobRequest.GetElementById(this.Id);
            element.GetComponent<UnityEngine.UI.Text>().text = "";

            return new Responses.StringResponse();
        }
    }
}
