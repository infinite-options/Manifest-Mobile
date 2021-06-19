using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Manifest.Config;
using Manifest.LogIn.Apple;
using Manifest.LogIn.Classes;
using Newtonsoft.Json;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Manifest.Models
{
    public class SignIn
    {
        public SignIn()
        {
        }

        public async Task<string> UserVerification(AuthenticatorCompletedEventArgs user = null, AppleAccount appleCredentials = null, string platform = "")
        {
            string result = "";
            try
            {
                var client = new HttpClient();
                var socialLogInPost = new SocialLogInPost();
                var googleData = new GoogleResponse();
                var facebookData = new FacebookResponse();

                if (platform == "GOOGLE")
                {
                    var request = new OAuth2Request("GET", new Uri(AppConstants.GoogleUserInfoUrl), null, user.Account);
                    var GoogleResponse = await request.GetResponseAsync();
                    var googelUserData = GoogleResponse.GetResponseText();

                    googleData = JsonConvert.DeserializeObject<GoogleResponse>(googelUserData);

                    socialLogInPost.email = googleData.email;
                    socialLogInPost.social_id = googleData.id;
                    socialLogInPost.mobile_access_token = user.Account.Properties["access_token"];
                    socialLogInPost.mobile_refresh_token = user.Account.Properties["refresh_token"];

                    //socialLogInPost.email = "omarfacio2010@gmail.com";
                    //socialLogInPost.social_id = "105601887216228109150";
                    //socialLogInPost.mobile_access_token = "ya29.a0AfH6SMAEKLXHhpcDC2W7wBXIyUPWhxEUU9Cgm64ivSf3qC7-Y3L-sx0briU9mHIIRIk_Uc6n9g9RHwPTEAsBWDDOfOwOdNqVJO1CsmbL7_YUlxY-qyspF7RXLKFCTU2y7JqkkPDEleBNjPRASpSiZoE_VycL";
                    //socialLogInPost.mobile_refresh_token = "1//06AnR3q70ZXOQCgYIARAAGAYSNwF-L9IrN3s1ncmDXu1_wNlxs5B-z981yv2v4YJRFTQMWRxTuR2IiyGOEMNA27jRQmZ5sjp6Ruc";

                    //Application.Current.Properties["accessToken"] = "ya29.a0AfH6SMAEKLXHhpcDC2W7wBXIyUPWhxEUU9Cgm64ivSf3qC7-Y3L-sx0briU9mHIIRIk_Uc6n9g9RHwPTEAsBWDDOfOwOdNqVJO1CsmbL7_YUlxY-qyspF7RXLKFCTU2y7JqkkPDEleBNjPRASpSiZoE_VycL";
                    //Application.Current.Properties["refreshToken"] = "1//06AnR3q70ZXOQCgYIARAAGAYSNwF-L9IrN3s1ncmDXu1_wNlxs5B-z981yv2v4YJRFTQMWRxTuR2IiyGOEMNA27jRQmZ5sjp6Ruc";

                }
                else if (platform == "FACEBOOK")
                {

                    var facebookResponse = client.GetStringAsync(AppConstants.FacebookUserInfoUrl + user.Account.Properties["access_token"]);
                    var facebookUserData = facebookResponse.Result;

                    facebookData = JsonConvert.DeserializeObject<FacebookResponse>(facebookUserData);

                    socialLogInPost.email = facebookData.email;
                    socialLogInPost.social_id = facebookData.id;
                    socialLogInPost.mobile_access_token = user.Account.Properties["access_token"];
                    socialLogInPost.mobile_refresh_token = user.Account.Properties["access_token"];
                }
                else if (platform == "APPLE")
                {
                    socialLogInPost.email = appleCredentials.Email;
                    socialLogInPost.social_id = appleCredentials.UserId;
                    socialLogInPost.mobile_access_token = appleCredentials.Token;
                    socialLogInPost.mobile_refresh_token = appleCredentials.Token;
                }

                socialLogInPost.signup_platform = platform;

                var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
                var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

                var RDSResponse = await client.PostAsync(AppConstants.BaseUrl + AppConstants.login, postContent);
                //var RDSResponse = await client.PostAsync(AppConstants.BaseUrl + AppConstants.UserIdFromEmailUrl, postContent);

                var responseContent = await RDSResponse.Content.ReadAsStringAsync();
                Debug.WriteLine(responseContent);
                var authetication = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                var session = JsonConvert.DeserializeObject<Session>(responseContent);
                if (RDSResponse.IsSuccessStatusCode)
                {
                    if (responseContent != null)
                    {
                        if (authetication.code.ToString() == AppConstants.EmailNotFound)
                        {
                            // Missing a Oops message you don't have an account
                            //Application.Current.MainPage = new LogInPage();
                            result = "EMAIL WAS NOT FOUND";
                            return result;
                        }
                        if (authetication.code.ToString() == AppConstants.AutheticatedSuccesful)
                        {

                            try
                            {
                                Debug.WriteLine("USER AUTHENTICATED");
                                DateTime today = DateTime.Now;
                                DateTime expDate = today.AddDays(AppConstants.days);

                                Application.Current.Properties["userId"] = session.result[0].user_unique_id;
                                Application.Current.Properties["timeStamp"] = expDate;
                                Application.Current.Properties["platform"] = platform;
                                Application.Current.Properties["accessToken"] = "1";
                                Application.Current.Properties["refreshToken"] = "1";

                                if (platform == "GOOGLE")
                                {
                                    Application.Current.Properties["showCalendar"] = true;
                                    Application.Current.Properties["accessToken"] = user.Account.Properties["access_token"];
                                    Application.Current.Properties["refreshToken"] = user.Account.Properties["refresh_token"];
                                }

                                //_ = Application.Current.SavePropertiesAsync();
                                //GetCurrOccurance();
                                string id = (string)Application.Current.Properties["userId"];
                                string guid = (string)Application.Current.Properties["guid"];
                                Debug.WriteLine("GUID FROM MAIN PAGE: " + guid);
                                if (guid != "")
                                {
                                    NotificationPost notificationPost = new NotificationPost();

                                    notificationPost.user_unique_id = id;
                                    notificationPost.guid = guid;
                                    notificationPost.notification = "TRUE";

                                    var notificationSerializedObject = JsonConvert.SerializeObject(notificationPost);
                                    Debug.WriteLine("Notification JSON Object to send: " + notificationSerializedObject);

                                    var notificationContent = new StringContent(notificationSerializedObject, Encoding.UTF8, "application/json");

                                    var clientResponse = await client.PostAsync(AppConstants.BaseUrl + AppConstants.addGuid, notificationContent);

                                    Debug.WriteLine("Status code: " + clientResponse.IsSuccessStatusCode);

                                    if (clientResponse.IsSuccessStatusCode)
                                    {
                                        result = "USER SIGNED IN SUCCESSFULLY AND DEVICE ID WAS REGISTERED SUCCESSFULLY";
                                        return result;
                                        //Debug.WriteLine("We have post the guid to the database");
                                    }
                                    else
                                    {
                                        //await DisplayAlert("Ooops!", "Something went wrong. We are not able to send you notification at this moment", "OK");
                                    }
                                }
                                result = "USER SIGNED IN SUCCESSFULLY AND DEVICE ID WAS NOT REGISTERED SUCCESSFULLY";
                                return result;
                                //foreach (string key in Application.Current.Properties.Keys)
                                //{
                                //    Debug.WriteLine("key: {0}, value: {1}", key, Application.Current.Properties[key]);
                                //}
                            }
                            catch (Exception s)
                            {
                                result = "ERROR WHEN CALLING ENDPOINT";
                                return result;
                                //await DisplayAlert("Something went wrong with notifications", "", "OK");
                            }
                        }
                        if (authetication.code.ToString() == AppConstants.ErrorPlatform)
                        {
                            //var RDSCode = JsonConvert.DeserializeObject<RDSLogInMessage>(responseContent);
                            //test.Hide();
                            //Application.Current.MainPage = new LogInPage("Message", RDSCode.message);
                            result = "SIGN IN WITH THE CORRECT VIA SOCIAL MEDIA ACCOUNT";
                            return result;
                        }

                        //if (authetication.code.ToString() == AppConstants.ErrorUserDirectLogIn)
                        //{
                        //    //test.Hide();
                        //    //Application.Current.MainPage = new LogInPage("Oops!", "You have an existing Serving Fresh account. Please use direct login");
                        //}
                    }
                }
                
                return result;
            }
            catch (Exception first)
            {
                Debug.WriteLine(first.Message);
                result = "SOMETHING FAILED IN THE USER VERIFICATION METHOD";
                return result;
            }
            
        }
    }
}
