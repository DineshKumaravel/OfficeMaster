using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
namespace OfficeHelper
{
    public class NotificationAPI
    {
        private static string token = Environment.GetEnvironmentVariable("NOTIFICATIONAPI_TOKEN");
        private static string url = Environment.GetEnvironmentVariable("NOTIFICATIONAPI_URL");
        public static string _otp { get; set; }

        public static void SendErrorMail(string ex)
        {
            var body = new
            {
                type = "errornotification",
                to = new
                {
                    id = "dineshkumaravel04@gmail.com",
                    email = "dineshkumaravel04@gmail.com"
                },
                parameters = new
                {
                    name = Environment.UserName,
                    error = ex
                },
                templateId = "errornotificationtemplate"
            };

            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
            request.Content = JsonContent.Create(body);

            var response = client.Send(request);
        }
    }
        #region
        //private static string GenerateOTP()
        //{
        //    Random generator = new Random();
        //    _otp = generator.Next(1000, 10000).ToString();
        //    return _otp;
        //}
        //public static bool VerifyOTP(string otp)
        //{
        //    return _otp == otp ? true : false;
        //}
        //public static string SendOtpEmail(string email, string templateId)
        //{
        //    try
        //    {
        //        var response = SendMail(email,templateId , null ,null , true);

        //        string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return "Response: " + responseString;
        //        }
        //        else
        //        {
        //            return $"Error: {response.StatusCode} - {responseString}";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}
        //public static string SendReportMail(string email , string templateId , string attachmentName , string attachmentContentBytes)
        //{
        //    try
        //    {
        //        var response = SendMail(email, templateId, attachmentName, attachmentContentBytes, false);

        //        string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return "Response: " + responseString;
        //        }
        //        else
        //        {
        //            return $"Error: {response.StatusCode} - {responseString}";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}
        //private static HttpResponseMessage SendMail(string email, string templateId, string attachmentName = null, string attachmentContentBytes = null, bool otpMail = false)
        //{
        //    var body = new
        //    {
        //        type = "OFFICE MASTER",
        //        to = new
        //        {
        //            id = email,
        //            email = email
        //        },
        //        parameters = new
        //        {
        //            client_name = Environment.UserName,
        //            otp = otpMail ? GenerateOTP() : null
        //        },
        //        templateId = templateId,
        //        Option = attachmentName != null ? GenerateAttachmentBody(attachmentName, attachmentContentBytes) : new Dictionary<string, object>()

        //    };

        //    HttpClient client = new HttpClient();
        //    var request = new HttpRequestMessage(HttpMethod.Post, url);
        //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
        //    request.Content = JsonContent.Create(body);

        //    var response = client.Send(request);
        //    return response;
        //}
        //private static Dictionary<string, object> GenerateAttachmentBody(string attachmentName, string attachmentContentBytes)
        //{

        //    return new Dictionary<string, object>
        //    {
        //        { "email", new Dictionary<string, object>
        //            {
        //                { "attachments", new List<Dictionary<string, object>>
        //                    {
        //                        new Dictionary<string, object>
        //                        {
        //                            { "filename", attachmentName},
        //                            { "content", attachmentContentBytes },
        //                            { "contentType", "image/png" }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    };
        //}
        #endregion
}

