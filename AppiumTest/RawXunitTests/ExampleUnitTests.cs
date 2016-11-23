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

    public class ExampleUnitTests : IDisposable
    {
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
                    //new CreateDriver[] { TestFramework.ConstructAndroidDriver },
                    new CreateDriver[] { TestFramework.ConstructIOSDriver }
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
                    new CreateDriver[] { TestFramework.ConstructBootstrapDriver },
                };
            }
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
        #endregion


        #region Constructor / Destructor
        public ExampleUnitTests(ITestOutputHelper output)
            // Dependency injection from xUnit     ^^^^
        {
            this.m_output = output;
            TestFramework.g_driver = null;
        }
        
        public void Dispose()
        {
            TestFramework.ReleaseDriver();
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
        private AppiumWebElement FindSampleImage()
        {
            WaitForHCP();
            return IssueCommand("Looking for Image", () => { return TestFramework.FindHCPElement("Image"); }) as AppiumWebElement;
        }  
        
        private AppiumWebElement FindSampleButton()
        {
            WaitForHCP();
            return IssueCommand("Looking for Button", () => { return TestFramework.FindHCPElement("Button"); }) as AppiumWebElement;
        }

        private void TakeScreenshot(string filename = null)
        {
            m_output.WriteLine(TestFramework.TakeScreenshot(filename));
        }

        private void WaitForHCP()
        {
            TestFramework.WaitForHCP();
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
            IssueCommand("Waiting for HCP", () => { WaitForHCP(); });
            IssueCommand("Swiping screen", () => { driver.Swipe(500, 500, 1500, 1000, 5000); });
            TakeScreenshot("CheckSwipeGestureColin"); // Explicit filename
            TakeScreenshot(); // Implicit using stack
        }

        ////////////////////////////////////////////////////////////
        // @brief Should return pagesource for both appium and hcp
        // in xml
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckPageSource(CreateDriver constructor)
        {
            var driver = constructor();
            IssueCommand("Waiting for HCP", () => { WaitForHCP(); });
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
            TestFramework.g_driver = constructor();
            WaitForHCP();
        } 
        
        [Theory, MemberData("OnDevices")]
        public AppiumWebElement CheckFindButton(CreateDriver constructor)
        {
            TestFramework.g_driver = constructor();
            WaitForHCP();

            AppiumWebElement button = FindSampleButton();

            Assert.False(button == null);
            return button;
        }

        [Theory, MemberData("OnDevices")]
        public AppiumWebElement CheckFindImage(CreateDriver constructor)
        {
            TestFramework.g_driver = constructor();
            WaitForHCP();

            AppiumWebElement image = FindSampleImage();

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
            WaitForHCP();

            TakeScreenshot();
            AppiumWebElement button = FindSampleButton();
            button.Click();
            
            AppiumWebElement image = FindSampleImage();
            var enabled = image.Enabled;
            
            TakeScreenshot();
            Assert.True(enabled);
        }

        
        [Theory, MemberData("OnDevices")]
        public void CheckHoldButton(CreateDriver constructor)
        {
            var driver = constructor();
            WaitForHCP();
            AppiumWebElement button = FindSampleButton();

            Thread.Sleep(2000); // Wait for clear screen
            TakeScreenshot();

            // In this version we send the raw driver our touch request.
            // If you compare screenshots, you will see that the touch is
            // not at the button location.  This is due to an inversion of
            // the y-coordinate
            driver.PerformTouchAction(
                new TouchAction (driver)
                .Press (button.Location.X, button.Location.Y)
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

            TakeScreenshot();
        
        }

        ////////////////////////////////////////////////////////////
        // @brief Enters text into a field using the OS keyboard
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckEnterText(CreateDriver constructor)
        {
            var keys = "ABCDEFHHIGJLMNOPQRSTUVWXYZ";
            var driver = constructor();
            WaitForHCP();
            AppiumWebElement textField = driver.HCP().FindElementByName("TextField");
            driver.PerformTouchAction(
                new TouchAction (driver)
                .Press (textField.Location.X, textField.Location.Y)
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
            WaitForHCP();
            AppiumWebElement textField = driver.HCP().FindElementByName("TextField");
            textField.SendKeys(keys);
            TakeScreenshot();

            Assert.Equal(textField.Text, keys);
        }
        
        [Theory, MemberData("OnDevices")]
        public void CheckFindButtons(CreateDriver constructor)
        {
            var driver = constructor();
            WaitForHCP();

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
            WaitForHCP();

            var images = driver.HCP().FindElementsByClassName("UnityEngine.UI.Image");

            Assert.True(images.Count > 0);
        }

        [Theory, MemberData("OnDevices")]
        public void CheckFindTags(CreateDriver constructor)
        {
            var driver = constructor();
            WaitForHCP();

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
            WaitForHCP();
            AppiumWebElement button = FindSampleButton();
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
            WaitForHCP();
            AppiumWebElement button = FindSampleButton();
            var size = button.Size;

            Assert.True(size.IsEmpty == false);
        }
        
        [Theory, MemberData("OnDevices")]
        public void CheckButtonDisplayed(CreateDriver constructor)
        {
            var driver = constructor();
            WaitForHCP();
            AppiumWebElement button = FindSampleButton();
            var displayed = button.Displayed;
            
            TakeScreenshot();
            Assert.True(displayed);
        }

        [Theory, MemberData("OnDevices")]
        public void CheckButtonEnabled(CreateDriver constructor)
        {
            var driver = constructor();
            WaitForHCP();
            AppiumWebElement button = FindSampleButton();
            var enabled = button.Enabled;
            
            TakeScreenshot();
            Assert.True(enabled);
        }
        
        ////////////////////////////////////////////////////////////
        // @brief Displayed means activeAndEnabled
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckImageDisplayed(CreateDriver constructor)
        {
            var driver = constructor();
            WaitForHCP();
            AppiumWebElement image = FindSampleImage();
            var displayed = image.Displayed;
            
            TakeScreenshot();
            Assert.False(displayed);
        }

        ////////////////////////////////////////////////////////////
        // @brief Enabled means activeSelf
        ////////////////////////////////////////////////////////////
        [Theory, MemberData("OnDevices")]
        public void CheckImageEnabled(CreateDriver constructor)
        {
            var driver = constructor();
            WaitForHCP();
            AppiumWebElement image = FindSampleImage();
            var enabled = image.Enabled;
                        
            TakeScreenshot();
            Assert.False(enabled);
        }

        #endregion

        #endregion
    }
}
