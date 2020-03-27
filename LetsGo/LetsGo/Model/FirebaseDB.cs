using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using LetsGo.Model.Authentication;
using System.Net.Mail;
using System.Text;
using Firebase.Storage;
using System.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Globalization;
using System.Collections;

namespace LetsGo.Model
{
    public class FirebaseDB
    {
        public readonly FirebaseClient firebase = new FirebaseClient("https://letsgo-f4d0d.firebaseio.com/");
        private FirebaseStorage firebaseStorage = new FirebaseStorage("xamarinfirebase-letsgo-f4d0d.appspot.com");
        readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public FirebaseClient getDB()
        {
            return firebase;
        }
        public async Task<bool> LoginUser(string email, string password)
        {
            List<UserProfile> users = await GetAllUsers();
            var CurrentUser = users.Where(a => a.Email == email).FirstOrDefault();
            if (CurrentUser == null)
            {
                return false;
            }
            else if (email == CurrentUser.Email && EncryptDecrypt(password, 5) == CurrentUser.Password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool SignOutUser()
        {
            return true;
        }

        public async Task<bool> InitializeUser(string uName, DateTime uDOB, string uEmail, string password, bool publicAcct)
        {
            List<UserProfile> users = await GetAllUsers();

            var found = users.Where(a => a.Email == uEmail).FirstOrDefault();
            if (found != null)
            {
                return false;
            }
            string encodedPass = EncryptDecrypt(password, 5);
            UserProfile newUser = new UserProfile(uName.ToLower(), uDOB, uEmail.ToLower(), encodedPass, publicAcct);
            await firebase
              .Child("userprofiles")
              .PostAsync(newUser);
            return true;
        }

        public async Task<bool> InitializeEvent(string eName, string edetails, DateTime eDate, string eStart, string eEnd, string location, string eMail, string interests, bool publicAcct)
        {
            EventProfile newEvent = new EventProfile(eName.ToLower(), edetails.ToLower(), eDate, eStart, eEnd, location.ToLower(), eMail, interests.ToLower(), publicAcct);
            await firebase
              .Child("Events")
              .PostAsync(newEvent);
            return true;
        }

        public async Task<bool> SendPasswordRecoverEmail(string email)
        {

            try
            {

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("letsgo.noreply441@gmail.com");
                mail.To.Add(email);
                mail.Subject = "LetsGo: Password Recovery";
                mail.Body = "You are receiving this email to reset your LetsGo password. Your new temporary" +
                            " password is: " + "'LetsGoResetPassword441'. Use this new password to login. You may reset your password once logged in" +
                            " with this temporary password.";

                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("letsgo.noreply441@gmail.com", "LetsGo123");

                SmtpServer.Send(mail);

                List<UserProfile> users = await GetAllUsers();
                UserProfile CurrentUser = users.Where(a => a.Email == email).FirstOrDefault();

                var userToUpdate = (await firebase
                    .Child("userprofiles")
                    .OnceAsync<UserProfile>()).Where(a => a.Object.Email == email).FirstOrDefault();

                await firebase.Child("userprofiles").Child(userToUpdate.Key)
                    .PutAsync(new UserProfile() { Name = CurrentUser.Name, Email = CurrentUser.Email, DateOfBirth = CurrentUser.DateOfBirth, Password = EncryptDecrypt("LetsGoResetPassword441", 5), Location = CurrentUser.Location, Interests = CurrentUser.Interests, PublicAcct = CurrentUser.PublicAcct });
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<List<UserProfile>> GetAllUsers()
        {
            var users = (await firebase
                    .Child("userprofiles")
                    .OnceAsync<UserProfile>()).Select(item => new UserProfile
                    {
                        Name = item.Object.Name,
                        Email = item.Object.Email,
                        DateOfBirth = item.Object.DateOfBirth,
                        Password = item.Object.Password,
                        Location = item.Object.Location,
                        Interests = item.Object.Interests,
                        PublicAcct = item.Object.PublicAcct
                    }).ToList();

            List<UserProfile> AllUsers = new List<UserProfile>();

            for (int i = 0; i < users.Count; i++)
            {
                AllUsers.Add(new UserProfile() { Name = textInfo.ToTitleCase(users.ElementAt(i).Name), DateOfBirth = users.ElementAt(i).DateOfBirth,
                    Email = users.ElementAt(i).Email, Interests = users.ElementAt(i).Interests, Location = textInfo.ToTitleCase(users.ElementAt(i).Location),
                                                 PublicAcct = users.ElementAt(i).PublicAcct, Password = users.ElementAt(i).Password});
            }

            return AllUsers;
        }

        public async Task<bool> UpdateUserProfile(string uName, string location, bool publicAccount, List<string> interestList)
        {

            string CurrentUserEmail = GetCurrentUser();
            var userToUpdate = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == CurrentUserEmail).FirstOrDefault();

            List<UserProfile> users = await GetAllUsers();



            var CurrentUser = users.Where(a => a.Email == CurrentUserEmail).FirstOrDefault();
            DateTime date = CurrentUser.DateOfBirth;
            string email = CurrentUser.Email;
            List<string> interests;

            if (CurrentUser.Email != email)
                return false;

            interests = new List<string>();
            for (int i = 0; i < interestList.Count; i++)
            {
                if (!interests.Contains(interestList.ElementAt(i).ToLower()))
                    interests.Add(interestList.ElementAt(i).ToLower());
            }



            await firebase
            .Child("userprofiles")
            .Child(userToUpdate.Key)
            .PutAsync(new UserProfile() { Name = uName.ToLower(), Email = CurrentUser.Email, DateOfBirth = CurrentUser.DateOfBirth, Password = CurrentUser.Password, Location = location.ToLower(), Interests = interests, PublicAcct = publicAccount });
            return true;
        }

        public void SetCurrentUser(string email)
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            auth.SetCurrentUser(email);
        }

        public string GetCurrentUser()
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            return auth.GetCurrentUser();
        }


        private async Task<UserProfile> User()
        {
            string CurrentUserEmail = GetCurrentUser();
            var userToUpdate = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == CurrentUserEmail).FirstOrDefault();

            List<UserProfile> users = await GetAllUsers();

            var CurrentUser = users.Where(a => a.Email == CurrentUserEmail).FirstOrDefault();
            return CurrentUser;
        }

        public async Task<bool> HasPublicAccount()
        {
            UserProfile current = await User();

            return current.PublicAcct;
        }

        public async Task<string> GetUsersLocation()
        {
            UserProfile current = await User();

            return current.Location;
        }

        public async Task<string> GetUsersName()
        {
            UserProfile current = await User();

            return current.Name;
        }

        public async Task<List<string>> GetUsersInterests()
        {
            UserProfile current = await User();

            List<string> interests = new List<string>();
            for (int i = 0; i < current.Interests.Count; i++)
            {
                interests.Add(textInfo.ToTitleCase(current.Interests.ElementAt(i)));
            }
            return interests;

        }

        public async Task<bool> DeleteUserAccount()
        {
            string current = GetCurrentUser();
            var userToDelete = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == current).FirstOrDefault();

            await firebase.Child("userprofiles").Child(userToDelete.Key).DeleteAsync();
            return true;
        }

        public async Task<bool> ChangePassword(string oldPassword, string NewPassword)
        {
            List<UserProfile> users = await GetAllUsers();
            string CurrentUserEmail = GetCurrentUser();
            var CurrentUser = users.Where(a => a.Email == CurrentUserEmail).FirstOrDefault();

            if (oldPassword != EncryptDecrypt(CurrentUser.Password, 5))
                return false;
            var userToUpdate = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == CurrentUserEmail).FirstOrDefault();

            await firebase
                        .Child("userprofiles")
                        .Child(userToUpdate.Key)
                        .PutAsync(new UserProfile()
                        {
                            Name = CurrentUser.Name,
                            Email = CurrentUser.Email,
                            DateOfBirth = CurrentUser.DateOfBirth,
                            Password = EncryptDecrypt(NewPassword, 5),
                            Location = CurrentUser.Location,
                            Interests = CurrentUser.Interests,
                            PublicAcct = CurrentUser.PublicAcct
                        });
            return true;
        }

        private string EncryptDecrypt(string szPlainText, int szEncryptionKey)
        {
            StringBuilder szInputStringBuild = new StringBuilder(szPlainText);
            StringBuilder szOutStringBuild = new StringBuilder(szPlainText.Length);
            char Textch;
            for (int iCount = 0; iCount < szPlainText.Length; iCount++)
            {
                Textch = szInputStringBuild[iCount];
                Textch = (char)(Textch ^ szEncryptionKey);
                szOutStringBuild.Append(Textch);
            }
            return szOutStringBuild.ToString();
        }


        public async Task<string> UploadFile(Stream fileStream, string userEventOrComm, string fileName)
        {
            var imageUrl = await firebaseStorage
                .Child(userEventOrComm)
                .Child(fileName)
                .PutAsync(fileStream);
            return imageUrl;
        }

        public async Task<List<EventProfile>> GetFeed()
        {
            string currentEmail = GetCurrentUser();
            string location = await GetUsersLocation();

            var events = (await firebase
                        .Child("Events")
                        .OnceAsync<EventProfile>()).Where(a => a.Object.Location == location && a.Object.PublicEvent == true).ToList();

            var feedEvents = events.ToList();

            List<EventProfile> feed = new List<EventProfile>();
            for (int i = 0; i < feedEvents.Count; i++)
            {
                feed.Add(new EventProfile() { Name = textInfo.ToTitleCase(feedEvents.ElementAt(i).Object.Name), DateOfEvent = feedEvents.ElementAt(i).Object.DateOfEvent,
                                             Location = textInfo.ToTitleCase(feedEvents.ElementAt(i).Object.Location), Description = textInfo.ToTitleCase(feedEvents.ElementAt(i).Object.Description),
                                             EventOwner = feedEvents.ElementAt(i).Object.EventOwner, StartOfEvent = feedEvents.ElementAt(i).Object.StartOfEvent,
                                             EndOfEvent = feedEvents.ElementAt(i).Object.EndOfEvent, Interests = feedEvents.ElementAt(i).Object.Interests,
                                             PublicEvent = feedEvents.ElementAt(i).Object.PublicEvent});
            }
            
            return feed;
        }

        public async Task<ArrayList> Search(string InterestTag)
        {
            string currentEmail = GetCurrentUser();
            ArrayList searchResults = new ArrayList();
            List<EventProfile> publicEvents = await GetPublicEvents(InterestTag);
            List<CommunityProfile> publicCommunities = await GetPublicCommunities(InterestTag);
            List<UserProfile> publicUsers = await GetPublicUsers(InterestTag);

            for (int i = 0; i < publicEvents.Count; i++)
            {
                searchResults.Add(new EventProfile()
                {
                    Name = textInfo.ToTitleCase(publicEvents.ElementAt(i).Name),
                    DateOfEvent = publicEvents.ElementAt(i).DateOfEvent,
                    Location = textInfo.ToTitleCase(publicEvents.ElementAt(i).Location),
                    Description = textInfo.ToTitleCase(publicEvents.ElementAt(i).Description),
                    EventOwner = publicEvents.ElementAt(i).EventOwner,
                    StartOfEvent = publicEvents.ElementAt(i).StartOfEvent,
                    EndOfEvent = publicEvents.ElementAt(i).EndOfEvent,
                    Interests = publicEvents.ElementAt(i).Interests,
                    PublicEvent = publicEvents.ElementAt(i).PublicEvent
                });
            }

            for (int i = 0; i < publicUsers.Count; i++)
            {
                searchResults.Add(new UserProfile()
                {
                    Name = textInfo.ToTitleCase(publicUsers.ElementAt(i).Name),
                    DateOfBirth = publicUsers.ElementAt(i).DateOfBirth,
                    Email = publicUsers.ElementAt(i).Email,
                    Interests = publicUsers.ElementAt(i).Interests,
                    Location = textInfo.ToTitleCase(publicUsers.ElementAt(i).Location),
                    PublicAcct = publicUsers.ElementAt(i).PublicAcct
                });
            }

            for (int i = 0; i < publicCommunities.Count; i++)
            {
                searchResults.Add(new CommunityProfile()
                {
                    Name = textInfo.ToTitleCase(publicCommunities.ElementAt(i).Name),
                    Identity = textInfo.ToTitleCase(publicCommunities.ElementAt(i).Identity),
                    CommunityLeader = publicCommunities.ElementAt(i).CommunityLeader,
                    Interests = publicCommunities.ElementAt(i).Interests,
                    PublicCommunity = publicCommunities.ElementAt(i).PublicCommunity
                });
            }


            return searchResults;

        }

        public async Task<List<UserProfile>> GetPublicUsers(string InterestTag)
        {
            var users = (await firebase
                        .Child("userprofiles")
                        .OnceAsync<UserProfile>()).Where(a => a.Object.PublicAcct == true && a.Object.Interests.Contains(InterestTag)).ToList();
            List<UserProfile> publicUsers = new List<UserProfile>();
            for (int i = 0; i < users.Count; i++)
            {
                publicUsers.Add(new UserProfile()
                {
                    Name = textInfo.ToTitleCase(users.ElementAt(i).Object.Name),
                    DateOfBirth = users.ElementAt(i).Object.DateOfBirth,
                    Email = users.ElementAt(i).Object.Email,
                    Interests = users.ElementAt(i).Object.Interests,
                    Location = textInfo.ToTitleCase(users.ElementAt(i).Object.Location),
                    PublicAcct = users.ElementAt(i).Object.PublicAcct
                });
            }
            return publicUsers;
        }
        public async Task<List<CommunityProfile>> GetPublicCommunities(string InterestTag)
        {
            var communities = (await firebase
                .Child("Communities")
                .OnceAsync<CommunityProfile>()).Where(a => a.Object.PublicCommunity == true && a.Object.Interests.Contains(InterestTag)).ToList();
            List<CommunityProfile> publicCommunities = new List<CommunityProfile>();

            var commList = communities.ToList();
            for (int i = 0; i < commList.Count; i++)
            {
                publicCommunities.Add(new CommunityProfile()
                {
                    Name = textInfo.ToTitleCase(commList.ElementAt(i).Object.Name),
                    Identity = textInfo.ToTitleCase(commList.ElementAt(i).Object.Identity),
                    CommunityLeader = commList.ElementAt(i).Object.CommunityLeader,
                    Interests = commList.ElementAt(i).Object.Interests,
                    PublicCommunity = commList.ElementAt(i).Object.PublicCommunity
                });
            }

            return publicCommunities;
        }

        public async Task<List<EventProfile>> GetPublicEvents(string InterestTag)
        {
            var events = (await firebase
            .Child("Events")
            .OnceAsync<EventProfile>()).Where(a => a.Object.PublicEvent == true && a.Object.Interests.Contains(InterestTag)).ToList();

            var eventsList = events.ToList();

            List<EventProfile> publicEvents = new List<EventProfile>();

            for (int i = 0; i < eventsList.Count; i++)
            {
                publicEvents.Add(new EventProfile()
                {
                    Name = textInfo.ToTitleCase(eventsList.ElementAt(i).Object.Name),
                    DateOfEvent = eventsList.ElementAt(i).Object.DateOfEvent,
                    Location = textInfo.ToTitleCase(eventsList.ElementAt(i).Object.Location),
                    Description = textInfo.ToTitleCase(eventsList.ElementAt(i).Object.Description),
                    EventOwner = eventsList.ElementAt(i).Object.EventOwner,
                    StartOfEvent = eventsList.ElementAt(i).Object.StartOfEvent,
                    EndOfEvent = eventsList.ElementAt(i).Object.EndOfEvent,
                    Interests = eventsList.ElementAt(i).Object.Interests,
                    PublicEvent = eventsList.ElementAt(i).Object.PublicEvent
                });
            }
            return publicEvents;
        }

    }
}
