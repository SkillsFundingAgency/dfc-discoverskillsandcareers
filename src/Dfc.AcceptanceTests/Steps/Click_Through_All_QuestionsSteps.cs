using System;
using Dfc.DiscoverSkillsAndCareers.AcceptanceTests.Pages;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Dfc.DiscoverSkillsAndCareers.AcceptanceTests.Steps
{
    [Binding]
    public class Click_Through_All_QuestionsSteps
    {
        private static string referenceNumber;

        private By titleLink = By.XPath("//*[@id='content']/div[2]/div/div[2]/div/h1");
        private By assessmentCompleteTitle = By.XPath("//*[@id='content']/div[2]/div/div/div/h1");
        private By seeResultsButton = By.XPath("//*[@id='content']/div[2]/div/div/a");
        private By yourResultsTitle = By.XPath("//*[@id='content']/div[2]/div/div[2]/div/h1");
        private By returnedToYourResultsTitle = By.XPath("//*[@id='content']/div[2]/div/div/div/h1");   //when used back button from filter Q
        private By printResults = By.XPath("//*[@id='content']/div[4]/div/div[1]/div/div[1]/div[1]/h2");
        private By noResultsMsg = By.XPath("//*[@id='content']/div[3]/div/div/div[1]/div/p");
        private By jobCategoriesAreReturnedText = By.XPath("//*[@id='content']/div[4]/div/div[2]/h2");
        private By startFilterQuestionsButton = By.XPath("//*[@id='app-results-list']/li[1]/div/div/a");
        private By reloadedFilterQuestionsButton =By.XPath("//*[@id='content']/div[4]/div[2]/div[1]/div[1]/div/a"); //when back button used previously

        private By filterQuestion = By.XPath("//*[@id='content']/div[2]/form/div/div[1]/div[1]/fieldset/div[1]/h1");

        private By filterNextQuestionButton = By.XPath("//*[@id='content']/div[2]/form/div/div[1]/div[2]/button");
        private By filterReturnButton = By.XPath("//*[@id='content']/div[1]/div[2]/a");

        private By jobRolesReturnedText = By.XPath("//*[@id='content']/div[4]/div[2]/div[1]/div[1]/div/p[1]");

        private By sendResultsButton = By.XPath("//*[@id='content']/div[4]/div/div[1]/div/div[1]/div[2]/form/button");                                                
        private By sendResultsContinue = By.XPath("//*[@id='content']/div[2]/div/div/form/button");
        private By returnToResultsFromRefNoPage = By.XPath("//*[@id='content']/div[2]/div/div/p/a");
        private By referenceNoContinueButton = By.XPath("//*[@id='content']/div[3]/div/div[2]/div[1]/form/button");

        private By enterRefNoField = By.XPath("//*[@id='code']");


        [Given(@"Set up test")]
        public void GivenSetUpTest()
        {
            Console.WriteLine("Setup");
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSACMainPage.SetupTest();
        }


        [Then(@"I teardown the test"), Scope(Tag = "smoke_test")]
        public void ThenITeardownTheTest()
        {
            Console.WriteLine("Teardown");
            DYSACMainPage DYSAC = new DYSACMainPage();
           
            DYSACMainPage.TeardownTest();
        }

        [Then(@"I teardown the test"), Scope(Tag = "adhoc_test")]
        public void ThenITeardownTheTestAdhoc()
        {
            Console.WriteLine("Browser left open for adhoc testing");
            //DYSACMainPage DYSAC = new DYSACMainPage();

            //DYSACMainPage.TeardownTest();
        }





        [Given(@"I have openned DYSAC main page")]
        public void GivenIHaveOpennedDYSACMainPage()
        {
            Console.WriteLine("Open DYSAC page");
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.OpenDYSACPage();
        }
        

        [When(@"I have selected to start assessment")]
        public void WhenIHaveSelectedToStartAssessment()
        {
            Console.WriteLine("Start Assessment");
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.StartDYSACAssessment();
        }


        [Then(@"I am on question page")]
        public void ThenIAmOnQuestionPage()
        {
            Console.Write("...");
        }



        [Then(@"the percentage complete progress is (.*)")]
        public void ThenThePercentageCompleteProgressIs(string percentageText)
        {
            Console.Write("% complete : " + percentageText);
        }




        [Then(@"I click on answer (.*)")]
        public void ThenIClickOnAnswer(string answerID)
        {
            Console.WriteLine("Click Answer:");
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.selectAnswer(answerID);
        }




        [Then(@"I am on Results Page")]
        public void ThenIAmOnResultsPage()
        {
            Console.WriteLine("Verfiy on Results Page:");
            DYSACMainPage DYSAC = new DYSACMainPage();

            DYSAC.verifyText("Your results", titleLink);
            //"//*[@id='question']/div[2]/div[" + answerID + "]/label"
        }



        [Then(@"I am on Assessment Complete Page")]
        public void ThenIAmOnAssessmentCompletePage()
        {
            Console.WriteLine("Verfiy on Assessment Complete Page:");
            DYSACMainPage DYSAC = new DYSACMainPage();

            DYSAC.verifyText("Assessment complete", assessmentCompleteTitle);
        }



        [Then(@"I click on see results")]
        public void ThenIClickOnSeeResults()
        {
            Console.WriteLine("CLick see results button");
            DYSACMainPage DYSAC = new DYSACMainPage();

            DYSAC.clickObject(seeResultsButton);
            
        }



        [Then(@"I am on the your results page")]
        public void ThenIAmOnTheYourResultsPage()
        {
            Console.WriteLine("Verify on Assessment Your Results Page:");
            DYSACMainPage DYSAC = new DYSACMainPage();

            DYSAC.verifyText("Your results", yourResultsTitle);
        }


        [Then(@"I am returned to the Results Page")]
        public void ThenIAmReturnedToTheResultsPage()
        {
            Console.WriteLine("Verify returned to Your Results Page:");
            DYSACMainPage DYSAC = new DYSACMainPage();

            DYSAC.verifyText("Your results", returnedToYourResultsTitle);
        }




        [Then(@"There is an option to print my results")]
        public void ThenThereIsAnOptionToPrintMyResults()
        {
            Console.WriteLine("Verfiy Print Results displayed:");
            DYSACMainPage DYSAC = new DYSACMainPage();

            DYSAC.verfiyDisplayed(printResults);
        }


        [Then(@"no Job Categories are found message shown")]
        public void ThenNoJobCategoriesAreFoundMessageShown()
        {
            Console.WriteLine("Check Msg No Job Categories Found shown:");
            DYSACMainPage DYSAC = new DYSACMainPage();

            DYSAC.verifyText("Because of your answers, we could not recommend any job categories. You might want to go through the assessment again to check that your responses were correct.", noResultsMsg);
        }


        [Then(@"some job categories are returned")]
        public void ThenSomeJobCategoriesAreReturned()
        {
            Console.WriteLine("Check some jobs are shown:");
            DYSACMainPage DYSAC = new DYSACMainPage();

            DYSAC.verifyPartialText("job categories that might suit you", jobCategoriesAreReturnedText);
        }



        [When(@"I click on the Filter Questions Button")]
        public void WhenIClickOnTheFilterQuestionsButton()
        {
            Console.WriteLine("Click on filter questions button:");
            DYSACMainPage DYSAC = new DYSACMainPage();

            DYSAC.clickObject(startFilterQuestionsButton);
        }


        [When(@"I click on the reloaded Filter Questions Button")]
        public void WhenIClickOnTheReloadedFilterQuestionsButton()
        {
            Console.WriteLine("Click on reloaded filter questions button:");
            DYSACMainPage DYSAC = new DYSACMainPage();

            DYSAC.clickObject(reloadedFilterQuestionsButton);
        }



        [Then(@"I am shown a filter question page")]
        public void ThenIAmShownAFilterQuestionPage()
        {
            Console.WriteLine("Verify on Filter Questions:");
           
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.verfiyDisplayed(filterQuestion);          
        }



        [When(@"I click on the filter Yes button")]
        public void WhenIClickOnTheFilterYesButton()
        {
            DYSACMainPage DYSAC = new DYSACMainPage();
            Console.WriteLine("Click yes on filter button");
            DYSAC.clickFilterQuestionYes();
        }



        [When(@"click on the filter next question button")]
        public void WhenClickOnTheFilterNextQuestionButton()
        {
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.clickObject(filterNextQuestionButton);
        }



        [Then(@"I am shown Job Roles returned by filter question criteria")]
        public void ThenIAmShownJobRolesReturnedByFilterQuestionCriteria()
        {
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.verifyPartialText("that might suit you", jobRolesReturnedText);
        }



        [When(@"I click on Return Button")]
        public void WhenIClickOnReturnButton()
        {
            Console.WriteLine("Click back to results button:");


            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.clickObject(filterReturnButton);

        }



        [Then(@"I click on send results button")]
        public void ThenIClickOnSendResultsButton()
        {
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.clickObject(sendResultsButton);
        }

        [Then(@"I choose option to generate reference number")]
        public void ThenIChooseOptionToGenerateReferenceNumber()
        {
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.clickRadioButton2();
            DYSAC.clickObject(sendResultsContinue);
        }

        [Then(@"I am presented with a reference number")]
        public void ThenIAmPresentedWithAReferenceNumber()
        {
            DYSACMainPage DYSAC = new DYSACMainPage();
            referenceNumber = DYSAC.getReferenceNumber();
            Console.WriteLine("Got refernce no: " + referenceNumber);
        }


        [Then(@"I click to return to assessment from reference number page")]
        public void ThenIClickToReturnToAssessmentFromReferenceNumberPage()
        {
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.clickObject(returnToResultsFromRefNoPage);
        }



        [When(@"I re-open the DYSAC main start page")]
        public void WhenIRe_OpenTheDYSACMainStartPage()
        {
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.OpenDYSACPage();
        }


        [When(@"enter my reference number")]
        public void WhenEnterMyReferenceNumber()
        {
            DYSACMainPage DYSAC = new DYSACMainPage();
            DYSAC.enterText(referenceNumber, enterRefNoField);
            DYSAC.clickObject(referenceNoContinueButton);
        }


    }
}
