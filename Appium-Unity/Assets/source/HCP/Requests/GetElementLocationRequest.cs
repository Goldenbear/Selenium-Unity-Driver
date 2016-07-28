using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;

using UnityEngine;

namespace HCP.Requests
{
    public class GetElementLocationRequest : JobRequest
    {
        public string Id { get { return Data["elementId"]; } }
        
        public GetElementLocationRequest(JSONClass json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var element = JobRequest.GetElementById(this.Id);
            Vector3 point = element.transform.TransformPoint(Vector3.zero);

            return Responses.JSONResponse.FromObject(new { x = (int)point.x, y = (int)point.y, z = (int)point.z });
                // Note that appium has no concept of z, but passing it anyways
        }
    }
}
