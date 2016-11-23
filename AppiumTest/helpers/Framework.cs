//--------------------------------------------------------------------------
//  <copyright file="Framework.cs">
//      Copyright (c) Andrea Tino. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------


using Xunit;
[assembly: CollectionBehavior(MaxParallelThreads = 1)]   // IMPORTANT!!!
    // The above turns off parallel execution

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
            this.HCPPort = 14812;
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

    public sealed class TestFramework
        // DevNote: There is a lot of details here, so I chose to group vars and 
        // methods by purpose so that you might better digest what is going on.
    {
        #region Communication Defaults
        private static Uri APPIUM_SERVER_URI = new Uri(TestServers.Server1);
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
        public static AppiumHCPDriver<AppiumWebElement> g_driver;
        #endregion
        

        #region Screenshots
        private static string IMAGE_DIRECTORY { get { return "screenshots"; } }                     // Filesystem
        private static string IMAGE_HOST { get { return "bin/Debug/screenshots"; } }    // URL

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
        public static string TakeScreenshot(string fileName = null)
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

            // Below is a sample "click to expand" html code block.  You could add code
            // to scale to a max and preserve ration etc.
            var urlPath = String.Format("\\<A HREF=\"{0}/{1} - {2:yyyy-MM-dd_hh-mm-ss-tt}.jpg\"\\>\\<IMG HEIGHT=300 WIDTH=200 SRC=\"{0}/{1} - {2:yyyy-MM-dd_hh-mm-ss-tt}.jpg\"\\>\\</A\\>",
                IMAGE_HOST,
                fileName,
                DateTime.Now);

            var screenshot = g_driver.GetScreenshot();
            screenshot.SaveAsFile(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);

            return urlPath;
        }
        #endregion

        

        #region Driver Construction - Used to initialize Appium
        ////////////////////////////////////////////////////////////
        // @brief Non-HCP sample to pull data and do sanity tests.
        ////////////////////////////////////////////////////////////
        public static AppiumHCPDriver<AppiumWebElement> ConstructBootstrapDriver()
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
        public static AppiumHCPDriver<AppiumWebElement> ConstructAndroidDriver()
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
        public static AppiumHCPDriver<AppiumWebElement> ConstructIOSDriver()
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
                capabilties.PlatformVersion = "9.3"; // Must match device
                capabilties.SupportsHCP = true;
                capabilties.HCPHost = "http://192.168.0.5";
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


        
        #region Cleanup
        ////////////////////////////////////////////////////////////
        // @brief This is necessary and will cause the Appium server
        // to enter an error state is missed.
        ////////////////////////////////////////////////////////////
        public static void ReleaseDriver()
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
        // @brief Returns the first element found with the name  
        // "Image".  Take special note of the HCP() call.  This call
        // will set a bool that is referenced on the next command.  It
        // is a bit hacky, but avoids MAJOR code duplication and 
        // headache.  This does not work
        // var hcp = driver.HCP();
        // hcp.SomeHCPCall(); <- will work as expected
        // hcp.SomeOtherHCPCall(); <- HCP state is already consumed,
        //                            and will be a regular appium
        //                            command.
        // You should always inline the HCP call
        // driver.HCP().SomeHCPCall();
        // driver.HCP().SomeOtherHCPCall();
        ////////////////////////////////////////////////////////////
        public static AppiumWebElement FindHCPElement(string name)
        {
            TestFramework.WaitForHCP();
            return g_driver.HCP().FindElementByName(name) as AppiumWebElement;
        }  

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
        public static void WaitForHCP()
        {
            WebDriverWait wait = new WebDriverWait(g_driver, HCP_TIMEOUT_SEC);
            bool result = wait.Until<bool>(ExpectedHCPConditions.HCPReady());

            Assert.Equal(result, true);
        }
        #endregion
    }
}


