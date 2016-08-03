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
        
        public GetElementTextRequest(JSONClass json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var element = JobRequest.GetElementById(this.Id);

            var textItem = element.GetComponent<Text>();
            var inputItem = element.GetComponent<InputField>();
            string text = "";

            if(textItem != null) text = textItem.text;
            else text = inputItem.text;

            return new Responses.StringResponse(text);
        }
    }
}
