//Generated Code
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

// ST: additional namespaces
using AppiumTests.Helpers;
using OpenQA.Selenium.Appium.HCP;
using OpenQA.Selenium.Appium; /* This is Appium */
using System.Diagnostics;
using System.Reflection;
using Xunit.Abstractions;
using System.Collections.Generic;
using Xunit;
using AppiumTests;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

namespace AppiumTests 
{
	public class RecordedTest 
	{
		// ST: my unit test
		[Fact]
		static void SeanTest() 
		{
			//AppiumHCPDriver<AppiumWebElement> wd = StartAppiumHCPDriver_IOS();
			AppiumHCPDriver<AppiumWebElement> wd = StartAppiumHCPDriver_Android();

			// ST: replace contents of this try clause with output from Appium inspector
			try 
			{
				wd.HCP().FindElement(By.Id("HCP-8af8f187-fc27-4437-9072-7162c00694bb")).Click();
				wd.HCP().FindElement(By.Id("HCP-05f19f10-6ca3-4926-b8d5-636e5883f263")).Click();
			} finally { wd.Quit(); }
		}

		// ST: start an iOS Appium HCP Driver with required device capabilities
		public static AppiumHCPDriver<AppiumWebElement> StartAppiumHCPDriver_IOS()
		{
			DesiredCapabilities capabilities = new DesiredCapabilities();
			//capabilities.SetCapability("autoWebview", false);
			//capabilities.SetCapability("browserName", String.Empty); 	// Leave empty otherwise you test on browsers
			capabilities.SetCapability("udid", "auto");					// "auto" if only one device connected to server
			capabilities.SetCapability("appium-version", "any");        // Not used but set to something
			capabilities.SetCapability("platformName", "iOS");          // IOSDriver sets this
			capabilities.SetCapability("platformVersion", "9.3");		// Required by iOS
			capabilities.SetCapability("hcp", "true");                  // Starts HCP on server
			capabilities.SetCapability("hcpHost", "http://192.168.0.5");// Server IP address. Actual address required for iOS.
			capabilities.SetCapability("hcpPort", "14812");             // Port HCP is listening on - 14812
			//capabilities.SetCapability("deviceName", "iPhone 6");		// Not required
			//capabilities.SetCapability("app", "/Users/Sean/Dev/Selenium-Unity-Driver/Appium/minimal.ipa");
			capabilities.SetCapability("app", "com.hutch.minimal");

			//RemoteWebDriver wd = new RemoteWebDriver(new Uri("http://localhost:4723/wd/hub"), capabilities);
			AppiumHCPDriver<AppiumWebElement> wd = new IOSDriver<AppiumWebElement>(new Uri("http://localhost:4723/wd/hub"), capabilities, TimeSpan.FromSeconds(360));
			wd.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
			WaitForHCP(wd);

			return wd;
		}

		// ST: start an Android Appium HCP Driver with required device capabilities
		public static AppiumHCPDriver<AppiumWebElement> StartAppiumHCPDriver_Android()
		{
			DesiredCapabilities capabilities = new DesiredCapabilities();
			//capabilities.SetCapability("autoWebview", false);
			//capabilities.SetCapability("browserName", String.Empty); 	// Leave empty otherwise you test on browsers
			capabilities.SetCapability("appium-version", "any");        // Not used but set to something
			//capabilities.SetCapability("platformName", "Android");	// AndroidDriver sets this
			capabilities.SetCapability("platformVersion", "");			// Leave empty on Android
			capabilities.SetCapability("hcp", "true");					// Starts HCP on server
			capabilities.SetCapability("hcpHost", "http://127.0.0.1");  // Server IP address eg. "http://127.0.0.1"
			capabilities.SetCapability("hcpPort", "14812");				// Port HCP is listening on - 14812
			capabilities.SetCapability("deviceName", "anything");		// Not used but set to something
			capabilities.SetCapability("app", "/Users/Sean/Dev/Selenium-Unity-Driver/Appium/minimal.apk");

			//RemoteWebDriver wd = new RemoteWebDriver(new Uri("http://localhost:4723/wd/hub"), capabilities);
			AppiumHCPDriver<AppiumWebElement> wd = new AndroidDriver<AppiumWebElement>(new Uri("http://localhost:4723/wd/hub"), capabilities, TimeSpan.FromSeconds(360));
			wd.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
			WaitForHCP(wd);

			return wd;
		}

		public static void WaitForHCP(AppiumHCPDriver<AppiumWebElement> wd)
		{
			WebDriverWait wait = new WebDriverWait(wd, TimeSpan.FromSeconds(10));
			bool result = wait.Until<bool>(ExpectedHCPConditions.HCPReady());

			Assert.Equal(result, true);
		}
	}
}
