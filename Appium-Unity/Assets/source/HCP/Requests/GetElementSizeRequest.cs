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
            var renderer = element.GetComponent<Renderer> ();
            var rectTransform = element.GetComponent<RectTransform> ();

            Bounds bounds = new Bounds ();
            if (renderer != null)
                bounds = renderer.bounds;
            if (rectTransform != null)
                bounds = new Bounds (Vector3.zero, rectTransform.sizeDelta);

            float scale = (Camera.main.WorldToScreenPoint (Vector3.forward) - Camera.main.WorldToScreenPoint (Vector3.zero)).magnitude;
            
            return bounds.extents * 2 * scale / Server.DeviceScreenScalar;
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
