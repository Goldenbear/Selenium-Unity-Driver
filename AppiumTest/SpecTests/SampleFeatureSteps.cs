using System;
using TechTalk.SpecFlow;

namespace AppiumTest
{
    [Binding]
    public class SampleFeatureSteps
    {
        [Given(@"I start the app with some (.*)")]
        public void GivenIStartTheAppWithSome(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"I have waiting for the app to have loaded")]
        public void GivenIHaveWaitingForTheAppToHaveLoaded()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"the element named (.*) is visible")]
        public void GivenTheElementNamedIsVisible(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"I click the element named (.*)")]
        public void WhenIClickTheElementNamed(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the element named (.*) is visible")]
        public void ThenTheElementNamedIsVisible(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"I save a screenshot")]
        public void ThenISaveAScreenshot()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
