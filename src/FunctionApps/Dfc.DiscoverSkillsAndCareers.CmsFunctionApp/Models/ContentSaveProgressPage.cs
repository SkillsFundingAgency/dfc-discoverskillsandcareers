﻿using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentSaveProgressPage : IContentPage
    {
        public string Title { get; set; } 
        public string PageTitle { get; set; } = "Save progress | National Careers Service";
        public string EmailPrompt { get; set; } = "Email address";
        public string EmailInputLabel { get; set; } = "Email";
        public string EmailInputButtonText { get; set; } = "Send";
        public string EmailHint { get; set; } = "We will only use this to send you a link to return to your assessment";
        public string SmsPrompt { get; set; } = "What is your phone number?";
        public string SmsInputLabel { get; set; } = "What is your phone number?";
        public string SmsInputButtonText { get; set; } = "Send";
        public string ReturnText { get; set; } = "Use the reference in the text message to return to your assessment.";
        public string ContinueButtonText { get; set; } = "Continue";
        public string EmailSentHeader { get; set; } = "Check your email";
        public string SmsSentHeader { get; set; } = "Check your phone";
        public string EmailSentText { get; set; } = "An email has been sent to";
        public string SmsSentText { get; set; } = "A text message has been sent to";
        public string SaveProgressHeader { get; set; } = "Save your progress";
        public string SaveProgressLabel { get; set; } = "How would you like to return to your assessment?";
        public string SaveProgressOptionEmail { get; set; } = "Send me an email with a link";
        public string SaveProgressOptionSms { get; set; } = "Send me a text";
        public string SaveProgressOptionReference { get; set; } = "Get a reference";
        public string SaveProgressReturnText { get; set; } = "Return to assessment";
        public string SaveProgressNoOptionSelectedMessage { get; set; } = "Please select an option to continue";
        public string ReferenceYourNumberText { get; set; } = "Your reference";
        public string ReferenceReturnText { get; set; } = "Return to assessment";
        public string ReferenceInstructionsText { get; set; } = "The code above is your unique reference. You can use this to:";
        public string ReferenceInstruction1Text { get; set; } = "return to your assessment and continue";
        public string ReferenceInstruction2Text { get; set; } = "see your results, if you have finished the assessment";
        public string SmsInputInvalidMessage { get; set; } = "Enter a phone number";
        public string SmsInputNotifyFailMessage { get; set; } = "Enter a valid phone number";
        public string SmsHint { get; set; } = "We will only use this to send you a link to return to your assessment";
        public string EmailInputNotifyFailMessage { get; set; } = "Enter a valid email address";
        public string EmailInputNotEntered { get; set; } = "You must enter an email address";

        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
    }
}
