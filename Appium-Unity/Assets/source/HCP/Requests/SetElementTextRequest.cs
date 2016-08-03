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
        public bool Replace { get { return Data["replace"].AsBool; } }
        public bool Unicode { get { return Data["unicodeKeyboard"].AsBool; } }
        
        public SetElementTextRequest(JSONClass json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var element = JobRequest.GetElementById(this.Id);

            var textItem = element.GetComponent<Text>();
            var inputItem = element.GetComponent<InputField>();

            if(this.Replace)
            {
                if(textItem != null) textItem.text = this.Text;
                else inputItem.text = this.Text;
            }
            else
            {
                if(textItem != null) textItem.text += this.Text;
                else inputItem.text += this.Text;
            }

            return new Responses.StringResponse();
        }

    }
}
