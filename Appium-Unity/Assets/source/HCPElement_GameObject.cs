using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HCPElement_GameObject : UniqueId, IHCPElement
{
    public bool Displayed
    {
        get
        {
            return this.gameObject.activeInHierarchy;
        }
    }

    public bool Enabled
    {
        get
        {
            return this.gameObject.activeSelf;
        }
    }

    public string Id
    {
        get
        {
            return this.m_sUniqueGuid;
        }
    }

    public Vector2 Location
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public Vector2 LocationOnScreenOnceScrolledIntoView
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public bool Selected
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public Vector2 Size
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public string TagName
    {
        get
        {
            return this.gameObject.tag;
        }
    }

    public string Text
    {
        get
        {
            throw new NotImplementedException();
        }
    }
}
