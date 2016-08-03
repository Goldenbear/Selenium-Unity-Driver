using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP.SimpleJSON;

using UnityEngine;

namespace HCP
{
    ////////////////////////////////////////////////////////////
    // @brief JobRequests are what they sound like.  They have 
    // JSON formated data that is used to query against. This
    // class should be derived from once for each acceptable HCP
    // action.  These actions are then registerd in the server
    // object.  See Server.cs/Awake for that process.
    ////////////////////////////////////////////////////////////
    public abstract class JobRequest
    {
        private JSONNode m_data;
        protected JSONNode Data { get { return this.m_data; } }

        public JobRequest(JSONClass json)
        {            
            if(json == null)
            {
                throw new ArgumentNullException("JobRequest cannot assign null data");
            }

            m_data = json;
        }        

        public abstract JobResponse Process();

        #region Utility
        protected static Element GetElementById(string id)
        {
            return Resources.FindObjectsOfTypeAll<Element>().First(e => e.Id == id);
        }
        #endregion
    }
}
