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
    /// @brief	UniqueIdentifierAttribute class.  
    //////////////////////////////////////////////////////////////////////////
    public class ElementAttribute : PropertyAttribute {}

    //////////////////////////////////////////////////////////////////////////
    /// @brief	UniqueId class.  Stores a guid for the component it is 
    /// attached to.  This isn't editable.
    //////////////////////////////////////////////////////////////////////////
    [AddComponentMenu("HCP/Element")]
    public class Element : MonoBehaviour
    {
        /***************************** PUBLIC DATA ******************************/
        [Element]
        [SerializeField]
        protected string m_sUniqueGuid;
        public string Id { get { return m_sUniqueGuid; } }
    }
}

