using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;
using UnityEngine.EventSystems;

namespace HCP.Requests
{
    public class GetElementAttributeRequest : JobRequest
    {
        public enum EAttribute
        {
            NAME,
            DISPLAYED,
            ENABLED,
            SELECTED
        };

        public string Id { get { return Data["elementId"]; } }
        public EAttribute Attribute
        {
            get
            {
                var stringValue = Data["attribute"];

                switch(stringValue)
                {
                    case "className":   return EAttribute.NAME;
                    case "displayed":   return EAttribute.DISPLAYED;
                    case "enabled":     return EAttribute.ENABLED;
                    case "selected":    return EAttribute.SELECTED;

                    default: throw new FormatException("Unsupported element request");
                }
            }
        }

        
        public GetElementAttributeRequest(string json) : base(json)
        {
        }

        public override JobResponse Process()
        {
            string response = null;
            var element = JobRequest.GetElementById(this.Id);

            switch(this.Attribute)
            {
                case EAttribute.NAME:
                    response = element.gameObject.name;
                    break;
                case EAttribute.DISPLAYED:
                    response = element.gameObject.activeInHierarchy.ToString();
                    break;
                case EAttribute.ENABLED:
                    response = element.gameObject.activeSelf.ToString();
                    break;
                case EAttribute.SELECTED:
                    response = (element.gameObject == EventSystem.current.currentSelectedGameObject).ToString();
                    break;
            }

            return  new Responses.StringResponse(response);
        }
    }
}
