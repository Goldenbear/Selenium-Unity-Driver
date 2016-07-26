using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;

using UnityEngine;

namespace HCP.Requests
{
    public class GetElementSizeRequest : JobRequest
    {
        public string Id { get { return Data["elementId"]; } }
        
        public GetElementSizeRequest(string json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var element = JobRequest.GetElementById(this.Id);
            Bounds bounds = element.GetComponent<Renderer>().bounds;

            return Responses.JSONResponse.FromObject(new { width = bounds.extents.x, height = bounds.extents.y, depth = bounds.extents.z });
                // Note that appium has no concept of depth, but passing it anyways
        }
    }
}
