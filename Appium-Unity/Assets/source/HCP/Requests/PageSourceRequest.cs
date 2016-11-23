using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HCP;
using HCP.SimpleJSON;
using System.Xml;

using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace HCP.Requests
{
    // Format sample at EOF
    public class PageSourceRequest : JobRequest
    {
        public PageSourceRequest(JSONClass json) : base(json)
        {
        }

        protected static void CompleteChild(GameObject gameObject, XmlDocument xmlDoc, XmlElement parentXmlElement, int index, Canvas canvas)
        {
            var element = gameObject.GetComponent<Element>();
            var xmlElement = parentXmlElement;

            if(element != null)
			{
				var canvasComponent = element.GetComponent<Canvas>();
				var buttonComponent = element.GetComponent<UnityEngine.UI.Button>();
				var textComponent = element.GetComponent<UnityEngine.UI.Text>();
				var inputComponent = element.GetComponent<UnityEngine.UI.InputField>();
				var toggleComponent = element.GetComponent<UnityEngine.UI.Toggle>();
				//var imageComponent = element.GetComponent<UnityEngine.UI.Image>();

				if(canvasComponent != null)
					canvas = canvasComponent;

				var childXmlElement = xmlDoc.CreateElement (GetElementAttributeRequest.GetClassName(element));
				childXmlElement.SetAttribute ("class", GetElementAttributeRequest.GetClassName(element));

				if (buttonComponent != null)
				{
					childXmlElement.SetAttribute ("clickable", "true");
				}
				else if (textComponent != null) 
				{
					childXmlElement.SetAttribute ("text", textComponent.text);
					//childXmlElement.SetAttribute ("clickable", "true");			// ST: Don't want to pick Text elements in Appium Inspector screenshot window
				}  
				else if (inputComponent != null) 
				{
					childXmlElement.SetAttribute ("text", inputComponent.text);
					childXmlElement.SetAttribute ("clickable", "true");
				}   
				else if (toggleComponent != null) 
				{
					childXmlElement.SetAttribute ("checked", toggleComponent.isOn ? "true" : "false");
					childXmlElement.SetAttribute ("clickable", "true");
					childXmlElement.SetAttribute ("checkable", "true");
				} 
                childXmlElement.SetAttribute("package", "unity");
				childXmlElement.SetAttribute("enabled", element.gameObject.activeSelf ? "true" : "false");
				childXmlElement.SetAttribute("displayed", element.gameObject.activeInHierarchy ? "true" : "false");
				childXmlElement.SetAttribute("name", element.gameObject.name);

				Rect screenRect = element.GetScreenRect(canvas);
				childXmlElement.SetAttribute("bounds", String.Format("[{0},{1}][{2},{3}]", (int)screenRect.min.x, (int)screenRect.min.y, (int)screenRect.size.x, (int)screenRect.size.y));


                childXmlElement.SetAttribute("resource-id", element.Id);
                childXmlElement.SetAttribute("index", index.ToString());
                childXmlElement.SetAttribute("isHCP", "true");
                
                parentXmlElement.AppendChild(childXmlElement);
                xmlElement = childXmlElement;
            }

            for(int i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);
                CompleteChild(child.gameObject, xmlDoc, xmlElement, i, canvas);
            }

        }

        public override JobResponse Process()
        {
			Debug.Log("HutchAppium: pagesource request received.");

			// Create xml doc to hold page source of UI hierarchy
            XmlDocument xmlDoc = new XmlDocument( );

            XmlElement xmlElement = xmlDoc.CreateElement("hierarchy");
            xmlDoc.AppendChild(xmlElement);

			// Get list of UI Canvases currently in the world
			Canvas[] canvases = GameObject.FindObjectsOfType(typeof(Canvas)) as Canvas[];
			Debug.Log("HutchAppium: pagesource found "+canvases.Length+" Canvases");

			// Add root canvases with their hierarchy of child UI objects to the xml doc
            for(int i = 0; i < canvases.Length; i++)
            {
				bool bIsRootCanvas = true;

				// Determine whether this is a root canvas (ie. doesn't have any parent canvas)
				GameObject go = canvases[i].gameObject;
				while((go != null) && (go.transform != null) && (go.transform.parent != null) && (go.transform.parent.gameObject != null))
				{
					go = go.transform.parent.gameObject;
					Canvas parentCanvas = go.GetComponent<Canvas>();
					if(parentCanvas != null)
					{
						bIsRootCanvas = false;
					}
				}

				if(bIsRootCanvas)
				{
					Debug.Log("HutchAppium: pagesource adding root Canvas "+canvases[i].name);
                	CompleteChild(canvases[i].gameObject, xmlDoc, xmlElement, i, canvases[i]);
				}
				else
				{
					Debug.Log("HutchAppium: pagesource Canvas "+canvases[i].name+" is not a root Canvas.");
				}
            }
            
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xmlDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();

				string pageSource = stringWriter.GetStringBuilder().ToString();
				Debug.Log("HutchAppium: pagesource response: "+pageSource);

                return new Responses.StringResponse(pageSource);
            }
        }
    }
}

//<?xml version="1.0" encoding="UTF-8"?>
//<hierarchy rotation="0">
//	<android.widget.FrameLayout index="0" text="" class="android.widget.FrameLayout" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="false" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,0][1920,1080]" resource-id="" instance="0">
//		<android.widget.LinearLayout index="0" text="" class="android.widget.LinearLayout" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="false" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,0][1920,1080]" resource-id="" instance="0">
//			<android.widget.FrameLayout index="0" text="" class="android.widget.FrameLayout" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="false" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,25][1920,50]" resource-id="" instance="1">
//				<android.widget.TextView index="0" text="Contact Manager" class="android.widget.TextView" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="false" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[6,26][1914,48]" resource-id="android:id/title" instance="0"/>
//			</android.widget.FrameLayout>
//			<android.widget.FrameLayout index="1" text="" class="android.widget.FrameLayout" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="false" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,50][1920,1080]" resource-id="android:id/content" instance="2">
//				<android.widget.LinearLayout index="0" text="" class="android.widget.LinearLayout" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="false" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,50][1920,1080]" resource-id="" instance="1">
//					<android.widget.ListView index="0" text="" class="android.widget.ListView" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="true" enabled="true" focusable="true" focused="true" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,50][1920,984]" resource-id="com.example.android.contactmanager:id/contactList" instance="0">
//						<android.widget.LinearLayout index="0" text="" class="android.widget.LinearLayout" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="true" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,50][90,69]" resource-id="" instance="2">
//							<android.widget.TextView index="0" text="Gordon Wright" class="android.widget.TextView" package="com.example.android.contactmanager" content-desc="false" checkable="false" checked="false" clickable="false" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,50][90,69]" resource-id="com.example.android.contactmanager:id/contactEntryText" instance="1"/>
//						</android.widget.LinearLayout>
//						<android.widget.LinearLayout index="1" text="" class="android.widget.LinearLayout" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="true" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,70][121,89]" resource-id="" instance="3">
//							<android.widget.TextView index="0" text="jason@kotaku.com" class="android.widget.TextView" package="com.example.android.contactmanager" content-desc="false" checkable="false" checked="false" clickable="false" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,70][121,89]" resource-id="com.example.android.contactmanager:id/contactEntryText" instance="2"/>
//						</android.widget.LinearLayout>
//						<android.widget.LinearLayout index="2" text="" class="android.widget.LinearLayout" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="true" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,90][32,109]" resource-id="" instance="4">
//							<android.widget.TextView index="0" text="Mom" class="android.widget.TextView" package="com.example.android.contactmanager" content-desc="false" checkable="false" checked="false" clickable="false" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,90][32,109]" resource-id="com.example.android.contactmanager:id/contactEntryText" instance="3"/>
//						</android.widget.LinearLayout>
//						<android.widget.LinearLayout index="3" text="" class="android.widget.LinearLayout" package="com.example.android.contactmanager" content-desc="" checkable="false" checked="false" clickable="true" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,110][76,129]" resource-id="" instance="5">
//							<android.widget.TextView index="0" text="Shea Martin" class="android.widget.TextView" package="com.example.android.contactmanager" content-desc="false" checkable="false" checked="false" clickable="false" enabled="true" focusable="false" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,110][76,129]" resource-id="com.example.android.contactmanager:id/contactEntryText" instance="4"/>
//						</android.widget.LinearLayout>
//					</android.widget.ListView>
//					<android.widget.CheckBox index="1" text="Show Invisible Contacts (Only)" class="android.widget.CheckBox" package="com.example.android.contactmanager" content-desc="Show Invisible Contacts (Only)" checkable="true" checked="false" clickable="true" enabled="true" focusable="true" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,984][254,1032]" resource-id="com.example.android.contactmanager:id/showInvisible" instance="0"/>
//					<android.widget.Button index="2" text="Add Contact" class="android.widget.Button" package="com.example.android.contactmanager" content-desc="Add Contact" checkable="false" checked="false" clickable="true" enabled="true" focusable="true" focused="false" scrollable="false" long-clickable="false" password="false" selected="false" bounds="[0,1032][1920,1080]" resource-id="com.example.android.contactmanager:id/addContactButton" instance="0"/>
//				</android.widget.LinearLayout>
//			</android.widget.FrameLayout>
//		</android.widget.LinearLayout>
//	</android.widget.FrameLayout>
//</hierarchy>
