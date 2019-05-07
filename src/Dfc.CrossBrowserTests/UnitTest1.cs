using System;
using System.Threading;
using Xunit;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Dfc.CrossBrowserTests
{
    [Binding]
    public class UnitTest1
    {
        private IWebDriver _driver;
        private BrowserStackDriver _bsDriver;

        public void SingleSteps() 
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
        }

        [Given(@"I am on the google page for (.*) and (.*)")]
        public void GivenIAmOnTheGooglePage(string profile, string environment)
        {
            _driver = _bsDriver.Init(profile, environment);
            _driver.Navigate().GoToUrl("https://www.google.com/ncr");
        }

        [When(@"I search for ""(.*)""")]
        public void WhenISearchFor(string keyword)
        {
            var q = _driver.FindElement(By.Name("q"));
            q.SendKeys(keyword);
            q.Submit();
        }

        [Then(@"I should see title ""(.*)""")]
        public void ThenIShouldSeeTitle(string title)
        {
            Thread.Sleep(5000);
            Assert.Equal(_driver.Title, title);
        }
    }
}
