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
        
        public GetElementSizeRequest(JSONClass json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            var element = JobRequest.GetElementById(this.Id);
            var renderer = element.GetComponent<Renderer>();
            var rectTransform = element.GetComponent<RectTransform>();

            Bounds bounds = new Bounds();
            if(renderer != null) bounds = renderer.bounds;
            if(rectTransform != null) bounds = new Bounds(Vector3.zero, rectTransform.sizeDelta);

            return Responses.JSONResponse.FromObject(new { width = (int)bounds.extents.x, height = (int)bounds.extents.y, depth = (int)bounds.extents.z });
                // Note that appium has no concept of depth, but passing it anyways
        }
    }
}
