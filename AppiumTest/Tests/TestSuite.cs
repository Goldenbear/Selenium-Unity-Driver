//--------------------------------------------------------------------------
//  <copyright file="TestSuite.cs">
//      Copyright (c) Andrea Tino. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace AppiumTests
{
    using System;
    using AppiumTests.Helpers;
    using AppiumTest.Framework;
    using OpenQA.Selenium; /* Appium is based on Selenium, we need to include it */
    using OpenQA.Selenium.Appium; /* This is Appium */
    using OpenQA.Selenium.Appium.Android;
    using OpenQA.Selenium.Appium.Interfaces; /* Not needed for commands shown here. It might be needed in single tests for automation */
    using OpenQA.Selenium.Appium.MultiTouch; /* Not needed for commands shown here. It might be needed in single tests for automation */
    using OpenQA.Selenium.Interactions; /* Not needed for commands shown here. It might be needed in single tests for automation */
    using OpenQA.Selenium.Remote;
    using System.Threading.Tasks;
    using System.Threading;
    using Xunit;
    using System.Diagnostics;
    using OpenQA.Selenium.Support.UI;
    using OpenQA.Selenium.Appium.HCP;
    using OpenQA.Selenium.Appium.iOS;
    using System.Collections.Generic;
    using Xunit.Extensions;
    using Xunit.Abstractions;
    using System.Reflection;

    public delegate AppiumHCPDriver<AppiumWebElement> CreateDriver();

    public class PMSmokeTestSuite : IDisposable
    {
        
        private static Uri g_testServerAddress = new Uri(TestServers.Server1);
        private static int g_commonPort = 14811;
        private static AppiumHCPDriver<AppiumWebElement> g_Driver;
       
        private static TimeSpan INIT_TIMEOUT_SEC = TimeSpan.FromSeconds(360); /* Change this to a more reasonable value */
        private static TimeSpan IMPLICIT_TIMEOUT_SEC = TimeSpan.FromSeconds(10); /* Change this to a more reasonable value */
        private static TimeSpan HCP_TIMEOUT_SEC = TimeSpan.FromSeconds(100); /* Change this to a more reasonable value */
        
        private static AppiumHCPDriver<AppiumWebElement> ConstructBootstrap()
        {
            if(g_Driver == null)
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
                capabilties.HCPPort = g_commonPort;

                capabilties.AssignAppiumCapabilities(ref capabilities);

                AppiumHCPDriver<AppiumWebElement> driver = new AndroidDriver<AppiumWebElement>(g_testServerAddress, capabilities, INIT_TIMEOUT_SEC);
                driver.Manage().Timeouts().ImplicitlyWait(IMPLICIT_TIMEOUT_SEC);

                g_Driver = driver;
            }

            return g_Driver;
        }

        private static AppiumHCPDriver<AppiumWebElement> ConstructAndroid()
        {
            if(g_Driver == null)
            {
                DesiredCapabilities capabilities = new DesiredCapabilities();
                
                TestCapabilities capabilties = new TestCapabilities();
                capabilties.App = "./minimal.apk";
                capabilties.AutoWebView = false; 
                capabilties.AutomationName = "minimal";
                capabilties.BrowserName = String.Empty; // Leave empty otherwise you test on browsers
                capabilties.DeviceName = "Android";
                capabilties.FwkVersion = "1.0"; // Not really needed
                capabilties.Platform = TestCapabilities.DevicePlatform.Android; // Or IOS
                capabilties.PlatformVersion = String.Empty; // Not really needed
                capabilties.SupportsHCP = true;
                capabilties.HCPHost = "http://127.0.0.1";
                capabilties.HCPPort = g_commonPort;

                capabilties.AssignAppiumCapabilities(ref capabilities);

                AppiumHCPDriver<AppiumWebElement> driver = new AndroidDriver<AppiumWebElement>(g_testServerAddress, capabilities, INIT_TIMEOUT_SEC);
                driver.Manage().Timeouts().ImplicitlyWait(IMPLICIT_TIMEOUT_SEC);

                g_Driver = driver;
            }

            return g_Driver;
        }

        private static AppiumHCPDriver<AppiumWebElement> ConstructIOS()
        {
            #if __MonoCS__

                if(g_Driver == null)
                {
                    DesiredCapabilities capabilities = new DesiredCapabilities();
                
                    TestCapabilities capabilties = new TestCapabilities();
                    capabilties.App = "./minimal.ipa";
                    capabilties.AutoWebView = false; 
                    capabilties.AutomationName = "minimal";
                    capabilties.BrowserName = String.Empty; // Leave empty otherwise you test on browsers
                    capabilties.DeviceName = "iOS";
                    capabilties.FwkVersion = "1.0"; // Not really needed
                    capabilties.Platform = TestCapabilities.DevicePlatform.IOS; // Or IOS
                    capabilties.PlatformVersion = String.Empty; // Not really needed
                    capabilties.SupportsHCP = true;
                    capabilties.HCPHost = "http://127.0.0.1";
                    capabilties.HCPPort = g_commonPort;

                    capabilties.AssignAppiumCapabilities(ref capabilities);

                    AppiumHCPDriver<AppiumWebElement> driver = new IOSDriver<AppiumWebElement>(testServerAddress, capabilities, INIT_TIMEOUT_SEC);
                    driver.Manage().Timeouts().ImplicitlyWait(IMPLICIT_TIMEOUT_SEC);

                    g_Driver = driver;
                }
            #else 
            #endif
                
            return g_Driver;
        }
        

        public static IEnumerable<object[]> OnDevices
        {
            get
            {
                // Or this could read from a file. :)
                return new[]
                {
                    new CreateDriver[] { ConstructAndroid },
                    #if __MonoCS__
                        new CreateDriver[] { ConstructIOS }
                    #else 
                    #endif
                };
            }
        }

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

        public static string ImageDirectory { get { return "./screenshots/"; } }
        public static string ImageHost { get { return "http://127.0.0.1/images/"; } }

        public void WriteScreenshot(string fileName = null)
        {
            if(fileName == null)
            {
                StackTrace st = new StackTrace();
                StackFrame sf = st.GetFrame(1);
                MethodBase testMethodName = sf.GetMethod();
                fileName = testMethodName.ToString();
            }
            var fullPath = String.Format("{0}/{1} - {2}.jpg",
                ImageDirectory,
                DateTime.Now.ToString("HH:mm:ss tt"),
                fileName);
            var urlPath = String.Format("{0}/{1} - {2}.jpg",
                ImageHost,
                DateTime.Now.ToString("HH:mm:ss tt"),
                fileName);

            var screenshot = g_Driver.GetScreenshot();
            screenshot.SaveAsFile(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            output.WriteLine(urlPath);
        }
        
        public readonly ITestOutputHelper output;

        public PMSmokeTestSuite(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void Dispose()
        {
            if(g_Driver != null)
            {
                g_Driver.Quit(); // Always quit, if you don't, next test session will fail
                g_Driver = null;
            }
        }
         
        /// <summary>
        /// Just a simple test to heck out Appium environment.
        /// </summary>
        [Theory, MemberData("OnDevices")]
        public void CheckDriverContext(CreateDriver constructor)
        {
            var driver = constructor();
            var context = driver.Context;

            // We have a context
            Assert.NotEmpty(context);
        }

        public void WaitforHCP(AppiumHCPDriver<AppiumWebElement> driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, HCP_TIMEOUT_SEC);
            bool result = wait.Until<bool>(ExpectedHCPConditions.HCPReady());

            Assert.Equal(result, true);
        } 
        
        [Theory, MemberData("OnDevices")]
        public void CheckHCPEnvironment(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
        } 
        
        [Theory, MemberData("OnDevices")]
        public AppiumWebElement CheckFindButton(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);

            // This is how to get all
            // var elementsList = driver.FindElementsByXPath("//*");
            
            AppiumWebElement button = FindButton(driver);

            Assert.False(button == null);
            return button;
        }

        private AppiumWebElement FindImage(AppiumHCPDriver<AppiumWebElement> driver)
        {
            WaitforHCP(driver);
            return driver.HCP().FindElementByName("Image");
        }  
        
        private AppiumWebElement FindButton(AppiumHCPDriver<AppiumWebElement> driver)
        {
            WaitforHCP(driver);
            return driver.HCP().FindElementByName("Button");
        }      

        [Theory, MemberData("OnDevices")]
        public AppiumWebElement CheckFindImage(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);

            // This is how to get all
            // var elementsList = driver.FindElementsByXPath("//*");
            
            AppiumWebElement image = FindImage(driver);

            Assert.False(image == null);
            return image;
        }

        
        [Theory, MemberData("OnDevices")]
        public void CheckClickButton(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement button = FindButton(driver);
            button.Click();
            
            AppiumWebElement image = FindImage(driver);
            var enabled = image.Enabled;

            Assert.True(enabled);
        }
        
        
        [Theory, MemberData("OnDevices")]
        public void CheckFindButtons(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);

            var buttons = driver.HCP().FindElementsByClassName("UnityEngine.UI.Button");

            Assert.True(buttons.Count > 0);
        }
        
        [Theory, MemberData("WithBootstrap")]
        public void Bootstrap_CheckFindAll(CreateDriver constructor)
        {
            var driver = constructor();
            Thread.Sleep(3000); 
            var all = driver.FindElementsByXPath("//*");

            Assert.True(all.Count > 0);
        }
        
        [Theory, MemberData("OnDevices")]
        public void CheckFindImages(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);

            var images = driver.HCP().FindElementsByClassName("UnityEngine.UI.Image");

            Assert.True(images.Count > 0);
        }

        [Theory, MemberData("OnDevices")]
        public void CheckFindTags(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);

            var images = driver.HCP().FindElementsByTagName("SomeTag");

            Assert.True(images.Count == 2);
        }
        
        [Theory, MemberData("OnDevices")]
        public void CheckButtonLocation(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement button = FindButton(driver);
            var location = button.Location;

            Assert.True(location.IsEmpty == false);
        }

        [Theory, MemberData("OnDevices")]
        public void CheckButtonSize(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement button = FindButton(driver);
            var size = button.Size;

            Assert.True(size.IsEmpty == false);
        }
        
        [Theory, MemberData("OnDevices")]
        public void CheckButtonDisplayed(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement button = FindButton(driver);
            var displayed = button.Displayed;

            Assert.True(displayed);
        }

        [Theory, MemberData("OnDevices")]
        public void CheckButtonEnabled(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement button = FindButton(driver);
            var enabled = button.Enabled;

            Assert.True(enabled);
        }
        
        [Theory, MemberData("OnDevices")]
        public void CheckImageDisplayed(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement image = FindImage(driver);
            var displayed = image.Displayed;

            Assert.False(displayed);
        }

        [Theory, MemberData("OnDevices")]
        public void CheckImageEnabled(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement image = FindImage(driver);
            var enabled = image.Enabled;

            Assert.False(enabled);
        }

        
        [Theory, MemberData("OnDevices")]
        public void CheckSwipeGesture(CreateDriver constructor)
        {           
            var driver = constructor();
            WaitforHCP(driver);
            driver.Swipe(500, 500, 1500, 1000, 5000);
            WriteScreenshot("CheckSwipeGestureColin"); // Explicit filename
            WriteScreenshot(); // Implicit using stack
        }

        
        [Theory, MemberData("OnDevices")]
        public void CheckPageSource(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            var source = driver.PageSource;
            var hcpSource = driver.HCP().PageSource;

            Assert.False(String.IsNullOrEmpty(hcpSource));
        }
    }
}
