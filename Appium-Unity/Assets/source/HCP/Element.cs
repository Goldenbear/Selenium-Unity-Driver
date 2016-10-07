//////////////////////////////////////////////////////////////////////////
/// @file	Element.cs
///
/// @author Colin Nickerson
///
/// @brief	A unique attribute (GUID) that can be added to any component.
///
/// @note 	Copyright 2016 Hutch Games Ltd. All rights reserved.
//////////////////////////////////////////////////////////////////////////

/************************ EXTERNAL NAMESPACES ***************************/

using UnityEngine;																// Unity 			(ref http://docs.unity3d.com/Documentation/ScriptReference/index.html)
using System;

namespace HCP
{
    //////////////////////////////////////////////////////////////////////////
    /// @brief	ElementAttribute class.  
    //////////////////////////////////////////////////////////////////////////
    public class ElementAttribute : PropertyAttribute {}

    //////////////////////////////////////////////////////////////////////////
    /// @brief	Element class.  Stores a guid for the component it is 
    /// attached to.  This isn't editable.  HCP requires objects to have Element
    /// components if they wish to be visible to it.
    //////////////////////////////////////////////////////////////////////////
    [AddComponentMenu("HCP/Element")]
    public class Element : MonoBehaviour
    {
        /***************************** PUBLIC DATA ******************************/
        [Element]
        [SerializeField]
        protected string m_sUniqueGuid;
        public string Id { get { return "HCP-" + (this.m_bUnsafe ? "UNSAFE-" : "" ) + m_sUniqueGuid; } }

        [SerializeField]
        [HideInInspector]
        protected bool m_bUnsafe = true;   // Generated at runtime

		/// @brief	Computes the screen-space rect of this element from its RectTransform and Canvas (will find if not provided)
		public Rect GetScreenRect(Canvas canvas=null)
		{
			RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
			if(rectTrans != null)
			{
				// Find this elements Canvas if not provided
				if(canvas == null)
				{
					Transform trans = transform;
					do
					{
						canvas = trans.gameObject.GetComponent<Canvas>();
						if(canvas != null)
							break;
						trans = trans.parent;
					}
					while((trans != null) && (canvas == null));
				}
	
				// If have a canvas then determine the screen space rect
				if(canvas != null)
				{
					Rect rect = GetScreenRect(rectTrans, canvas);
					return rect;
				}
			}

			return new Rect(0, 0, 0, 0);
		}

        // Reset is called when the user hits the Reset button in the Inspector's 
        // context menu or when adding the component the first time. This function
        // is only called in editor mode. Reset is most commonly used to give good 
        // default values in the inspector.
        private void Reset()
        {
            m_bUnsafe = false;     
            
            SetUId();
        }
        
        private void SetUId()
        {
            // Generate a unique ID, defaults to an empty string if nothing has been serialized yet
            if (String.IsNullOrEmpty(m_sUniqueGuid))
            {
                Guid guid = Guid.NewGuid();
                m_sUniqueGuid = guid.ToString();
            }
        }

        private void Start()
        {
            if(this.GetComponents<Element>().Length > 1)
            {
                throw new System.Exception("HCP.Element Error - You cannot attach more than one Element component to a single game object.");
            }
            
            SetUId();
        }

		private static Rect GetScreenRect(RectTransform rectTransform, Canvas canvas) 
		{
			Vector3[] corners = new Vector3[4];
			Vector3[] screenCorners = new Vector3[2];
			
			rectTransform.GetWorldCorners(corners);
			
			if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
			{
				screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
				screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
			}
			else
			{
				screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
				screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
			}
			
			screenCorners[0].y = Screen.height - screenCorners[0].y;
			screenCorners[1].y = Screen.height - screenCorners[1].y;
			
			return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
	    }
    }

}

