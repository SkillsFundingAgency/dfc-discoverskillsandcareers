using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    public class GoogleSheetsReader
    {

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";

        public static IList<IList<object>> ReadRange(string spreadsheetId, string range)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            // Define request parameters.
            
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.FORMATTEDVALUE;
            
            ValueRange response = request.Execute();
            return response.Values;
            
        }
        
        public static IList<T> ReadRangeWithHeaders<T>(string spreadsheetId, string range, Func<string[], IList<object>, T> mapper)
        {
            var data = ReadRange(spreadsheetId, range);
            var headers = data[0].Select(r => r.ToString()).ToArray();
            
            return data.Skip(1).Select(row => mapper(headers, row)).Where(r => r != null).ToList();
        }
        
        public static IList<T> ReadRange<T>(string spreadsheetId, string range, Func<IList<object>, T> mapper)
        {
            return ReadRange(spreadsheetId, range).Select(mapper).Where(r => r != null).ToList();
        }
        
        
        
    }
}