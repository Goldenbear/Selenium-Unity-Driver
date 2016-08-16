using System;
using TechTalk.SpecFlow;
using AppiumTest.Framework;

namespace AppiumTest.SpecTests
{
    [Binding]
    public class SampleFeatureSteps
    {
        [Given(@"I start the app with driver type (.*)")]
        public void GivenIStartTheAppWithDriverType(string p0)
        {
            switch(p0)
            {
                case "SampleAndroid":
                    TestFramework.ConstructAndroidDriver();
                    break;
                case "SampleIOS":
                    TestFramework.ConstructIOSDriver();
                    break;
            }
        }
        
        [Given(@"I have waited for the app to have loaded")]
        public void GivenIHaveWaitedForTheAppToHaveLoaded()
        {
            TestFramework.WaitForHCP();
        }
    }
}
