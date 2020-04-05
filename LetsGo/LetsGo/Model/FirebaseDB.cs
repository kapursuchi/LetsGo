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
using System.Threading;

namespace LetsGo.Model
{
    public class FirebaseDB
    {
        public readonly FirebaseClient firebase = new FirebaseClient("https://letsgo-f4d0d.firebaseio.com/");
        private FirebaseStorage firebaseStorage = new FirebaseStorage("letsgo - f4d0d.appspot.com");
        readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public async Task<bool> LoginUser(string email, string password)
        {
            List<UserProfile> users = await GetAllUsers();
            var CurrentUser = users.Where(a => a.Email.ToLower() == email.ToLower()).FirstOrDefault();
            if (CurrentUser == null)
            {
                return false;
            }
            else if (email.ToLower() == CurrentUser.Email && EncryptDecrypt(password, 5) == CurrentUser.Password)
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

            var found = users.Where(a => a.Email.ToLower() == uEmail.ToLower()).FirstOrDefault();
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
                    .PutAsync(new UserProfile() { Name = CurrentUser.Name, Email = CurrentUser.Email, ProfileImage = CurrentUser.ProfileImage, FriendRequests = CurrentUser.FriendRequests, DateOfBirth = CurrentUser.DateOfBirth, Password = EncryptDecrypt("LetsGoResetPassword441", 5), Location = CurrentUser.Location, Interests = CurrentUser.Interests, Friends = CurrentUser.Friends, PublicAcct = CurrentUser.PublicAcct });
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
                        Friends = item.Object.Friends,
                        PublicAcct = item.Object.PublicAcct,
                        ProfileImage = item.Object.ProfileImage
                    }).ToList();

            List<UserProfile> AllUsers = users.ToList();


            return AllUsers;
        }

        public async Task<List<EventProfile>> GetAllEvents()
        {
            var events = (await firebase
                    .Child("Events")
                    .OnceAsync<EventProfile>()).Select(item => new EventProfile
                    {
                        Name = item.Object.Name,
                        DateOfEvent = item.Object.DateOfEvent,
                        StartOfEvent = item.Object.StartOfEvent,
                        EndOfEvent = item.Object.EndOfEvent,
                        Description = item.Object.Description,
                        Location = item.Object.Location,
                        Interests = item.Object.Interests,
                        PublicEvent = item.Object.PublicEvent,
                        EventOwner = item.Object.EventOwner
                    }).ToList();

            List<EventProfile> AllEvents = events.ToList();


            return AllEvents;
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
            .PutAsync(new UserProfile() { Name = uName.ToLower(), Email = CurrentUser.Email, ProfileImage = CurrentUser.ProfileImage, FriendRequests = CurrentUser.FriendRequests, DateOfBirth = CurrentUser.DateOfBirth, Password = CurrentUser.Password, Location = location.ToLower(), Interests = interests, Friends = CurrentUser.Friends, PublicAcct = publicAccount });
            return true;
        }

        /*public async Task<bool> UpdateEventProfile(string Name, string location, bool publicEvent, DateTime Date, List<string> interestList)
        {
            string CurrentUserEmail = GetCurrentUser();
            var CurrentEvent = (await firebase
                .Child("Events")
                .OnceAsync<EventProfile>()).Where(a => a.Object.EventOwner == CurrentUserEmail).FirstOrDefault();

            List<EventProfile> Events = await GetAllEvents();



            var CurrentUser = Events.Where(a => a.EventOwner == CurrentUserEmail).FirstOrDefault();
            DateTime date = CurrentUser.DateOfEvent;
            string email = CurrentUser.EventOwner;
            List<string> interests;

            if (CurrentUser.EventOwner != email)
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
            .PutAsync(new UserProfile() { Name = uName.ToLower(), Email = CurrentUser.Email, ProfileImage = CurrentUser.ProfileImage, FriendRequests = CurrentUser.FriendRequests, DateOfBirth = CurrentUser.DateOfBirth, Password = CurrentUser.Password, Location = location.ToLower(), Interests = interests, Friends = CurrentUser.Friends, PublicAcct = publicAccount });
            return true;
        }*/

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

        private async Task<EventProfile> Event()
        {
            string CurrentUserEmail = GetCurrentUser();
            var AllEvents = (await firebase
                .Child("Event")
                .OnceAsync<EventProfile>()).Where(a => a.Object.EventOwner == CurrentUserEmail).FirstOrDefault();

            List<EventProfile> events = await GetAllEvents();

            var CurrentEvent = events.Where(a => a.EventOwner == CurrentUserEmail).FirstOrDefault();
            return CurrentEvent;
        }

        public async Task<bool> HasPublicAccount()
        {
            UserProfile current = await User();

            return current.PublicAcct;
        }

        public async Task<string> GetUsersLocation()
        {
            UserProfile current = await User();
            if (current.Location == null)
                return null;
            return current.Location;
        }

        public async Task<string> GetUsersName()
        {
            UserProfile current = await User();

            return current.Name;
        }

        public async Task<string> GetEventName()
        {
            EventProfile current = await Event();

            return current.Name;
        }
        public async Task<string> GetEventLocation()
        {
            EventProfile current = await Event();

            return current.Location;
        }

        public async Task<string> GetEventDescription()
        {
            EventProfile current = await Event();

            return current.Description;
        }



        public async Task<List<string>> GetUsersInterests()
        {
            UserProfile current = await User();

            
            List<string> interests = new List<string>();
            if (current.Interests == null)
                return interests;
            for (int i = 0; i < current.Interests.Count; i++)
            {
                interests.Add(textInfo.ToTitleCase(current.Interests.ElementAt(i)));
            }
            return interests;

        }

        public async Task<List<string>> GetUsersInterests(string email)
        {
            UserProfile current = await GetProfileOfUser(email);

            List<string> interests = new List<string>();
            if (current.Interests == null)
                return interests;
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
                            FriendRequests = CurrentUser.FriendRequests,
                            Friends = CurrentUser.Friends,
                            PublicAcct = CurrentUser.PublicAcct,
                            ProfileImage = CurrentUser.ProfileImage
                        }) ;
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




        public async Task<string> UploadProfilePhoto(Stream filestream)
        {
            List<UserProfile> users = await GetAllUsers();
            string CurrentUserEmail = GetCurrentUser();
            var CurrentUser = users.Where(a => a.Email == CurrentUserEmail).FirstOrDefault();
            var userToUpdate = (await firebase
                    .Child("userprofiles")
                    .OnceAsync<UserProfile>()).Where(a => a.Object.Email == CurrentUserEmail).FirstOrDefault();

            string photo = await UploadFile(filestream, "userprofiles", CurrentUserEmail, "profilepicture.jpg");
            await firebase
                    .Child("userprofiles")
                    .Child(userToUpdate.Key)
                    .PutAsync(new UserProfile()
                    {
                        Name = CurrentUser.Name,
                        Email = CurrentUser.Email,
                        DateOfBirth = CurrentUser.DateOfBirth,
                        Password = CurrentUser.Password,
                        Location = CurrentUser.Location,
                        Interests = CurrentUser.Interests,
                        FriendRequests = CurrentUser.FriendRequests,
                        Friends = CurrentUser.Friends,
                        PublicAcct = CurrentUser.PublicAcct,
                        ProfileImage = photo
                    });
            return photo;
        }

        public async Task<string> UploadEventPhoto(Stream filestream, string eventName)
        {
            string photo = await UploadFile(filestream, "events", eventName, "eventimage.jpg");
            return photo;
        }

        public async Task<string> UploadCommunityPhoto(Stream filestream, string communityName)
        {
            string photo = await UploadFile(filestream, "communities", communityName, "communityimage.jpg");
            return photo;
        }

        private async Task<string> UploadFile(Stream fileStream, string root, string child, string fileName)
        {
            var imageUrl = await new FirebaseStorage("letsgo-f4d0d.appspot.com")
                .Child(root)
                .Child(child)
                .Child(fileName)
                .PutAsync(fileStream);
            return imageUrl;
        }

        public async Task<string> GetProfilePicture()
        {
            string currentUser = GetCurrentUser();
            string picture = await GetProfilePicture(currentUser);
            return picture;
        }

        public async Task<string> GetProfilePicture(string user)
        {
            string picture = await GetPicture("userprofiles", user, "profilepicture.jpg");
            return picture;
        }

        public async Task<string> GetEventPicture(string eventName)
        {
            string picture = await GetPicture("events", eventName, "eventimage.jpg");
            return picture;
        }

        public async Task<string> GetCommunityPicture(string communityName)
        {
            string picture = await GetPicture("communities", communityName, "communityimage.jpg");
            return picture;
        }
        private async Task<string> GetPicture(string root, string child, string pictureType)
        {
            string img;
            try
            { 
                var imageurl = await new FirebaseStorage("letsgo-f4d0d.appspot.com")
                .Child(root)
                .Child(child)
                .Child(pictureType)
                .GetDownloadUrlAsync();
                img = imageurl;
            }
            catch (Exception)
            {
                return null;
            }
           

            return img;
        }

        public async Task<List<EventProfile>> GetFeed()
        {
            List<EventProfile> feed = new List<EventProfile>();
            string currentEmail = GetCurrentUser();
            
            string location = await GetUsersLocation();
            if (location == null)
                return feed;

            var events = (await firebase
                        .Child("Events")
                        .OnceAsync<EventProfile>()).Where(a => a.Object.Location == location.ToLower() && a.Object.PublicEvent == true).ToList();

            var feedEvents = events.ToList();

            
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
            List<EventProfile> publicEvents = await GetPublicEvents(InterestTag.ToLower());
            List<CommunityProfile> publicCommunities = await GetPublicCommunities(InterestTag.ToLower());
            List<UserProfile> publicUsers = await GetPublicUsers(InterestTag.ToLower());

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
                    Friends = publicUsers.ElementAt(i).Friends,
                    Location = textInfo.ToTitleCase(publicUsers.ElementAt(i).Location),
                    FriendRequests = publicUsers.ElementAt(i).FriendRequests,
                    PublicAcct = publicUsers.ElementAt(i).PublicAcct,
                    ProfileImage = publicUsers.ElementAt(i).ProfileImage
                }) ;
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
                        .OnceAsync<UserProfile>()).Where(a => /*a.Object.PublicAcct == true &&*/ a.Object.Interests.Contains(InterestTag) && a.Object.Email != GetCurrentUser()).ToList();
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
                    Friends = users.ElementAt(i).Object.Friends,
                    FriendRequests = users.ElementAt(i).Object.FriendRequests,
                    PublicAcct = users.ElementAt(i).Object.PublicAcct,
                    ProfileImage = users.ElementAt(i).Object.ProfileImage
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

        public async void AddFriend(string friendEmail)
        {
            string CurrentUserEmail = GetCurrentUser();
            var friendToAdd = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == friendEmail).FirstOrDefault();
            if (friendToAdd.Object.PublicAcct)
            {
                bool added = await AddUserAsFriend(CurrentUserEmail, friendEmail);
            }
            else
            {
                SendFriendRequest(CurrentUserEmail, friendEmail);
            }
        }

        private async Task<bool> AddUserAsFriend(string currentUser, string friendToAdd)
        {
            var current = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == currentUser).FirstOrDefault();
            var friend = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == friendToAdd).FirstOrDefault();
            List<UserProfile> users = await GetAllUsers();
            var CurrentUser = users.Where(a => a.Email == currentUser).FirstOrDefault();
            var FriendUser = users.Where(a => a.Email == friendToAdd).FirstOrDefault();
            List<string> CurrentFriendsList = await GetAllFriends(currentUser);
            CurrentFriendsList.Add(friendToAdd);
            await firebase
                .Child("userprofiles")
                .Child(current.Key)
                .PutAsync(new UserProfile()
                {
                    Name = CurrentUser.Name.ToLower(),
                    Email = CurrentUser.Email,
                    DateOfBirth = CurrentUser.DateOfBirth,
                    Password = CurrentUser.Password,
                    Location = CurrentUser.Location.ToLower(),
                    Interests = CurrentUser.Interests,
                    Friends = CurrentFriendsList,
                    FriendRequests = CurrentUser.FriendRequests,
                    PublicAcct = CurrentUser.PublicAcct,
                    ProfileImage = CurrentUser.ProfileImage
                });

            List<string> FriendsList = await GetAllFriends(friendToAdd);
            FriendsList.Add(CurrentUser.Email);
            await firebase
                .Child("userprofiles")
                .Child(friend.Key)
                .PutAsync(new UserProfile()
                {
                    Name = FriendUser.Name.ToLower(),
                    Email = FriendUser.Email,
                    DateOfBirth = FriendUser.DateOfBirth,
                    Password = FriendUser.Password,
                    Location = FriendUser.Location.ToLower(),
                    Interests = FriendUser.Interests,
                    Friends = FriendsList,
                    FriendRequests = FriendUser.FriendRequests,
                    PublicAcct = FriendUser.PublicAcct,
                    ProfileImage = FriendUser.ProfileImage
                });

            return true;
        }

        public async Task<bool> isFriend(string userIsFriend)
        {
            string current = GetCurrentUser();
            List<string> FriendsList = await GetAllFriends(current);
            if (FriendsList.Contains(userIsFriend))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void SendFriendRequest(string currentUser, string friendToRequest)
        {
            var current = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == currentUser).FirstOrDefault();
            var friend = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == friendToRequest).FirstOrDefault();
            List<UserProfile> users = await GetAllUsers();
            var CurrentUser = users.Where(a => a.Email == currentUser).FirstOrDefault();
            var FriendUser = users.Where(a => a.Email == friendToRequest).FirstOrDefault();
            List<string> notification = FriendUser.FriendRequests;
            if (notification == null)
                notification = new List<string>();
            notification.Add(currentUser);
            List<string> FriendsList = FriendUser.Friends;
            if (FriendsList == null)
            {
                FriendsList = new List<string>();
            }
            await firebase
                .Child("userprofiles")
                .Child(friend.Key)
                .PutAsync(new UserProfile()
                {
                    Name = FriendUser.Name.ToLower(),
                    Email = FriendUser.Email,
                    DateOfBirth = FriendUser.DateOfBirth,
                    Password = FriendUser.Password,
                    Location = FriendUser.Location.ToLower(),
                    Interests = FriendUser.Interests,
                    Friends = FriendsList,
                    FriendRequests = notification,
                    PublicAcct = FriendUser.PublicAcct,
                    ProfileImage = FriendUser.ProfileImage
                });

        }

        public async Task<List<UserProfile>> GetFriendRequests()
        {
            string current = GetCurrentUser();
            var currentUser = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == current).FirstOrDefault();
            List<UserProfile> users = await GetAllUsers();
            var CurrentUser = users.Where(a => a.Email == current).FirstOrDefault();


            List<string> notifications = new List<string>();
            List<UserProfile> friendRequests = new List<UserProfile>();
            if (currentUser.Object.FriendRequests == null)
                return friendRequests;
            for (int i = 0; i < currentUser.Object.FriendRequests.Count; i++)
            {
                UserProfile userToAdd = await GetProfileOfUser(currentUser.Object.FriendRequests.ElementAt(i));
                friendRequests.Add(new UserProfile()
                {
                    Name = textInfo.ToTitleCase(userToAdd.Name),
                    Email = userToAdd.Email,
                    DateOfBirth = userToAdd.DateOfBirth,
                    Password = userToAdd.Password,
                    Location = textInfo.ToTitleCase(userToAdd.Location),
                    Interests = userToAdd.Interests,
                    Friends = userToAdd.Friends,
                    FriendRequests = userToAdd.FriendRequests,
                    PublicAcct = userToAdd.PublicAcct,
                    ProfileImage = userToAdd.ProfileImage
                });
            }
            return friendRequests;
        }

        public async void RemoveRequest(UserProfile userToRemove)
        {
            string current = GetCurrentUser();
            var currentUser = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == current).FirstOrDefault();
            List<UserProfile> friendrequests = await GetFriendRequests();
            friendrequests.Remove(userToRemove);

            List<string> updated = new List<string>();
            for (int i = 0; i < friendrequests.Count; i++)
            {
                updated.Add(friendrequests.ElementAt(i).Email);
            }

            await firebase
                .Child("userprofiles")
                .Child(currentUser.Key)
                .PutAsync(new UserProfile()
                {
                    Name = currentUser.Object.Name.ToLower(),
                    Email = currentUser.Object.Email,
                    DateOfBirth = currentUser.Object.DateOfBirth,
                    Password = currentUser.Object.Password,
                    Location = currentUser.Object.Location.ToLower(),
                    Interests = currentUser.Object.Interests,
                    Friends = currentUser.Object.Friends,
                    FriendRequests = updated,
                    PublicAcct = currentUser.Object.PublicAcct,
                    ProfileImage = currentUser.Object.ProfileImage
                });

        }

        public async void AcceptRequest(UserProfile newFriend)
        {
            string current = GetCurrentUser();
            bool added = await AddUserAsFriend(current, newFriend.Email);
            RemoveRequest(newFriend);
        }


        public async void DeleteFriend(string friendToRemove)
        {
            var currentUser = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == GetCurrentUser()).FirstOrDefault();

            List<string> updated = new List<string>();
            for (int i = 0; i < currentUser.Object.Friends.Count; i++)
            {
                if (currentUser.Object.Friends.ElementAt(i) != friendToRemove)
                {
                    updated.Add(currentUser.Object.Friends.ElementAt(i));
                }
            }
            await firebase
                .Child("userprofiles")
                .Child(currentUser.Key)
                .PutAsync(new UserProfile()
                {
                    Name = currentUser.Object.Name.ToLower(),
                    Email = currentUser.Object.Email,
                    DateOfBirth = currentUser.Object.DateOfBirth,
                    Password = currentUser.Object.Password,
                    Location = currentUser.Object.Location.ToLower(),
                    Interests = currentUser.Object.Interests,
                    Friends = updated,
                    FriendRequests = currentUser.Object.FriendRequests,
                    PublicAcct = currentUser.Object.PublicAcct,
                    ProfileImage = currentUser.Object.ProfileImage
                });

            var friendUser = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == friendToRemove).FirstOrDefault();

            List<string> friendUpdated = new List<string>();
            for (int i = 0; i < friendUser.Object.Friends.Count; i++)
            {
                if (friendUser.Object.Friends.ElementAt(i) != currentUser.Object.Email)
                {
                    friendUpdated.Add(friendUser.Object.Friends.ElementAt(i));
                }
            }
            await firebase
                .Child("userprofiles")
                .Child(friendUser.Key)
                .PutAsync(new UserProfile()
                {
                    Name = friendUser.Object.Name.ToLower(),
                    Email = friendUser.Object.Email,
                    DateOfBirth = friendUser.Object.DateOfBirth,
                    Password = friendUser.Object.Password,
                    Location = friendUser.Object.Location.ToLower(),
                    Interests = friendUser.Object.Interests,
                    Friends = friendUpdated,
                    FriendRequests = friendUser.Object.FriendRequests,
                    PublicAcct = friendUser.Object.PublicAcct,
                    ProfileImage = friendUser.Object.ProfileImage
                });


        }

        public async Task<UserProfile> GetProfileOfUser(string email)
        {
            var userToUpdate = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == email).FirstOrDefault();

            List<UserProfile> users = await GetAllUsers();

            var user = users.Where(a => a.Email == email).FirstOrDefault();
            return user;


        }

        public async Task<List<string>> GetAllFriends(string userEmail)
        {

            List<UserProfile> users = await GetAllUsers();
            var user = users.Where(a => a.Email == userEmail).FirstOrDefault();

            List<string> friends = new List<string>();
            if (user.Friends != null)
            {
                for (int i = 0; i < user.Friends.Count; i++)
                {
                    if (user.Friends.ElementAt(i) != null)
                        friends.Add(user.Friends.ElementAt(i));
                }
                
            }
            return friends;


        }

        public async Task<List<UserProfile>> GetFriends(string userEmail)
        {

            List<UserProfile> users = await GetAllUsers();
            var user = users.Where(a => a.Email == userEmail).FirstOrDefault();

            List<UserProfile> friends = new List<UserProfile>();
            if (user.Friends != null)
            {
                for (int i = 0; i < user.Friends.Count; i++)
                {
                    var friend = users.Where(a => a.Email == user.Friends.ElementAt(i)).FirstOrDefault();
                    friends.Add(new UserProfile()
                    {
                        Name = textInfo.ToTitleCase(friend.Name),
                        Email = friend.Email,
                        DateOfBirth = friend.DateOfBirth,
                        Password = friend.Password,
                        Location = textInfo.ToTitleCase(friend.Location),
                        Interests = friend.Interests,
                        Friends = friend.Friends,
                        FriendRequests = friend.FriendRequests,
                        PublicAcct = friend.PublicAcct,
                        ProfileImage = friend.ProfileImage
                });
            }

            }
            return friends;


        }


    }
}
