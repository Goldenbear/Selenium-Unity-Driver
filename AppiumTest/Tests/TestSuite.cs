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
    using System.Text.RegularExpressions;

    public delegate AppiumHCPDriver<AppiumWebElement> CreateDriver();
    
    public class PMSmokeTestSuite : IDisposable
    {
        private static Uri APPIUM_SERVER_URI = new Uri(TestServers.Server1);
        private static int HCP_PORT = 14812;
        private static TimeSpan INIT_TIMEOUT_SEC = TimeSpan.FromSeconds(360); /* Change this to a more reasonable value */
        private static TimeSpan IMPLICIT_TIMEOUT_SEC = TimeSpan.FromSeconds(10); /* Change this to a more reasonable value */
        private static TimeSpan HCP_TIMEOUT_SEC = TimeSpan.FromSeconds(100); /* Change this to a more reasonable value */
        
        
        ////////////////////////////////////////////////////////////
        // @brief This driver is used to communicate with Appium. You
        // must close this properly or the server will be in an error
        // state.  This is done in Dispose.
        ////////////////////////////////////////////////////////////
        private static AppiumHCPDriver<AppiumWebElement> g_Driver;


        #region Driver Construction - Used to initialize Appium
        ////////////////////////////////////////////////////////////
        // @brief Non-HCP sample to pull data and do sanity tests.
        ////////////////////////////////////////////////////////////
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
                capabilties.HCPPort = HCP_PORT;

                capabilties.AssignAppiumCapabilities(ref capabilities);

                AppiumHCPDriver<AppiumWebElement> driver = new AndroidDriver<AppiumWebElement>(APPIUM_SERVER_URI, capabilities, INIT_TIMEOUT_SEC);
                driver.Manage().Timeouts().ImplicitlyWait(IMPLICIT_TIMEOUT_SEC);

                g_Driver = driver;
            }

            return g_Driver;
        }

        ////////////////////////////////////////////////////////////
        // @brief Constructs an Android driver looking for minimal.apk
        ////////////////////////////////////////////////////////////
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
                capabilties.HCPPort = HCP_PORT;

                capabilties.AssignAppiumCapabilities(ref capabilities);

                AppiumHCPDriver<AppiumWebElement> driver = new AndroidDriver<AppiumWebElement>(APPIUM_SERVER_URI, capabilities, INIT_TIMEOUT_SEC);
                driver.Manage().Timeouts().ImplicitlyWait(IMPLICIT_TIMEOUT_SEC);

                g_Driver = driver;
            }

            return g_Driver;
        }

        ////////////////////////////////////////////////////////////
        // @brief Constructs an iOS driver looking for minimal.apk.
        // Only works on OSX
        ////////////////////////////////////////////////////////////
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
        #endregion

        #region Test Parameters - Driver Lists
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
                    new CreateDriver[] { ConstructAndroid },
                    #if __MonoCS__
                        new CreateDriver[] { ConstructIOS }
                    #else 
                    #endif
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


        #region Screenshots
        private static string IMAGE_DIRECTORY { get { return "screenshots"; } }
        private static string IMAGE_HOST { get { return "http://127.0.0.1/images/"; } }

        private static string MakeValidFilename(string fileName)
        {
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
        private void WriteScreenshot(string fileName = null)
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

            var screenshot = g_Driver.GetScreenshot();
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
        public readonly ITestOutputHelper m_output;

        public void IssueCommand(string comment, Action command)
        {
            m_output.WriteLine(comment);
            command();
        }

        public object IssueCommand(string comment, Func<object> command)
        {
            m_output.WriteLine(comment);
            return command();
        }

        public PMSmokeTestSuite(ITestOutputHelper output)
        {
            this.m_output = output;
        }
        #endregion

        #region Cleanup
        public void Dispose()
        {
            if(g_Driver != null)
            {
                g_Driver.Quit(); // Always quit, if you don't, next test session will fail
                g_Driver = null;
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
        private void WaitforHCP(AppiumHCPDriver<AppiumWebElement> driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, HCP_TIMEOUT_SEC);
            bool result = wait.Until<bool>(ExpectedHCPConditions.HCPReady());

            Assert.Equal(result, true);
        } 

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
        private AppiumWebElement FindImage(AppiumHCPDriver<AppiumWebElement> driver)
        {
            WaitforHCP(driver);
            return IssueCommand("Looking for Image", () => { return driver.HCP().FindElementByName("Image"); }) as AppiumWebElement;
        }  
        
        private AppiumWebElement FindButton(AppiumHCPDriver<AppiumWebElement> driver)
        {
            WaitforHCP(driver);
            return IssueCommand("Looking for Button", () => { return driver.HCP().FindElementByName("Button"); }) as AppiumWebElement;
        }
        #endregion

        #region Tests

        #region With Logging using IssueCommand
        
        ////////////////////////////////////////////////////////////
        // @brief This should show a trail of particles in the 
        // screenshot
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckSwipeGesture(CreateDriver constructor)
        {           
            var driver = constructor();
            IssueCommand("Waiting for HCP", () => { WaitforHCP(driver); });
            IssueCommand("Swiping screen", () => { driver.Swipe(500, 500, 1500, 1000, 5000); });
            WriteScreenshot("CheckSwipeGestureColin"); // Explicit filename
            WriteScreenshot(); // Implicit using stack
        }

        ////////////////////////////////////////////////////////////
        // @brief Should return pagesource for both appium and hcp
        // in xml
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckPageSource(CreateDriver constructor)
        {
            var driver = constructor();
            IssueCommand("Waiting for HCP", () => { WaitforHCP(driver); });
            var source = IssueCommand("Asking for page source", () => { return driver.PageSource; }) as String;
            var hcpSource = IssueCommand("Asking for hcp page source", () => { return driver.HCP().PageSource; }) as String;

            Assert.False(String.IsNullOrEmpty(hcpSource));
        }

        #endregion

        #region Without Logging
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

            AppiumWebElement button = FindButton(driver);

            Assert.False(button == null);
            return button;
        }

        [Theory, MemberData("OnDevices")]
        public AppiumWebElement CheckFindImage(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);

            AppiumWebElement image = FindImage(driver);

            Assert.False(image == null);
            return image;
        }

        ////////////////////////////////////////////////////////////
        // @brief A black unity logo should appear above the button
        // when clicked.
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckClickButton(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);

            WriteScreenshot();
            AppiumWebElement button = FindButton(driver);
            button.Click();
            
            AppiumWebElement image = FindImage(driver);
            var enabled = image.Enabled;
            
            WriteScreenshot();
            Assert.True(enabled);
        }

        
        [Theory, MemberData("OnDevices")]
        public void CheckHoldButton(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement button = FindButton(driver);

            Thread.Sleep(2000); // Wait for clear screen
            WriteScreenshot();

            // In this version we send the raw driver our touch request.
            // If you compare screenshots, you will see that the touch is
            // not at the button location.  This is due to an inversion of
            // the y-coordinate
            driver.PerformTouchAction(
                new TouchAction (driver)
                .Press (button.Location.X, driver.Manage().Window.Size.Height - button.Location.Y)
                .Wait(10000)
                .Release());
            
            
            // Here is the HCP version, which is similar but can
            // take a button.  This avoids the coordinate system 
            // conversion above, and uses unity events to touch
            // the location on screen
            // TODO - Needs bug fixing
            /*
            driver.HCP().PerformTouchAction(
                new TouchAction (driver)
                .Press (button)
                .Wait(1000)
                .Release());
            */

            WriteScreenshot();

        }

        ////////////////////////////////////////////////////////////
        // @brief Enters text into a field using the OS keyboard
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckEnterText(CreateDriver constructor)
        {
            var keys = "ABCDEFHHIGJLMNOPQRSTUVWXYZ";
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement textField = driver.HCP().FindElementByName("TextField");
            driver.PerformTouchAction(
                new TouchAction (driver)
                .Press (textField.Location.X, driver.Manage().Window.Size.Height - textField.Location.Y)
                .Wait(100)
                .Release()
                );
            driver.Keyboard.SendKeys(keys);
            

            Assert.Equal(textField.Text, keys);
        }
        
        ////////////////////////////////////////////////////////////
        // @brief Enters text into a field by setting it directly
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckSetText(CreateDriver constructor)
        {
            var keys = "ABCDEFHHIGJLMNOPQRSTUVWXYZ";
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement textField = driver.HCP().FindElementByName("TextField");
            textField.SendKeys(keys);
            WriteScreenshot();

            Assert.Equal(textField.Text, keys);
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
        
        ////////////////////////////////////////////////////////////
        // @brief In world coordinate system.  Int values
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckButtonLocation(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement button = FindButton(driver);
            var location = button.Location;

            Assert.True(location.IsEmpty == false);
        }

        ////////////////////////////////////////////////////////////
        // @brief In world coordinate system.  Int values.
        ////////////////////////////////////////////////////////////
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
            
            WriteScreenshot();
            Assert.True(displayed);
        }

        [Theory, MemberData("OnDevices")]
        public void CheckButtonEnabled(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement button = FindButton(driver);
            var enabled = button.Enabled;
            
            WriteScreenshot();
            Assert.True(enabled);
        }
        
        ////////////////////////////////////////////////////////////
        // @brief Displayed means activeAndEnabled
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckImageDisplayed(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement image = FindImage(driver);
            var displayed = image.Displayed;
            
            WriteScreenshot();
            Assert.False(displayed);
        }

        ////////////////////////////////////////////////////////////
        // @brief Enabled means activeSelf
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckImageEnabled(CreateDriver constructor)
        {
            var driver = constructor();
            WaitforHCP(driver);
            AppiumWebElement image = FindImage(driver);
            var enabled = image.Enabled;
                        
            WriteScreenshot();
            Assert.False(enabled);
        }
        #endregion

        #endregion
    }
}
