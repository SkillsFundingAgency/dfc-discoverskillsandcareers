using System;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Dfc.DiscoverSkillsAndCareers.AcceptanceTests.Pages

{

    public class DYSACMainPage
    {
        private static IWebDriver driver;
        public static StringBuilder verificationErrors;
        private static string baseURL;
        private bool acceptNextAlert = true;

        private Boolean matches;

        [SetUp]
        public static void SetupTest()
        {
          
            driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            baseURL = "https://discover-skills-careers-dev.nationalcareersservice.org.uk/";

            verificationErrors = new StringBuilder();
        }

        [TearDown]
        public static void TeardownTest()
        {
            try
            {
                
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }


        public void OpenDYSACPage()
        {
            Console.WriteLine("BaseURL:" + baseURL);
            driver.Navigate().GoToUrl(baseURL);

        }

        public void StartDYSACAssessment()
        {
            try
            {
                //Look for the Start Assessment button
                Assert.AreEqual("Start assessment", driver.FindElement(By.LinkText("Start assessment")).Text);

            }
            catch (AssertionException e)
            {
                verificationErrors.Append(e.Message);
            }

            driver.FindElement(By.XPath("//*[@id='content']/div[3]/div/div[1]/a")).Click();
        }


        public void checkOnQuestion(string questionText)
        {

            try
            {
                //check user is shown first question
                Assert.AreEqual("I am comfortable telling people what they need to do", driver.FindElement(By.Id("question-heading")).Text);
            }
            catch (AssertionException e)
            {
                verificationErrors.Append(e.Message);
            }
            try
            {
                //check user is shown 0% at start
                Assert.AreEqual("0%", driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Back to start'])[1]/following::b[1]")).Text);
            }
            catch (AssertionException e)
            {
                verificationErrors.Append(e.Message);
            }

        }

        public void verfiyDisplayed(By objectXPath)
        {

            try
            {

                Assert.IsTrue(driver.FindElement(objectXPath).Displayed);

            }
            catch (AssertionException e)
            {
                verificationErrors.Append("VerfiyDisplyed error: " + e.Message + "  " + objectXPath + "<<<");
            }

        }

        public void clickObject(By objectXPath)
        {

            driver.FindElement(objectXPath).Click();

        }

        public void selectAnswer(string answerID)
        {

            driver.FindElement(By.XPath("//*[@id='question']/div[2]/div[" + answerID + "]/label")).Click();
            driver.FindElement(By.XPath("//*[@id='content']/div[2]/form/div[3]/button")).Click();

        }

        public void clickFilterQuestionYes()
        {
            driver.FindElement(By.Id("selected_answer-1")).Click();

        }

        public void clickRadioButton2()
        {
            driver.FindElement(By.Id("SelectedOption-2")).Click();
            //*[@id="content"]/div[2]/div/div/form/div/div/fieldset/div/div[2]/label

        }


        public string getReferenceNumber() 
        {
            string referenceNumber;

            referenceNumber = driver.FindElement(By.XPath("//*[@id='content']/div[2]/div/div/div[1]/p/span")).Text;

            return referenceNumber;
        }

        public void verifyText(string titleText, By titleXPath)
        { 
                    try
            {
                //check user is shown 0% at start
                Assert.AreEqual(titleText, driver.FindElement(titleXPath).Text);
            }
            catch (AssertionException e)
            {
                verificationErrors.Append(e.Message);
            }
        }


        public void verifyPartialText(string titleText, By titleXPath)
        {
           
            try
            {
                    
                matches = ((driver.FindElement(titleXPath).Text).Contains(titleText));
                Assert.IsTrue(matches);
            }
            catch (AssertionException e)
            {
                verificationErrors.Append(e.Message);
            }
        }


        public void enterText(string thisText, By titleXPath)
        {
            try
            {
                
                driver.FindElement(titleXPath).SendKeys(thisText);
            }
            catch (AssertionException e)
            {
                verificationErrors.Append(e.Message);
            }
        }


        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
    }
}

