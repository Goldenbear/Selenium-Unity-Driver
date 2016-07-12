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

    public class PMSmokeTestSuite : IDisposable
    {
        private AndroidDriver<AppiumWebElement> driver;

        private static Uri testServerAddress = new Uri(TestServers.Server1);
        private static TimeSpan INIT_TIMEOUT_SEC = TimeSpan.FromSeconds(360); /* Change this to a more reasonable value */
        private static TimeSpan IMPLICIT_TIMEOUT_SEC = TimeSpan.FromSeconds(10); /* Change this to a more reasonable value */

        public PMSmokeTestSuite()
        {
            DesiredCapabilities capabilities = new DesiredCapabilities();
            TestCapabilities testCapabilities = new TestCapabilities();

            testCapabilities.App = "./minimal.apk";
            testCapabilities.AutoWebView = false; 
            testCapabilities.AutomationName = "minimal";
            testCapabilities.BrowserName = String.Empty; // Leave empty otherwise you test on browsers
            testCapabilities.DeviceName = "Android Emulator";
            testCapabilities.FwkVersion = "1.0"; // Not really needed
            testCapabilities.Platform = TestCapabilities.DevicePlatform.Android; // Or IOS
            testCapabilities.PlatformVersion = String.Empty; // Not really needed
            testCapabilities.SupportsHCP = true;
            testCapabilities.HCPHost = "http://127.0.0.1";

            testCapabilities.AssignAppiumCapabilities(ref capabilities);
            driver = new AndroidDriver<AppiumWebElement>(testServerAddress, capabilities, INIT_TIMEOUT_SEC);
            driver.Manage().Timeouts().ImplicitlyWait(IMPLICIT_TIMEOUT_SEC);
        }

        public void Dispose()
        {
            driver.Quit(); // Always quit, if you don't, next test session will fail
        }

        /// <summary>
        /// Just a simple test to heck out Appium environment.
        /// </summary>
        [Fact]
        public void CheckTestEnvironment()
        {
            var context = driver.Context;

            // We have a context
            Assert.NotEmpty(context);

            // UnityPlayer active
            Assert.Equal("com.unity3d.player.UnityPlayerActivity", driver.CurrentActivity);
        }

        [Fact]
        public async Task CheckHCPEnvironment()
        {

        } 
    }
}
