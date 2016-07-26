using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP.SimpleJSON;

using UnityEngine;

namespace HCP
{
    public abstract class JobRequest
    {
        private JSONNode m_data;
        protected JSONNode Data { get { return this.m_data; } }

        public JobRequest(string json)
        {            
            if(json == null)
            {
                throw new ArgumentNullException("JobRequest cannot assign null data");
            }

            m_data = JSON.Parse(json);
        }        

        public abstract JobResponse Process();

        #region Utility
        protected static Element GetElementById(string id)
        {
            return GameObject.FindObjectsOfType<Element>().First(e => e.Id == id);
        }
        #endregion
    }
}
