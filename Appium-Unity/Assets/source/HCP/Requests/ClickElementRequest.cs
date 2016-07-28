using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;

using UnityEngine;
using UnityEngine.EventSystems;

namespace HCP.Requests
{
    public class ClickElementRequest : JobRequest
    {
        public string Id { get { return Data["elementId"]; } }
        public float X { get { return Data["x"].AsFloat; } }
        public float Y { get { return Data["y"].AsFloat; } }
        
        public ClickElementRequest(JSONClass json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var toClick = JobRequest.GetElementById(this.Id);

            var ptr = new PointerEventData(EventSystem.current);
            ptr.position = ptr.pressPosition = new Vector2(X, Y);
            ExecuteEvents.Execute(toClick.gameObject, ptr, ExecuteEvents.pointerClickHandler);

            return new Responses.StringResponse();
        }
    }
}
