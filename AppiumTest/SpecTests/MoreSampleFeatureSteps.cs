using System;
using TechTalk.SpecFlow;
using AppiumTest.Framework;
using Xunit;
using System.Diagnostics;

using TechTalk.SpecFlow.Tracing;

namespace AppiumTest
{
    [Binding]
    public class MoreSampleFeatureSteps
    {
        [Given(@"the element named (.*) is visible")]
        public void GivenTheElementNamedIsVisible(string p0)
        {
            var element = TestFramework.FindHCPElement(p0);
            
            Assert.True(element.Displayed);
        }
        
        [When(@"I click the element named (.*)")]
        public void WhenIClickTheElementNamed(string p0)
        {
            var element = TestFramework.FindHCPElement(p0);
            element.Click();
        }
        
        [Then(@"the element named (.*) is visible")]
        public void ThenTheElementNamedIsVisible(string p0)
        {
            var element = TestFramework.FindHCPElement(p0);
            
            Assert.True(element.Displayed);
        }
        
        [Then(@"I save a screenshot")]
        public void ThenISaveAScreenshot()
        {
            // The trace here is a bit awkward, but the TakeScreenshot
            // function returns a string denoting where it is stored.
            // So this will print that location directly to our log.
            string url = TestFramework.TakeScreenshot();
            DefaultListener.xUnitOutput.WriteLine(url);            
        }
    }
}
