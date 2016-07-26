using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;

using UnityEngine;
using UnityEngine.UI;

namespace HCP.Requests
{
    public class GetElementTextRequest : JobRequest
    {
        public string Id { get { return Data["elementId"]; } }
        
        public GetElementTextRequest(string json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var element = JobRequest.GetElementById(this.Id);

            return new Responses.StringResponse(element.GetComponent<Text>().text);
        }
    }
}
