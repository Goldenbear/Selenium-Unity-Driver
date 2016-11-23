// ST: Which unit test framework to use. Add relevant Packages to project to use.
#define USE_NUNIT
//#define USE_XUNIT

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
using System.Collections.Generic;
using AppiumTests;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

#if USE_NUNIT
using NUnit.Framework;
#elif USE_XUNIT
using Xunit;
#endif

namespace AppiumTests 
{
	public class RecordedTest 
	{
		// The currently running Appium web-driver
		static AppiumHCPDriver<AppiumWebElement> wd = null;

		// ST: my unit test
#if USE_NUNIT
		[Test]
#elif USE_XUNIT
		[Fact]
#endif
		public static void SeanTest() 
		{
			//StartAppiumHCPDriver_IOS();
			StartAppiumHCPDriver_Android();

			// ST: replace contents of this try clause with output from Appium inspector
			try 
			{
				wd.HCP().FindElement(By.Id("HCP-8af8f187-fc27-4437-9072-7162c00694bb")).Click();
				wd.GetScreenshot();
				wd.HCP().FindElement(By.Id("HCP-05f19f10-6ca3-4926-b8d5-636e5883f263")).Click();
				wd.GetScreenshot();
			} finally { wd.Quit(); }
		}

		// ST: start an iOS Appium HCP Driver with required device capabilities
		public static void StartAppiumHCPDriver_IOS()
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

			//wd = new RemoteWebDriver(new Uri("http://localhost:4723/wd/hub"), capabilities);
			wd = new IOSDriver<AppiumWebElement>(new Uri("http://localhost:4723/wd/hub"), capabilities, TimeSpan.FromSeconds(360));
			wd.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
			WaitForHCP(wd);
		}

		// ST: start an Android Appium HCP Driver with required device capabilities
		public static void StartAppiumHCPDriver_Android()
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

			//wd = new RemoteWebDriver(new Uri("http://localhost:4723/wd/hub"), capabilities);
			wd = new AndroidDriver<AppiumWebElement>(new Uri("http://localhost:4723/wd/hub"), capabilities, TimeSpan.FromSeconds(360));
			wd.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
			WaitForHCP(wd);
		}

		public static void WaitForHCP(AppiumHCPDriver<AppiumWebElement> wd)
		{
			WebDriverWait wait = new WebDriverWait(wd, TimeSpan.FromSeconds(10));
			bool result = wait.Until<bool>(ExpectedHCPConditions.HCPReady());

#if USE_NUNIT
			Assert.IsTrue(result);
#elif USE_XUNIT
			Assert.Equal(result, true);
#endif
		}

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

			if (fileName == null)
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

			var screenshot = wd.GetScreenshot();
			screenshot.SaveAsFile(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);

			return urlPath;
		}
		#endregion
	}
}
