using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class ContentSaveProgressPage : IContentPage
    {
        public string Title { get; set; } 
        public string PageTitle { get; set; } 
        public string EmailPrompt { get; set; } 
        public string EmailInputLabel { get; set; } 
        public string EmailInputButtonText { get; set; } 
        public string SmsPrompt { get; set; } 
        public string SmsInputLabel { get; set; } 
        public string SmsInputButtonText { get; set; } 
        public string ReturnText { get; set; } 
        public string ContinueButtonText { get; set; } 
        public string EmailSentHeader { get; set; } 
        public string SmsSentHeader { get; set; } 
        public string EmailSentText { get; set; } 
        public string SmsSentText { get; set; } 
        public string SaveProgressHeader { get; set; } 
        public string SaveProgressLabel { get; set; } 
        public string SaveProgressOptionEmail { get; set; } 
        public string SaveProgressOptionSms { get; set; } 
        public string SaveProgressOptionReference { get; set; } 
        public string SaveProgressReturnText { get; set; } 
        public string SaveProgressNoOptionSelectedMessage { get; set; } 
        public string ReferenceYourNumberText { get; set; } 
        public string ReferenceReturnText { get; set; } 
        public string ReferenceInstructionsText { get; set; } 
        public string ReferenceInstruction1Text { get; set; } 
        public string ReferenceInstruction2Text { get; set; } 

        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
    }
}
