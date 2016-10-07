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
        public string Id { get { return Data ["elementId"]; } }

        public GetElementSizeRequest (JSONClass json) : base (json)
        {
        }

        public static Vector3 GetSize (Element element)
        {
			Rect screenRect = element.GetScreenRect();
			return new Vector3(screenRect.size.x, screenRect.size.y, 0.0f);
        }

        public override JobResponse Process ()
        {
            var element = JobRequest.GetElementById (this.Id);
            Vector3 size = GetSize (element);

            return Responses.JSONResponse.FromObject (new { width = (int)size.x, height = (int)size.y, depth = (int)size.z });
            // Note that appium has no concept of depth, but passing it anyways
        }
    }
}
