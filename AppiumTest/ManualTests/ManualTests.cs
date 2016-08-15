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

    public class PMSmokeTestSuite : TestSuite
    {
        // Unfortnately these need to be static
        public static new IEnumerable<object[]> OnDevices { get; } = TestSuite.OnDevices;
        public static new IEnumerable<object[]> WithBootstrap { get; } = TestSuite.WithBootstrap;
         

        public PMSmokeTestSuite(ITestOutputHelper output) : base(output)
            // Dependency injection from xUnit     ^^^^
        {
        }


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
