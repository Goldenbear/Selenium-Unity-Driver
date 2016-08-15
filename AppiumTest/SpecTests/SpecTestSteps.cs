using System;
using TechTalk.SpecFlow;

namespace AppiumTest.SpecTests
{
    [Binding]
    public class SpecTestSteps
    {
        [Given(@"I start the app with some (.*)")]
        public void GivenIStartTheAppWithSome(string p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
