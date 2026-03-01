using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using OfficeHealper;
using System;
using System.Collections.Generic;
using System.IO;

namespace OfficeHelper
{
    public class GoogleSpreedSheetAPI
    {
        private static string baseDir;

        private static string sheetId;

        private static string sheet1Name;

        private static string sheet2Name;
        static GoogleSpreedSheetAPI()
        {
            baseDir = AppContext.BaseDirectory + "\\DriveCred.json";
            sheetId = Environment.GetEnvironmentVariable("GOOGLESHEET_ID");
            sheet1Name = Util.GetSheetName("TimeTracker");
            sheet2Name = Util.GetSheetName("TimeAggregator");
        }

        public static bool WriteToExcel<T>(List<T> objs , bool isCacheDataPersistance = false)
        {
            try
            {
                var credential = GoogleCredential
               .FromFile(baseDir)
               .CreateScoped(SheetsService.Scope.Spreadsheets);

                var service = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "JsonToSheet"
                });

                string targetSheet = typeof(T) == typeof(TimeTracker) ? sheet1Name : sheet2Name;

                foreach (var obj in objs)
                {
                    // Get the row values for this object
                    List<IList<object>> values = typeof(T) == typeof(TimeTracker)
                        ? GetObjectDeserializedValues((TimeTracker)(object)obj)
                        : GetObjectDeserializedValues((TimeAggregator)(object)obj);

                    var appendRequest = new ValueRange
                    {
                        Values = values
                    };

                    var request = service.Spreadsheets.Values.Append(appendRequest, sheetId, targetSheet);
                    request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                    request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

                    request.Execute();

                    return true;
                }
                return false;
            }
            catch (Exception ex) 
            {
                if(isCacheDataPersistance)
                {
                    return false;
                }
                CacheManager.PutObject<T>(objs);
                return false;
            }
        }

        private static List<IList<object>> GetObjectDeserializedValues(TimeAggregator obj)
        {
            return new List<IList<object>> {
            new List<object> { obj.id, obj.date, obj.workHours, obj.breakHours, obj.officeHours, obj.compensationHours }
        };
        }

        private static List<IList<object>> GetObjectDeserializedValues(TimeTracker obj)
        {
            return new List<IList<object>> {
            new List<object> { obj.id, obj.date, obj.time, obj.event_, obj.eventId, obj.sessionId }
        };
        }
        public static bool CreateNewSheet()
        {
            try
            {

                GoogleCredential credential;
                using (var stream = new FileStream(baseDir, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(SheetsService.Scope.Spreadsheets);
                }

                var service = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Sheet Creator"
                });


                var batchRequest = GetBatch(sheet1Name);
                // Execute request
                service.Spreadsheets
                    .BatchUpdate(batchRequest, sheetId)
                    .Execute();

                var batchRequest2 = GetBatch(sheet2Name);

                service.Spreadsheets
                .BatchUpdate(batchRequest2, sheetId)
                .Execute();

                var batchRequest3 = GetBatch("UserDetails");

                service.Spreadsheets
                .BatchUpdate(batchRequest3, sheetId)
                .Execute();

                var timeTrackerHeader = new List<IList<object>> { new List<object> { "Id", "Date", "Time", "Event", "EventId", "SessionId" } };
                var timeAggregatorHeader = new List<IList<object>> { new List<object> { "Id", "Date", "WorkHours", "BreakHours", "OfficeHours", "CompensateHours" } };

                var timeTrackerHeaderRange = $"{sheet1Name}!A1:F1";
                var timeAggregatoeHeaderRange = $"{sheet2Name}!A1:F1";

                var headerRequest1 = new ValueRange
                {
                    Values = timeTrackerHeader
                };

                var headerRequest2 = new ValueRange
                {
                    Values = timeAggregatorHeader
                };

                var request1 = service.Spreadsheets.Values.Update(headerRequest1, sheetId, timeTrackerHeaderRange);
                request1.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                request1.Execute();


                var request2 = service.Spreadsheets.Values.Update(headerRequest2, sheetId, timeAggregatoeHeaderRange);
                request2.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                request2.Execute();

                var values = new List<IList<object>> { new List<object> { Environment.UserName } };

                var appendRequest = new ValueRange
                {
                    Values = values
                };

                var request3 = service.Spreadsheets.Values.Append(appendRequest, sheetId, "UserDetails");
                request3.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                request3.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

                request3.Execute();
                return true;
            }
            catch (Exception ex) { ErrorHandler.RecordError(ex.Message + "error at create new sheet");return false; }
        }

        private static BatchUpdateSpreadsheetRequest GetBatch(string name)
        {
            // Create AddSheet request
            return new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>
            {
                new Request
                {
                    AddSheet = new AddSheetRequest
                    {
                        Properties = new SheetProperties
                        {
                            Title = name
                        }
                    }
                }
            }
            };
        }
        public static bool CheckSheetExistance()
        {
            try
            {

                string newSheetName = "UserDetails";

                GoogleCredential credential;
                using (var stream = new FileStream(baseDir, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(SheetsService.Scope.Spreadsheets);
                }
                var service = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Sheet Checker"
                });

                // 1️⃣ Get spreadsheet metadata
                var spreadsheet = service.Spreadsheets.Values.Get(sheetId, newSheetName).Execute();

                // 2️⃣ Check if the sheet already exists
                //bool sheetExists = spreadsheet.Sheets
                //.Any(s => s.Properties.Title.Equals(newSheetName, StringComparison.OrdinalIgnoreCase));

                var values = spreadsheet.Values;

                bool check = false;

                foreach (var value in values)
                {
                    foreach (var item in value)
                    {
                        check = String.Equals(Environment.UserName, item) ? true : false;
                    }
                }
                return check;
            }
            catch (Exception ex) { ErrorHandler.RecordError(ex.Message + "error at sheet existance"); return false; }

        }
    }
}

