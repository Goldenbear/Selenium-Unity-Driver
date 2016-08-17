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
            CLASSNAME,
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

                switch (stringValue)
                {
                    case "name": return EAttribute.NAME;
                    case "className": return EAttribute.CLASSNAME;
                    case "displayed": return EAttribute.DISPLAYED;
                    case "enabled": return EAttribute.ENABLED;
                    case "selected": return EAttribute.SELECTED;

                    default: throw new FormatException("Unsupported element request");
                }
            }
        }


        public GetElementAttributeRequest(JSONClass json) : base(json)
        {
        }

        public static string GetClassName(Element element)
        {
            var textComponent = element.GetComponent<UnityEngine.UI.Text>();
            var canvasComponent = element.GetComponent<UnityEngine.Canvas>();
            var buttonComponent = element.GetComponent<UnityEngine.UI.Button>();
            var toggleComponent = element.GetComponent<UnityEngine.UI.Toggle>();
            var dropdownComponent = element.GetComponent<UnityEngine.UI.Dropdown>();
            var inputComponent = element.GetComponent <UnityEngine.UI.InputField>();

            if (canvasComponent != null)
            {
                return (canvasComponent.GetType().FullName);
            }
            else if (textComponent != null)
            {
                return (textComponent.GetType().FullName);
            }
            else if (buttonComponent != null)
            {
                return (buttonComponent.GetType().FullName);
            }
            else if (toggleComponent != null)
            {
                return (toggleComponent.GetType().FullName);
            }
            else if (dropdownComponent != null)
            {
                return (dropdownComponent.GetType().FullName);
            }
            else if (inputComponent != null)
            {
                return (inputComponent.GetType().FullName);
            }
            else
            {
                return (element.GetType().FullName);
            }
        }

        public override JobResponse Process()
        {
            string response = null;
            var element = JobRequest.GetElementById(this.Id);

            switch (this.Attribute)
            {
                case EAttribute.NAME:
                    response = element.gameObject.name;
                    break;
                case EAttribute.CLASSNAME:
                    response = GetClassName(element);
                    break;
                case EAttribute.DISPLAYED:
                    {
                        UnityEngine.Vector3 point = GetElementLocationRequest.GetLocation(element);
                        UnityEngine.Vector3 size = GetElementSizeRequest.GetSize(element);

                        if (point.y < 0 || point.y + size.y > UnityEngine.Screen.height ||
                            point.x < 0 || point.x + size.x > UnityEngine.Screen.width)
                        {
                            response = "false";
                        }
                        else
                        {
                            response = element.gameObject.activeInHierarchy ? "true" : "false";
                        }
                    }
                    break;
                case EAttribute.ENABLED:
                    response = element.gameObject.activeSelf ? "true" : "false";
                    break;
                case EAttribute.SELECTED:
                    response = (element.gameObject == EventSystem.current.currentSelectedGameObject) ? "true" : "false";
                    break;
            }

            return new Responses.StringResponse(response);
        }
    }
}
