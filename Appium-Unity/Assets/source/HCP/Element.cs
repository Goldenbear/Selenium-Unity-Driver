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
    }

}

