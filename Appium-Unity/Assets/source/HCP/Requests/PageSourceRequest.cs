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

        protected static void CompleteChild(GameObject gameObject, XmlDocument xmlDoc, XmlElement parentXmlElement, int index)
        {
            var element = gameObject.GetComponent<Element>();
            var xmlElement = parentXmlElement;

            if(element != null)
            {
                var childXmlElement = xmlDoc.CreateElement(element.name);
                var textComponent = element.GetComponent<Text>();
                var canvasComponent = element.GetComponent<Canvas>();
                var buttonComponent = element.GetComponent<Button>();
                var toggleComponent = element.GetComponent<Toggle>();
                var dropdownComponent = element.GetComponent<Dropdown>();

                if(canvasComponent != null)
                {
                    childXmlElement = xmlDoc.CreateElement(canvasComponent.GetType().FullName);
                    childXmlElement.SetAttribute("class", canvasComponent.GetType().FullName);
                }
                else if(textComponent != null)
                {
                    childXmlElement = xmlDoc.CreateElement(textComponent.GetType().FullName);
                    childXmlElement.SetAttribute("class", textComponent.GetType().FullName);
                    childXmlElement.SetAttribute("text", textComponent.text);
                }
                else if(buttonComponent != null)
                {
                    childXmlElement = xmlDoc.CreateElement(buttonComponent.GetType().FullName);
                    childXmlElement.SetAttribute("class", buttonComponent.GetType().FullName);
                }
                else if(toggleComponent != null)
                {
                    childXmlElement = xmlDoc.CreateElement(toggleComponent.GetType().FullName);
                    childXmlElement.SetAttribute("class", toggleComponent.GetType().FullName);
                    childXmlElement.SetAttribute("checked", toggleComponent.isOn ? "true" : "false");
                }
                else if(dropdownComponent != null)
                {
                    childXmlElement = xmlDoc.CreateElement(dropdownComponent.GetType().FullName);
                    childXmlElement.SetAttribute("class", dropdownComponent.GetType().FullName);
                }
                childXmlElement.SetAttribute("package", "unity");
                childXmlElement.SetAttribute("enabled", element.gameObject.activeSelf ? "true" : "false");
                childXmlElement.SetAttribute("displayed", element.gameObject.activeInHierarchy ? "true" : "false");

                Vector3 point = GetElementLocationRequest.GetLocation(element);
                Bounds bounds = GetElementSizeRequest.GetBounds(element);
                
                childXmlElement.SetAttribute("name", element.name);

                var rectTransform = element.GetComponent<RectTransform>();
                if(rectTransform != null)
                {
                    point.x -= bounds.extents.x*2 * rectTransform.pivot.x;
                    point.y -= bounds.extents.y*2 * rectTransform.pivot.y;
                }

                childXmlElement.SetAttribute("bounds", String.Format("[{0},{1}][{2},{3}]", (int)point.x, Screen.height - (int)point.y, (int)bounds.extents.x*2, (int)bounds.extents.y*2));


                childXmlElement.SetAttribute("resource-id", element.Id);
                childXmlElement.SetAttribute("index", index.ToString());
                childXmlElement.SetAttribute("isHCP", "true");
                
                parentXmlElement.AppendChild(childXmlElement);
                xmlElement = childXmlElement;
            }

            for(int i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);
                CompleteChild(child.gameObject, xmlDoc, xmlElement, i);
            }

        }

        public override JobResponse Process()
        {
            XmlDocument xmlDoc = new XmlDocument( );
            GameObject[] roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            XmlElement xmlElement = xmlDoc.CreateElement("hierarchy");
            xmlDoc.AppendChild(xmlElement);

            for(int i = 0; i < roots.Length; i++)
            {
                CompleteChild(roots[i].gameObject, xmlDoc, xmlElement, i);
            }
            
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xmlDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return new Responses.StringResponse(stringWriter.GetStringBuilder().ToString());
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
