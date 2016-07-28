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
    public class ComplexTapRequest : JobRequest
    {
        public float X { get { return Data["x"].AsFloat; } }
        public float Y { get { return Data["y"].AsFloat; } }
        
        public ComplexTapRequest(JSONClass json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var server = GameObject.FindObjectOfType<HCP.Server>();

            var ptr = new PointerEventData(EventSystem.current);
            ptr.position = ptr.pressPosition = new Vector2(X, Y);
            ExecuteEvents.Execute(server.gameObject, ptr, ExecuteEvents.pointerClickHandler);

            return new Responses.StringResponse();
        }
    }
}
