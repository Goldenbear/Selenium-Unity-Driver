using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;

using UnityEngine.UI;

namespace HCP.Requests
{
    public class SetElementTextRequest : JobRequest
    {
        public string Id { get { return Data["elementId"]; } }
        public string Text { get { return Data["text"]; } }
        
        public SetElementTextRequest(JSONClass json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var element = JobRequest.GetElementById(this.Id);
            element.GetComponent<Text>().text = this.Text;

            return new Responses.StringResponse();
        }

    }
}
