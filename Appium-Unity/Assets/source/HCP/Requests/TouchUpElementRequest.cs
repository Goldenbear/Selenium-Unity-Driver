using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;

using UnityEngine.EventSystems;
using UnityEngine;

namespace HCP.Requests
{
    public class TouchUpElementRequest : JobRequest
    {
        public string Id { get { return Data["elementId"]; } }
        public float X { get { return Data["x"].AsFloat; } }
        public float Y { get { return Data["y"].AsFloat; } }
        
        public TouchUpElementRequest(JSONClass json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var toTouch = JobRequest.GetElementById(this.Id);

            var ptr = new PointerEventData(EventSystem.current);
            ptr.position = new Vector2(X, Y);
            ExecuteEvents.Execute(toTouch.gameObject, ptr, ExecuteEvents.pointerUpHandler);

            return new Responses.StringResponse();
        }
    }
}
