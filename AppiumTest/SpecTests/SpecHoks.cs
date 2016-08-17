using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using AppiumTest.Framework;

// Ref: http://www.specflow.org/documentation/Hooks/
namespace AppiumTest.SpecTests
{
    ////////////////////////////////////////////////////////////
    // @brief These hooks are called before and after each
    // feature tests.  I make sure that the driver is cleaned
    // up properly following the end of each scenario and
    // assume that its set up by a feature step.  Note that
    // you can do some cool stuff with this, see the ref above.
    ////////////////////////////////////////////////////////////
    class SpecHoks
    {
        [BeforeScenario(Order = 0)]
        public static void DoNothing()
        {
            // we need to run this first
            // if you want to drive device from a shell script
            // have it read input data here to set up the
            // appium driver
        }

        [AfterScenario(Order = 100)]
        public static void ReleaseDriver()
        {
            TestFramework.ReleaseDriver();
        }
    }
}
