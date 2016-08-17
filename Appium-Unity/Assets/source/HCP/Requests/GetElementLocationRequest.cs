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

        public static Vector3 GetLocation(Element element)
		{
			Vector3 point = element.transform.TransformPoint(Vector3.zero);
			Vector3 size = GetElementSizeRequest.GetSize(element);

			var rectTransform = element.GetComponent<RectTransform>();
			if (rectTransform != null) 
			{
				point.x -= size.x * rectTransform.pivot.x;
				point.y += size.y * rectTransform.pivot.y;
			} 
			else
			{
				point = Camera.main.WorldToScreenPoint (point);
			}

			return new Vector3(point.x, Screen.height - point.y, point.z) / Server.DeviceScreenScalar;
        }

        public override JobResponse Process()
        {
            var element = JobRequest.GetElementById(this.Id);
            Vector3 point = GetLocation(element);

            return Responses.JSONResponse.FromObject(new { x = (int)point.x, y = (int)point.y, z = (int)point.z });
                // Note that appium has no concept of z, but passing it anyways
        }
    }
}
