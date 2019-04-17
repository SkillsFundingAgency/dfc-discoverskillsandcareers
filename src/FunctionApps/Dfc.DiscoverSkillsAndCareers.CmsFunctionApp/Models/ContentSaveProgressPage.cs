using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentSaveProgressPage : IContentPage
    {
        public string Title { get; set; } = "National Careers Service - Save Progress";
        public string PageTitle { get; set; } = "Save progress | National Careers Service";
        public string EmailPrompt { get; set; } = "What is your email?";
        public string EmailInputLabel { get; set; } = "Email";
        public string EmailInputButtonText { get; set; } = "Send";
        public string SmsPrompt { get; set; } = "What is your phone number?";
        public string SmsInputLabel { get; set; } = "Phone number";
        public string SmsInputButtonText { get; set; } = "Send";
        public string ReturnText { get; set; } = "Click the link in the text to return to your assessment";
        public string ContinueButtonText { get; set; } = "Continue";
        public string EmailSentHeader { get; set; } = "Check your email";
        public string SmsSentHeader { get; set; } = "Check your phone";
        public string EmailSentText { get; set; } = "An email has been sent to";
        public string SmsSentText { get; set; } = "A text has been sent to";
        public string SaveProgressHeader { get; set; } = "Save your progress";
        public string SaveProgressLabel { get; set; } = "Choose how to receive your results";
        public string SaveProgressOptionEmail { get; set; } = "Send me an email";
        public string SaveProgressOptionSms { get; set; } = "Send me a text";
        public string SaveProgressOptionReference { get; set; } = "Give me a reference number";
        public string SaveProgressReturnText { get; set; } = "Return to assessment";
        public string SaveProgressNoOptionSelectedMessage { get; set; } = "Please select an option to continue";
        public string ReferenceYourNumberText { get; set; } = "Your reference number";
        public string ReferenceReturnText { get; set; } = "Return to assessment";
        public string ReferenceInstructionsText { get; set; } = "The code above is your unique reference number. You can use this to:";
        public string ReferenceInstruction1Text { get; set; } = "Return to the assessment and resume your progress";
        public string ReferenceInstruction2Text { get; set; } = "Access your results after finishing the assessment";
        public string SmsInputInvalidMessage { get; set; } = "You must enter a phone number";

        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
