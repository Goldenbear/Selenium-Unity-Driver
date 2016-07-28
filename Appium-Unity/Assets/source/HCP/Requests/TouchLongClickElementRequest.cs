using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;

using UnityEngine.EventSystems;
using UnityEngine;

namespace HCP.Requests
{
    public class TouchLongClickElementRequest : JobRequest
    {
        public string Id { get { return Data["elementId"]; } }
        public float X { get { return Data["x"].AsFloat; } }
        public float Y { get { return Data["y"].AsFloat; } }
        public float Duration { get { return Data["duration"].AsFloat; } }
        
        public TouchLongClickElementRequest(JSONClass json) : base(json)
        {
            m_endTime = 0;
            m_startTime = 0;
        }

        protected float m_endTime;
        protected float m_startTime;

        protected enum EState
        {
            STAGE_1,
            STAGE_2,
            STAGE_3,
        }
        protected EState State
        {
            get
            {
                if(m_endTime == 0)
                // Stage 1 - Starting
                {
                    return EState.STAGE_1;
                }
                else if(m_startTime < m_endTime)
                    // Stage 2 - Running
                {
                    return EState.STAGE_2;
                }
                else
                    // State 3 - Complete
                {
                    return EState.STAGE_3;
                }
            }
        }

        protected JobResponse ProcessDown()
        {
            m_startTime = Time.time;
            m_endTime = m_startTime + this.Duration;

            var toTouch = JobRequest.GetElementById(this.Id);

            // Down
            var ptr = new PointerEventData(EventSystem.current);
            ptr.position = ptr.pressPosition = new Vector2(X, Y);
            ExecuteEvents.Execute(toTouch.gameObject, ptr, ExecuteEvents.pointerDownHandler);

            return null;
        }

        protected JobResponse ProcessWait()
        {
            // Intentionally nothing
            return null;
        }

        protected JobResponse ProcessUp()
        {
            m_startTime = 0;
            m_endTime = 0;

            var toTouch = JobRequest.GetElementById(this.Id);

            // Down
            var ptr = new PointerEventData(EventSystem.current);
            ptr.position = ptr.pressPosition = new Vector2(X, Y);
            ExecuteEvents.Execute(toTouch.gameObject, ptr, ExecuteEvents.pointerUpHandler);

            return new Responses.StringResponse();
        }

        public override JobResponse Process()
        {
            if(this.State == EState.STAGE_1)
                // Stage 1 - Starting
            {
                return ProcessDown();
            }
            else if(this.State == EState.STAGE_2)
                // Stage 2 - Running
            {
                return ProcessWait();
            }
            else
                // State 3 - Complete
            {
                return ProcessUp();
            }            
        }
    }
}
