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
        public string Id { get { return "HCP-" + this.name + "-" + m_sUniqueGuid; } }

        
        private void Start()
        {
            if(this.GetComponents<Element>().Length > 1)
            {
                throw new System.Exception("HCP.Element Error - You cannot attach more than Element component to a single game object.");
            }
        }
    }

}

