using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TechTalk.SpecFlow;
using System.IO;

namespace Dfc.CrossBrowserTests
{
    [Binding]
    public sealed class BrowserStack
    {
        private BrowserStackDriver bsDriver;
        private string[] tags;

        [BeforeScenario]
        public void BeforeScenario()
        {
            bsDriver = new BrowserStackDriver(ScenarioContext.Current);
            ScenarioContext.Current["bsDriver"] = bsDriver;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            bsDriver.Cleanup();
        }
    }

    public class BrowserStackDriver
    {
        private IWebDriver driver;
        private Local browserStackLocal;
        private string profile;
        private string environment;
        private ScenarioContext context;

        public BrowserStackDriver(ScenarioContext context)
        {
            this.context = context;
        }

        public IWebDriver Init(string profile, string environment)
        {
            NameValueCollection caps = ConfigurationManager.GetSection("capabilities/" + profile) as NameValueCollection;
            NameValueCollection settings = ConfigurationManager.GetSection("environments/" + environment) as NameValueCollection;

            DesiredCapabilities capability = new DesiredCapabilities();

            foreach (string key in caps.AllKeys)
            {
                capability.SetCapability(key, caps[key]);
            }

            foreach (string key in settings.AllKeys)
            {
                capability.SetCapability(key, settings[key]);
            }

            String username = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME");
            if (username == null)
            {
                username = ConfigurationManager.AppSettings.Get("user");
            }

            String accesskey = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY");
            if (accesskey == null)
            {
                accesskey = ConfigurationManager.AppSettings.Get("key");
            }

            capability.SetCapability("browserstack.user", username);
            capability.SetCapability("browserstack.key", accesskey);

            File.AppendAllText("C:\\Users\\Admin\\Desktop\\sf.log", "Starting local");

            if (capability.GetCapability("browserstack.local") != null && capability.GetCapability("browserstack.local").ToString() == "true")
            {
                browserStackLocal = new Local();
                List<KeyValuePair<string, string>> bsLocalArgs = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("key", accesskey)
                };
                browserStackLocal.start(bsLocalArgs);
            }

            File.AppendAllText("C:\\Users\\Admin\\Desktop\\sf.log", "Starting driver");
            driver = new RemoteWebDriver(new Uri("http://" + ConfigurationManager.AppSettings.Get("server") + "/wd/hub/"), capability);
            return driver;
        }

        public void Cleanup()
        {
            driver.Quit();
            if (browserStackLocal != null)
            {
                browserStackLocal.stop();
            }
        }
    }
}