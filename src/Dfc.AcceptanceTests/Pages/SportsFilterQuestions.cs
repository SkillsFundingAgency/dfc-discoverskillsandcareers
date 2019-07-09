using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Linq;

// not in use - use if more long-winded question tests added

namespace DYSAC_Tests

{
    class SportsFilterQuestions
    {
        private static IWebDriver driver;
        public static StringBuilder verificationErrors;
        private static string baseURL;
        private bool acceptNextAlert = true;
        private string textToMatch;
        private Boolean matches;



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
                verificationErrors.Append(e.Message);
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
