//--------------------------------------------------------------------------
//  <copyright file="Framework.cs">
//      Copyright (c) Andrea Tino. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace AppiumTest.Framework
{
    using System;
    using OpenQA.Selenium.Remote;
    using AppiumTests.Helpers;

    using OpenQA.Selenium.Appium.HCP;
    using OpenQA.Selenium.Appium; /* This is Appium */
    using System.Diagnostics;
    using System.Reflection;
    using Xunit.Abstractions;
    using System.Collections.Generic;
    using OpenQA.Selenium.Support.UI;
    using Xunit;
    using AppiumTests;
    using OpenQA.Selenium.Appium.Android;
    using OpenQA.Selenium.Appium.iOS;

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
        public bool SupportsHCP { get; set; }
        public string HCPHost { get; set; }
        public int HCPPort { get; set; }
        public string DeviceId { get; set; }

        public TestCapabilities()
        {
            this.BrowserName = String.Empty;
            this.FwkVersion = String.Empty;
            this.Platform = DevicePlatform.Undefined;
            this.PlatformVersion = String.Empty;
            this.DeviceName = "Unknown";
            this.App = String.Empty;
            this.AutoWebView = true;
            this.AutomationName = String.Empty;
            this.SupportsHCP = false;
            this.HCPHost = "http:/127.0.0.1";
            this.HCPPort = 14724;
            this.DeviceId = String.Empty;
        }

        public void AssignAppiumCapabilities(ref DesiredCapabilities appiumCapabilities)
        {

            appiumCapabilities.SetCapability("browserName", this.BrowserName);
            appiumCapabilities.SetCapability("appium-version", this.FwkVersion);
            appiumCapabilities.SetCapability("platformName", this.Platform2String(this.Platform));
            appiumCapabilities.SetCapability("platformVersion", this.PlatformVersion);
            appiumCapabilities.SetCapability("deviceName", this.DeviceName);
            appiumCapabilities.SetCapability("autoWebview", this.AutoWebView);
            appiumCapabilities.SetCapability("hcp", this.SupportsHCP);
            appiumCapabilities.SetCapability("hcpHost", this.HCPHost);
            appiumCapabilities.SetCapability("hcpPort", this.HCPPort);

            if (this.DeviceId != String.Empty)
                appiumCapabilities.SetCapability("udid", this.DeviceId);

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
    
    ////////////////////////////////////////////////////////////
    // @brief A delegate used to create drivers as needed
    ////////////////////////////////////////////////////////////
    public delegate AppiumHCPDriver<AppiumWebElement> CreateDriver();
    public delegate void IssueCommandCommand();

    public abstract class TestSuite : IDisposable
        // DevNote: There is a lot of details here, so I chose to group vars and 
        // methods by purpose so that you might better digest what is going on.
    {
        #region Communication Defaults
        private static Uri APPIUM_SERVER_URI = new Uri(TestServers.Server2);
        private static int HCP_PORT = 14812;
        private static TimeSpan INIT_TIMEOUT_SEC = TimeSpan.FromSeconds(360); /* Change this to a more reasonable value */
        private static TimeSpan IMPLICIT_TIMEOUT_SEC = TimeSpan.FromSeconds(10); /* Change this to a more reasonable value */
        private static TimeSpan HCP_TIMEOUT_SEC = TimeSpan.FromSeconds(100); /* Change this to a more reasonable value */
        private static string APP_PATH = "./minimal"; // NO EXTENSION

        ////////////////////////////////////////////////////////////
        // @brief This driver is used to communicate with Appium. You
        // must close this properly or the server will be in an error
        // state.  This is done in Dispose. You can only have one
        // of these for each appium server.
        ////////////////////////////////////////////////////////////
        private static AppiumHCPDriver<AppiumWebElement> g_driver;
        #endregion


        #region Screenshots
        private static string IMAGE_DIRECTORY { get { return "screenshots"; } }
        private static string IMAGE_HOST { get { return "http://127.0.0.1/images/"; } }

        private static string MakeValidFilename(string fileName)
        {
            // Nothing too complicated, removing bad characters.
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
               fileName = fileName.Replace(c, '_');
            }

            return fileName;
        }

        ////////////////////////////////////////////////////////////
        // @brief Creates a jpg and saves it to disk.  Will have a 
        // useful filename is called directly from a unit test.
        // Otherwise its function name guess in the stack will be off.
        // Spits out a sample URL for web access to the screenshots
        // When used in conjunction with IssueCommand, you will get
        // a detailed log.
        ////////////////////////////////////////////////////////////
        protected void WriteScreenshot(string fileName = null)
        {
            System.IO.Directory.CreateDirectory(IMAGE_DIRECTORY);

            if(fileName == null)
            {
                StackTrace st = new StackTrace();
                StackFrame sf = st.GetFrame(1);
                MethodBase testMethodName = sf.GetMethod();
                fileName = testMethodName.Name;
            }

            fileName = MakeValidFilename(fileName);
            var fullPath = String.Format("{0}/{1} - {2:yyyy-MM-dd_hh-mm-ss-tt}.jpg",
                IMAGE_DIRECTORY,
                fileName,
                DateTime.Now);
            var urlPath = String.Format("{0}/{1} - {2:yyyy-MM-dd_hh-mm-ss-tt}.jpg",
                IMAGE_HOST,
                fileName,
                DateTime.Now);

            var screenshot = g_driver.GetScreenshot();
            screenshot.SaveAsFile(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            m_output.WriteLine(urlPath);
        }
        #endregion

        #region Report Logging
        ////////////////////////////////////////////////////////////
        // @brief xUnit output is injected automatically.  You can
        // tie this is with jerkins using the available plugin:
        // https://wiki.jenkins-ci.org/display/JENKINS/xUnit+Plugin
        ////////////////////////////////////////////////////////////
        protected readonly ITestOutputHelper m_output;

        protected void IssueCommand(string comment, IssueCommandCommand command)
        {
            m_output.WriteLine(comment);
            command();
        }

        protected object IssueCommand(string comment, Func<object> command)
        {
            m_output.WriteLine(comment);
            return command();
        }

        public TestSuite(ITestOutputHelper output)
        {
            this.m_output = output;
            g_driver = null;
        }
        #endregion

        #region Driver Construction - Used to initialize Appium
        ////////////////////////////////////////////////////////////
        // @brief Non-HCP sample to pull data and do sanity tests.
        ////////////////////////////////////////////////////////////
        private static AppiumHCPDriver<AppiumWebElement> ConstructBootstrap()
        {
            if(g_driver == null)
            {
                DesiredCapabilities capabilities = new DesiredCapabilities();
                
                TestCapabilities capabilties = new TestCapabilities();
                capabilties.App = "./ContactManager.apk";
                capabilties.AutoWebView = false; 
                capabilties.AutomationName = "ContactManager";
                capabilties.BrowserName = String.Empty; // Leave empty otherwise you test on browsers
                capabilties.DeviceName = "Android";
                capabilties.FwkVersion = "1.0"; // Not really needed
                capabilties.Platform = TestCapabilities.DevicePlatform.Android; // Or IOS
                capabilties.PlatformVersion = String.Empty; // Not really needed
                capabilties.SupportsHCP = false;
                capabilties.HCPHost = "http://127.0.0.1";
                capabilties.HCPPort = HCP_PORT;

                capabilties.AssignAppiumCapabilities(ref capabilities);

                AppiumHCPDriver<AppiumWebElement> driver = new AndroidDriver<AppiumWebElement>(APPIUM_SERVER_URI, capabilities, INIT_TIMEOUT_SEC);
                driver.Manage().Timeouts().ImplicitlyWait(IMPLICIT_TIMEOUT_SEC);

                g_driver = driver;
            }

            return g_driver;
        }

        ////////////////////////////////////////////////////////////
        // @brief Constructs an Android driver looking for minimal.apk
        ////////////////////////////////////////////////////////////
        private static AppiumHCPDriver<AppiumWebElement> ConstructAndroid()
        {
            if(g_driver == null)
            {
                DesiredCapabilities capabilities = new DesiredCapabilities();
                
                TestCapabilities capabilties = new TestCapabilities();
                capabilties.App = APP_PATH + ".apk";
                capabilties.AutoWebView = false; 
                capabilties.AutomationName = "minimal";
                capabilties.BrowserName = String.Empty; // Leave empty otherwise you test on browsers
                capabilties.DeviceName = "Android";
                capabilties.FwkVersion = "1.0"; // Not really needed
                capabilties.Platform = TestCapabilities.DevicePlatform.Android; // Or IOS
                capabilties.PlatformVersion = String.Empty; // Not really needed
                capabilties.SupportsHCP = true;
                capabilties.HCPHost = "http://127.0.0.1";
                capabilties.HCPPort = HCP_PORT;

                capabilties.AssignAppiumCapabilities(ref capabilities);

                AppiumHCPDriver<AppiumWebElement> driver = new AndroidDriver<AppiumWebElement>(APPIUM_SERVER_URI, capabilities, INIT_TIMEOUT_SEC);
                driver.Manage().Timeouts().ImplicitlyWait(IMPLICIT_TIMEOUT_SEC);

                g_driver = driver;
            }

            return g_driver;
        }

        ////////////////////////////////////////////////////////////
        // @brief Constructs an iOS driver looking for minimal.apk.
        // Only works on OSX
        ////////////////////////////////////////////////////////////
        private static AppiumHCPDriver<AppiumWebElement> ConstructIOS()
        {
            if(g_driver == null)
            {
                DesiredCapabilities capabilities = new DesiredCapabilities();
                
                TestCapabilities capabilties = new TestCapabilities();
                capabilties.App = "com.hutch.minimal";
                capabilties.AutoWebView = false; 
                capabilties.AutomationName = "minimal";
                capabilties.BrowserName = String.Empty; // Leave empty otherwise you test on browsers
                capabilties.DeviceName = "iOS";
                capabilties.FwkVersion = "1.0"; // Not really needed
                capabilties.Platform = TestCapabilities.DevicePlatform.IOS; // Or IOS
                capabilties.PlatformVersion = "8.2"; // Must match device
                capabilties.SupportsHCP = true;
                capabilties.HCPHost = "http://192.168.1.2";
                capabilties.HCPPort = HCP_PORT;
                capabilties.DeviceId = "auto";//6b4f8fac9a129df15b5a69af471cdf93952dc34e"; 
                capabilties.AssignAppiumCapabilities(ref capabilities);

                AppiumHCPDriver<AppiumWebElement> driver = new IOSDriver<AppiumWebElement>(APPIUM_SERVER_URI, capabilities, INIT_TIMEOUT_SEC);
                driver.Manage().Timeouts().ImplicitlyWait(IMPLICIT_TIMEOUT_SEC);

                g_driver = driver;
            }
                
            return g_driver;
        }
        #endregion


        #region Test Parameters - Manual Driver Lists
        ////////////////////////////////////////////////////////////
        // @brief This is the format required to pass data in 
        // property form to our unit tests.  Note that this does not
        // return the driver itself because Visual Studio test panels
        // create the drivers repeatedly and break the server
        ////////////////////////////////////////////////////////////
        public static IEnumerable<object[]> OnDevices
        {
            get
            {
                // Or this could read from a file. :)
                return new[]
                {
                    //new CreateDriver[] { ConstructAndroid },
                  new CreateDriver[] { ConstructIOS }
                };
            }
        }

        ////////////////////////////////////////////////////////////
        // @brief A driver just for non-HCP Android tests.
        ////////////////////////////////////////////////////////////
        public static IEnumerable<object[]> WithBootstrap
        {
            get
            {
                // Or this could read from a file. :)
                return new[]
                {
                    new CreateDriver[] { ConstructBootstrap },
                };
            }
        }
        #endregion


        #region Cleanup
        ////////////////////////////////////////////////////////////
        // @brief This is necessary and will cause the Appium server
        // to enter an error state is missed.
        ////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if(g_driver != null)
            {
                g_driver.Quit(); // Always quit, if you don't, next test session will fail
                g_driver = null;
            }
        }
        #endregion

        #region Test Helpers 
        ////////////////////////////////////////////////////////////
        // @brief Illustrates how you can wait for something to be
        // complete piror to continuing in the test.  Here, we have
        // a sample that waits until HCP is ready, which internally
        // returns a test method to see if a specific bool is true.
        // Note that wait until still obeys timeouts specified in
        // driver construction. This is a better approach to sleeping
        // the thread of execution as you will continue as soon as its
        // ready, rather than waiting a fixed amount of time.
        ////////////////////////////////////////////////////////////
        protected void WaitforHCP(AppiumHCPDriver<AppiumWebElement> driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, HCP_TIMEOUT_SEC);
            bool result = wait.Until<bool>(ExpectedHCPConditions.HCPReady());

            Assert.Equal(result, true);
        }
        #endregion
    }
}


