//--------------------------------------------------------------------------
//  <copyright file="Framework.cs">
//      Copyright (c) Andrea Tino. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace AppiumTest.Framework
{
    using System;
    using OpenQA.Selenium.Remote;

    public sealed class TestCapabilities
    {
        /// Tracking platforms  
        public enum DevicePlatform
        {
            Undefined,
            Windows,
            IOS,
            Android
        }

        public string BrowserName { get; set; }
        public string FwkVersion { get; set; }
        public DevicePlatform Platform { get; set; }
        public string PlatformVersion { get; set; }
        public string DeviceName { get; set; }
        public string App { get; set; }
        public bool AutoWebView { get; set; }
        public string AutomationName { get; set; }

        public TestCapabilities()
        {
            this.BrowserName = String.Empty;
            this.FwkVersion = String.Empty;
            this.Platform = DevicePlatform.Undefined;
            this.PlatformVersion = String.Empty;
            this.DeviceName = String.Empty;
            this.App = String.Empty;
            this.AutoWebView = true;
            this.AutomationName = String.Empty;
        }

        public void AssignAppiumCapabilities(ref DesiredCapabilities appiumCapabilities)
        {
            appiumCapabilities.SetCapability("browserName", this.BrowserName);
            appiumCapabilities.SetCapability("appium-version", this.FwkVersion);
            appiumCapabilities.SetCapability("platformName", this.Platform2String(this.Platform));
            appiumCapabilities.SetCapability("platformVersion", this.PlatformVersion);
            appiumCapabilities.SetCapability("deviceName", this.DeviceName);
            appiumCapabilities.SetCapability("autoWebview", this.AutoWebView);

            // App push (will be covered later)
            if (this.App != String.Empty)
                appiumCapabilities.SetCapability("app", this.App);
        }

        /// Converting to string the platform (for Appium)
        private string Platform2String(DevicePlatform value)
        {
            switch (value)
            {
                case DevicePlatform.Windows:
                    return "win"; /* TODO: Need to write your own extension of Appium for this */
                case DevicePlatform.IOS:
                    return "iOS";
                case DevicePlatform.Android:
                    return "Android";
                default:
                    return "";
            }
        }
    }
}


