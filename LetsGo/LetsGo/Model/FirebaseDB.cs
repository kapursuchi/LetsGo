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

        public async Task<bool> InitializeCommunity(string userEmail, string description, string location, string interests, string name, bool publicCommunity, bool invOnly, List<string> mems, string id)
        {
            CommunityProfile newCommunity = new CommunityProfile(userEmail, description.ToLower(), location.ToLower(), interests.ToLower(), name.ToLower(), publicCommunity, invOnly, mems, id);
            await firebase
                .Child("Communities")
                .PostAsync(newCommunity);
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
                    .PutAsync(new UserProfile() { Name = CurrentUser.Name, Email = CurrentUser.Email, CommunityInvites = CurrentUser.CommunityInvites, ProfileImage = CurrentUser.ProfileImage, EventRequests = CurrentUser.EventRequests, CommunityRequests = CurrentUser.CommunityRequests, FriendRequests = CurrentUser.FriendRequests, DateOfBirth = CurrentUser.DateOfBirth, Password = EncryptDecrypt("LetsGoResetPassword441", 5), Location = CurrentUser.Location, Interests = CurrentUser.Interests, Friends = CurrentUser.Friends, PublicAcct = CurrentUser.PublicAcct });
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
                        ProfileImage = item.Object.ProfileImage,
                        EventRequests = item.Object.EventRequests,
                        CommunityRequests = item.Object.CommunityRequests,
                        CommunityInvites = item.Object.CommunityInvites
                    }).ToList();

            List<UserProfile> AllUsers = users.ToList();


            return AllUsers;
        }

        public async Task<List<CommunityProfile>> GetAllCommunities()
        {
            var communities = (await firebase
                    .Child("Communities")
                    .OnceAsync<CommunityProfile>()).Select(item => new CommunityProfile
                    {
                        Leader = item.Object.Leader,
                        Description = item.Object.Description,
                        Location = item.Object.Location,
                        Interests = item.Object.Interests,
                        Name = item.Object.Name,
                        PublicCommunity = item.Object.PublicCommunity,
                        InviteOnly = item.Object.InviteOnly,
                        Members = item.Object.Members,
                        CommunityID = item.Object.CommunityID,
                        CommunityImage = item.Object.CommunityImage,
                        CommunityRequests = item.Object.CommunityRequests
                    }).ToList();

            List<CommunityProfile> AllCommunities = communities.ToList();

            return AllCommunities;
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
                        EventOwner = item.Object.EventOwner,
                        EventID = item.Object.EventID,
                        EventImage = item.Object.EventImage,
                        Members = item.Object.Members
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
            .PutAsync(new UserProfile() { 
                Name = uName.ToLower(),
                Email = CurrentUser.Email, 
                ProfileImage = CurrentUser.ProfileImage,
                EventRequests = CurrentUser.EventRequests,
                CommunityRequests = CurrentUser.CommunityRequests,
                FriendRequests = CurrentUser.FriendRequests,
                DateOfBirth = CurrentUser.DateOfBirth, 
                Password = CurrentUser.Password, 
                Location = location.ToLower(), 
                Interests = interests, 
                Friends = CurrentUser.Friends, 
                PublicAcct = publicAccount ,
                CommunityInvites = CurrentUser.CommunityInvites
            });
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
            EventProfile empty = new EventProfile() { Name = "Not Part of Any Events!"};
            var CurrentEvent = events.Where(a => a.EventOwner == CurrentUserEmail).FirstOrDefault();
            if (CurrentEvent != null)
                return CurrentEvent;
            else
                return empty;
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

        public async Task<UserProfile> GetUserObject(string email)
        {
            List<UserProfile> users = await GetAllUsers();
            var selectedUser = users.Where(a => a.Email == email).FirstOrDefault();
            return selectedUser;
        }

        public async Task<string> GetUsersName(string email)
        {
            List<UserProfile> users = await GetAllUsers();
            var selectedUser = users.Where(a => a.Email == email).FirstOrDefault();
            return selectedUser.Name;
        }

        public async Task<bool> IsPublicUser(string email)
        {
            List<UserProfile> users = await GetAllUsers();
            var selectedUser = users.Where(a => a.Email == email).FirstOrDefault();
            if (selectedUser.PublicAcct)
            {
                return true;
            }
            else
            {
                return false;
            }
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

        public async Task<List<string>> GetCommunityInterests(CommunityProfile community)
        {
            List<CommunityProfile> communities = await GetAllCommunities();
            var CurrentCommunity = communities.Where(a => a.Leader == community.Leader && a.Name == community.Name).FirstOrDefault();
            List<string> interests = new List<string>();
            //if (CurrentCommunity.Interests == null)
            //    return interests;
            for (int i = 0; i < CurrentCommunity.Interests.Count; i++)
            {
                interests.Add(textInfo.ToTitleCase(CurrentCommunity.Interests.ElementAt(i)));
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

        public async Task<bool> DeleteCommunity(string leader, string communityname)
        {
            var communityToDelete = (await firebase.Child("Communities").OnceAsync<CommunityProfile>()).Where(a => a.Object.Leader == leader && a.Object.Name == communityname).FirstOrDefault();
            await firebase.Child("Communities").Child(communityToDelete.Key).DeleteAsync();

            return true;
        }

        public async Task<bool> DeleteUserAccount()
        {
            string current = GetCurrentUser();
            var userToDelete = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == current).FirstOrDefault();

            await firebase.Child("userprofiles").Child(userToDelete.Key).DeleteAsync();


            List<UserProfile> users = await GetAllUsers();
            for (int i = 0; i < users.Count; i++)
            {
                if (users.ElementAt(i).Friends != null)
                {
                    if (users.ElementAt(i).Friends.Contains(current))
                    {
                        List<string> friends = users.ElementAt(i).Friends;
                        friends.Remove(current);
                        var user = (await firebase.Child("userprofiles").OnceAsync<UserProfile>()).Where(a => a.Object.Email == users.ElementAt(i).Email).FirstOrDefault();
                        await firebase.Child("userprofiles").Child(user.Key).PutAsync(new UserProfile()
                        {
                            Name = user.Object.Name,
                            Email = user.Object.Email,
                            DateOfBirth = user.Object.DateOfBirth,
                            Password = user.Object.Password,
                            Location = user.Object.Location,
                            Interests = user.Object.Interests,
                            FriendRequests = user.Object.FriendRequests,
                            Friends = friends,
                            PublicAcct = user.Object.PublicAcct,
                            ProfileImage = user.Object.ProfileImage,
                            EventRequests = user.Object.EventRequests,
                            CommunityRequests = user.Object.CommunityRequests,
                            CommunityInvites = user.Object.CommunityInvites
                        });
                    }
                }

            }

            var communities = (await firebase
                    .Child("Communities")
                    .OnceAsync<CommunityProfile>()).Select(item => new CommunityProfile()
                    {
                        Leader = item.Object.Leader,
                        Description = item.Object.Description,
                        Location = item.Object.Location,
                        Interests = item.Object.Interests,
                        Name = item.Object.Name,
                        PublicCommunity = item.Object.PublicCommunity,
                        InviteOnly = item.Object.InviteOnly,
                        Members = item.Object.Members,
                        CommunityID = item.Object.CommunityID,
                        CommunityImage = item.Object.CommunityImage,
                        CommunityRequests = item.Object.CommunityRequests
                    }).ToList();

            List<CommunityProfile> comms = communities.ToList();
            for (int i = 0; i < comms.Count; i++)
            {
                var community = (await firebase.Child("Communities").OnceAsync<CommunityProfile>()).Where(a => a.Object.CommunityID == comms.ElementAt(i).CommunityID).FirstOrDefault();
                if (comms.ElementAt(i).Leader == current)
                {
                    await firebase.Child("Communities").Child(community.Key).DeleteAsync();
                }
                else if (comms.ElementAt(i).Members != null && comms.ElementAt(i).Members.Contains(current))
                {
                    List<string> members = comms.ElementAt(i).Members;
                    members.Remove(current);

                    await firebase.Child("Communities").Child(community.Key).PutAsync(new CommunityProfile()
                    {
                        Leader = community.Object.Leader,
                        Description = community.Object.Description,
                        Location = community.Object.Location,
                        Name = community.Object.Name,
                        PublicCommunity = community.Object.PublicCommunity,
                        Interests = community.Object.Interests,
                        Members = members,
                        InviteOnly = community.Object.InviteOnly,
                        CommunityID = community.Object.CommunityID,
                        CommunityImage = community.Object.CommunityImage,
                        CommunityRequests = community.Object.CommunityRequests
                    });
                }
            }

            List<EventProfile> events = await GetAllEvents();

            for (int i = 0; i < events.Count; i++)
            {
                var evt = (await firebase.Child("Events").OnceAsync<EventProfile>()).Where(a => a.Object.EventID == events.ElementAt(i).EventID).FirstOrDefault();
                if (events.ElementAt(i).EventOwner == current)
                {
                    await firebase.Child("Events").Child(evt.Key).DeleteAsync();
                }
                else if (events.ElementAt(i).Members != null && events.ElementAt(i).Members.Contains(current))
                {
                    List<string> members = events.ElementAt(i).Members;
                    members.Remove(current);

                    await firebase.Child("Events").Child(evt.Key).PutAsync(new EventProfile()
                    {
                        EventOwner = evt.Object.EventOwner,
                        Description = evt.Object.Description,
                        Location = evt.Object.Location,
                        Interests = evt.Object.Interests,
                        Name = evt.Object.Name,
                        DateOfEvent = evt.Object.DateOfEvent,
                        StartOfEvent = evt.Object.StartOfEvent,
                        EndOfEvent = evt.Object.EndOfEvent,
                        PublicEvent = evt.Object.PublicEvent,
                        Members = members,
                        EventID = evt.Object.EventID,
                        EventImage = evt.Object.EventImage,
                        EventRequests = evt.Object.EventRequests
                    });
                }
            }
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
                            ProfileImage = CurrentUser.ProfileImage,
                            EventRequests = CurrentUser.EventRequests,
                            CommunityRequests = CurrentUser.CommunityRequests,
                            CommunityInvites = CurrentUser.CommunityInvites
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
                        ProfileImage = photo,
                        EventRequests = CurrentUser.EventRequests,
                        CommunityRequests = CurrentUser.CommunityRequests,
                        CommunityInvites = CurrentUser.CommunityInvites
                    });
            return photo;
        }

        public async Task<string> UploadEventPhoto(Stream filestream, string eventID)
        {
           
            List<EventProfile> events = await GetAllEvents();
            var currentEvent = events.Where(a => a.EventID.ToString() == eventID).FirstOrDefault();
            var evtToUpdate = (await firebase
                                .Child("Events")
                                .OnceAsync<EventProfile>()).Where(a => a.Object.EventID == eventID).FirstOrDefault();
            string photo = await UploadFile(filestream, "events", eventID, "eventimage.jpg");

            await firebase
                .Child("Events")
                .Child(evtToUpdate.Key)
                .Child("EventID")
                .PutAsync(photo);

            return photo;
        }
        
        public async Task<string> UploadCommunityPhoto(Stream filestream, string communityID)
        {
            List<CommunityProfile> communities = await GetAllCommunities();
            var CurrentCommunity = communities.Where(a => a.CommunityID.ToString() == communityID).FirstOrDefault();
            var commToUpdate = (await firebase
                        .Child("Communities")
                        .OnceAsync<CommunityProfile>()).Where(a => a.Object.CommunityID.ToString() == communityID).FirstOrDefault();

            string photo = await UploadFile(filestream, "Communities", communityID, "communityimage.jpg");
            await firebase
                    .Child("Communities")
                    .Child(commToUpdate.Key)
                    .PutAsync(new CommunityProfile()
                    {
                        Leader = CurrentCommunity.Leader,
                        Description = CurrentCommunity.Description,
                        Location = CurrentCommunity.Location,
                        Name = CurrentCommunity.Name,
                        PublicCommunity = CurrentCommunity.PublicCommunity,
                        Interests = CurrentCommunity.Interests,
                        Members = CurrentCommunity.Members,
                        InviteOnly = CurrentCommunity.InviteOnly,
                        CommunityID = CurrentCommunity.CommunityID,
                        CommunityImage = photo,
                        CommunityRequests = CurrentCommunity.CommunityRequests
                    });
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

        public async Task<string> GetEventPicture(string eventID)
        {
            string picture = await GetPicture("events", eventID, "eventimage.jpg");
            return picture;
        }

        public async Task<string> GetCommunityPicture(string communityID)
        {
            string picture = await GetPicture("Communities", communityID, "communityimage.jpg");
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
                                             PublicEvent = feedEvents.ElementAt(i).Object.PublicEvent, EventID = feedEvents.ElementAt(i).Object.EventID, 
                                               EventImage = feedEvents.ElementAt(i).Object.EventImage, Members = feedEvents.ElementAt(i).Object.Members});
            }
            return feed;
        }

        public async Task<List<EventProfile>> GetUserEvents()
        {
            List<EventProfile> events = new List<EventProfile>();
            string currentEmail = GetCurrentUser();

            

            var allUserEvents = (await firebase
                        .Child("Events")
                        .OnceAsync<EventProfile>()).Where(a => a.Object.Members.Contains(currentEmail)).ToList();

            var feedEvents = allUserEvents.ToList();


            for (int i = 0; i < feedEvents.Count; i++)
            {
                events.Add(new EventProfile()
                {
                    Name = textInfo.ToTitleCase(feedEvents.ElementAt(i).Object.Name),
                    DateOfEvent = feedEvents.ElementAt(i).Object.DateOfEvent,
                    Location = textInfo.ToTitleCase(feedEvents.ElementAt(i).Object.Location),
                    Description = textInfo.ToTitleCase(feedEvents.ElementAt(i).Object.Description),
                    EventOwner = feedEvents.ElementAt(i).Object.EventOwner,
                    StartOfEvent = feedEvents.ElementAt(i).Object.StartOfEvent,
                    EndOfEvent = feedEvents.ElementAt(i).Object.EndOfEvent,
                    Interests = feedEvents.ElementAt(i).Object.Interests,
                    PublicEvent = feedEvents.ElementAt(i).Object.PublicEvent,
                    EventID = feedEvents.ElementAt(i).Object.EventID,
                    EventImage = feedEvents.ElementAt(i).Object.EventImage,
                    Members = feedEvents.ElementAt(i).Object.Members
                });
            }
            return events;
        }

        public async Task<List<CommunityProfile>> GetAllCommunities(string InterestTag)
        {
            var communities = (await firebase
                .Child("Communities")
                .OnceAsync<CommunityProfile>()).Where(a => a.Object.Interests.Contains(InterestTag)).ToList();

            List<CommunityProfile> publicCommunities = new List<CommunityProfile>();

            var commList = communities.ToList();
            for (int i = 0; i < commList.Count; i++)
            {
                publicCommunities.Add(new CommunityProfile()
                {
                    Name = textInfo.ToTitleCase(commList.ElementAt(i).Object.Name),
                    Description = textInfo.ToTitleCase(commList.ElementAt(i).Object.Description),
                    Location = textInfo.ToTitleCase(commList.ElementAt(i).Object.Location),
                    Leader = commList.ElementAt(i).Object.Leader,
                    Interests = commList.ElementAt(i).Object.Interests,
                    PublicCommunity = commList.ElementAt(i).Object.PublicCommunity,
                    InviteOnly = commList.ElementAt(i).Object.InviteOnly,
                    Members = commList.ElementAt(i).Object.Members,
                    CommunityID = commList.ElementAt(i).Object.CommunityID,
                    CommunityImage = commList.ElementAt(i).Object.CommunityImage,
                    CommunityRequests = commList.ElementAt(i).Object.CommunityRequests
                });
            }

            return publicCommunities;
        }

        public async Task<List<UserProfile>> GetAllUsers(string InterestTag)
        {
            var users = (await firebase
                        .Child("userprofiles")
                        .OnceAsync<UserProfile>()).Where(a => a.Object.Interests.Contains(InterestTag) && a.Object.Email != GetCurrentUser()).ToList();
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
                    ProfileImage = users.ElementAt(i).Object.ProfileImage,
                    EventRequests = users.ElementAt(i).Object.EventRequests,
                    CommunityRequests = users.ElementAt(i).Object.CommunityRequests,
                    CommunityInvites = users.ElementAt(i).Object.CommunityInvites
                });
            }
            return publicUsers;
        }

        public async Task<ArrayList> Search(string InterestTag)
        {
            string currentEmail = GetCurrentUser();
            ArrayList searchResults = new ArrayList();
            List<EventProfile> publicEvents = await GetAllEvents(InterestTag.ToLower());
            List<CommunityProfile> publicCommunities = await GetAllCommunities(InterestTag.ToLower());
            List<UserProfile> publicUsers = await GetAllUsers(InterestTag.ToLower());

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
                    PublicEvent = publicEvents.ElementAt(i).PublicEvent,
                    EventID = publicEvents.ElementAt(i).EventID,
                    EventImage = publicEvents.ElementAt(i).EventImage,
                    Members = publicEvents.ElementAt(i).Members
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
                    ProfileImage = publicUsers.ElementAt(i).ProfileImage,
                    EventRequests = publicUsers.ElementAt(i).EventRequests,
                    CommunityRequests = publicUsers.ElementAt(i).CommunityRequests,
                    CommunityInvites = publicUsers.ElementAt(i).CommunityInvites
                });
            }

            for (int i = 0; i < publicCommunities.Count; i++)
            {
                searchResults.Add(new CommunityProfile()
                {
                    Name = textInfo.ToTitleCase(publicCommunities.ElementAt(i).Name),
                    Description = textInfo.ToTitleCase(publicCommunities.ElementAt(i).Description),
                    Location = textInfo.ToTitleCase(publicCommunities.ElementAt(i).Location),
                    Leader = publicCommunities.ElementAt(i).Leader,
                    Interests = publicCommunities.ElementAt(i).Interests,
                    PublicCommunity = publicCommunities.ElementAt(i).PublicCommunity,
                    CommunityImage = publicCommunities.ElementAt(i).CommunityImage,
                    InviteOnly = publicCommunities.ElementAt(i).InviteOnly,
                    Members = publicCommunities.ElementAt(i).Members,
                    CommunityRequests = publicCommunities.ElementAt(i).CommunityRequests,
                    CommunityID = publicCommunities.ElementAt(i).CommunityID
                });
            }


            return searchResults;

        }

        public async Task<List<CommunityProfile>> GetMyCommunities()
        {
            string userEmail = GetCurrentUser();
            List<CommunityProfile> results = new List<CommunityProfile>();
            var communities = (await firebase
                              .Child("Communities")
                              .OnceAsync<CommunityProfile>()).Where(a => a.Object.Members.Contains(userEmail)).ToList();
            for (int i = 0; i < communities.Count; i++)
            {
                results.Add(new CommunityProfile()
                {
                    Name = textInfo.ToTitleCase(communities.ElementAt(i).Object.Name),
                    Description = textInfo.ToTitleCase(communities.ElementAt(i).Object.Description),
                    Location = textInfo.ToTitleCase(communities.ElementAt(i).Object.Location),
                    Leader = communities.ElementAt(i).Object.Leader,
                    Interests = communities.ElementAt(i).Object.Interests,
                    PublicCommunity = communities.ElementAt(i).Object.PublicCommunity,
                    CommunityImage = communities.ElementAt(i).Object.CommunityImage,
                    InviteOnly = communities.ElementAt(i).Object.InviteOnly,
                    Members = communities.ElementAt(i).Object.Members,
                    CommunityRequests = communities.ElementAt(i).Object.CommunityRequests,
                    CommunityID = communities.ElementAt(i).Object.CommunityID
                });
            }
            return results;
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
                    ProfileImage = users.ElementAt(i).Object.ProfileImage,
                    EventRequests = users.ElementAt(i).Object.EventRequests,
                    CommunityRequests = users.ElementAt(i).Object.CommunityRequests,
                    CommunityInvites = users.ElementAt(i).Object.CommunityInvites
                });
            }
            return publicUsers;
        }

        public async Task<bool> isCommunityMember(CommunityProfile community)
        {
            string current = GetCurrentUser();
            var comm = (await firebase
                .Child("Communities")
                .OnceAsync<CommunityProfile>()).Where(a => a.Object.Leader == community.Leader && a.Object.CommunityID.ToString() == community.CommunityID).FirstOrDefault();

            if (comm.Object.Members.Contains(current))
                return true;

            return false;
        }

        public async Task<bool> isEventMember(EventProfile evt)
        {
            string current = GetCurrentUser();
            var selectedEvt = (await firebase
                            .Child("Events")
                            .OnceAsync<EventProfile>()).Where(a => a.Object.EventOwner == evt.EventOwner && a.Object.EventID.ToString() == evt.EventID).FirstOrDefault();

            if (selectedEvt.Object.Members.Contains(current))
                return true;

            return false;
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
                    Description = textInfo.ToTitleCase(commList.ElementAt(i).Object.Description),
                    Location = textInfo.ToTitleCase(commList.ElementAt(i).Object.Location),
                    Leader = commList.ElementAt(i).Object.Leader,
                    Interests = commList.ElementAt(i).Object.Interests,
                    PublicCommunity = commList.ElementAt(i).Object.PublicCommunity,
                    InviteOnly = commList.ElementAt(i).Object.InviteOnly,
                    Members = commList.ElementAt(i).Object.Members,
                    CommunityID = commList.ElementAt(i).Object.CommunityID,
                    CommunityImage = commList.ElementAt(i).Object.CommunityImage,
                    CommunityRequests = commList.ElementAt(i).Object.CommunityRequests
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
                    PublicEvent = eventsList.ElementAt(i).Object.PublicEvent,
                    EventID = eventsList.ElementAt(i).Object.EventID,
                    EventImage = eventsList.ElementAt(i).Object.EventImage,
                    Members = eventsList.ElementAt(i).Object.Members
                   
                });
            }
            return publicEvents;
        }

        public async Task<bool> JoinCommunity(CommunityProfile community)
        {
            string currentUser = GetCurrentUser();

            var communityToUpdate = (await firebase
                                    .Child("Communities")
                                    .OnceAsync<CommunityProfile>()).Where(a => a.Object.CommunityID == community.CommunityID).FirstOrDefault();

            bool added = false;
            if (!communityToUpdate.Object.InviteOnly && communityToUpdate.Object.PublicCommunity)
            {
                added = await AddUserToCommunity(currentUser, community);
            }
            else if (communityToUpdate.Object.InviteOnly || !communityToUpdate.Object.PublicCommunity)
            {
                added = await SendCommunityRequest(currentUser, community);
            }
            return added;
        }

        public async Task<bool> JoinEvent(EventProfile event_)
        {
            string currentUser = GetCurrentUser();

            var eventToUpdate = (await firebase
                                    .Child("Events")
                                    .OnceAsync<EventProfile>()).Where(a => a.Object.EventOwner == event_.EventOwner && a.Object.Name == event_.Name.ToLower()).FirstOrDefault();

            bool added = false;
            if (eventToUpdate.Object.PublicEvent)
            {
                added = await AddUserToEvent(currentUser, event_);
            }
            else
            {
                added = await SendEventRequest(currentUser, event_);
            }
            return added;
        }
        private async Task<bool> AddUserToCommunity(string currentuser, CommunityProfile community)
        {
            var communityToUpdate = (await firebase
                        .Child("Communities")
                        .OnceAsync<CommunityProfile>()).Where(a => a.Object.Leader == community.Leader && a.Object.Name == community.Name.ToLower()).FirstOrDefault();

            List<string> memberList = new List<string>();
            for (int i = 0; i < community.Members.Count; i++)
            {
                memberList.Add(community.Members.ElementAt(i));
            }
            memberList.Add(currentuser);
            await firebase
                .Child("Communities")
                .Child(communityToUpdate.Key)
                .Child("Members")
                .PutAsync(memberList);

            return true;
        }

        private async Task<bool> AddUserToEvent(string currentuser, EventProfile event_)
        {
            var eventToUpdate = (await firebase
                        .Child("Events")
                        .OnceAsync<EventProfile>()).Where(a => a.Object.EventOwner == event_.EventOwner && a.Object.Name == event_.Name.ToLower()).FirstOrDefault();

            List<string> memberList = new List<string>();
            for (int i = 0; i < event_.Members.Count; i++)
            {
                memberList.Add(event_.Members.ElementAt(i));
            }
            memberList.Add(currentuser);
            await firebase
                .Child("Events")
                .Child(eventToUpdate.Key)
                .Child("Members")
                .PutAsync(memberList);

            return true;
        }
        public async void AddFriend(string friendEmail)
        {
            string CurrentUserEmail = GetCurrentUser();
            var friendToAdd = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == friendEmail).FirstOrDefault();
            if (friendToAdd.Object.PublicAcct && friendToAdd.Object.Email != CurrentUserEmail)
            {
                bool added = await AddUserAsFriend(CurrentUserEmail, friendEmail);
            }
            else
            {
                if (friendToAdd.Object.Email != CurrentUserEmail)
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
                    ProfileImage = CurrentUser.ProfileImage,
                    EventRequests = CurrentUser.EventRequests,
                    CommunityRequests = CurrentUser.CommunityRequests,
                    CommunityInvites = CurrentUser.CommunityInvites
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
                    ProfileImage = FriendUser.ProfileImage,
                    EventRequests = FriendUser.EventRequests,
                    CommunityRequests = FriendUser.CommunityRequests,
                    CommunityInvites = FriendUser.CommunityInvites
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

        private async Task<bool> SendCommunityRequest(string currentUser, CommunityProfile communityToJoin)
        {
            var current = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == currentUser).FirstOrDefault();

            var leader = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == communityToJoin.Leader).FirstOrDefault();

            var comm = (await firebase
                .Child("Communities")
                .OnceAsync<CommunityProfile>()).Where(a => a.Object.CommunityID == communityToJoin.CommunityID).FirstOrDefault();


            List<UserProfile> users = await GetAllUsers();
            var CurrentUser = users.Where(a => a.Email == currentUser).FirstOrDefault();
            var leaderUser = users.Where(a => a.Email == communityToJoin.Leader).FirstOrDefault();
            List<string> notification = leaderUser.CommunityRequests;
            if (notification == null)
                notification = new List<string>();
            if (!notification.Contains(currentUser))
                notification.Add(currentUser);

            List<string> commRequests = communityToJoin.CommunityRequests;
            if (commRequests == null)
                commRequests = new List<string>();
            if (!commRequests.Contains(currentUser))
                commRequests.Add(currentUser);
            await firebase
                .Child("userprofiles")
                .Child(leader.Key)
                .PutAsync(new UserProfile()
                {
                    Name = leader.Object.Name.ToLower(),
                    Email = leader.Object.Email,
                    DateOfBirth = leader.Object.DateOfBirth,
                    Password = leader.Object.Password,
                    Location = leader.Object.Location.ToLower(),
                    Interests = leader.Object.Interests,
                    Friends = leader.Object.Friends,
                    FriendRequests = leader.Object.FriendRequests,
                    PublicAcct = leader.Object.PublicAcct,
                    ProfileImage = leader.Object.ProfileImage,
                    EventRequests = leader.Object.EventRequests,
                    CommunityRequests = notification,
                    CommunityInvites = leader.Object.CommunityInvites
                });

            await firebase
                .Child("Communities")
                .Child(comm.Key)
                .PutAsync(new CommunityProfile
                {
                    Leader = comm.Object.Leader,
                    Description = comm.Object.Description,
                    Location = comm.Object.Location,
                    Interests = comm.Object.Interests,
                    Name = comm.Object.Name,
                    PublicCommunity = comm.Object.PublicCommunity,
                    InviteOnly = comm.Object.InviteOnly,
                    Members = comm.Object.Members,
                    CommunityID = comm.Object.CommunityID,
                    CommunityImage = comm.Object.CommunityImage,
                    CommunityRequests = commRequests
                });

            return true;
        }
        private async Task<bool> SendEventRequest(string currentUser, EventProfile eventToJoin)
        {
            var current = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == currentUser).FirstOrDefault();

            var leader = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == eventToJoin.EventOwner).FirstOrDefault();

            var event_ = (await firebase
                .Child("Events")
                .OnceAsync<EventProfile>()).Where(a => a.Object.EventOwner == eventToJoin.EventOwner && a.Object.Name == eventToJoin.Name).FirstOrDefault();


            List<UserProfile> users = await GetAllUsers();
            var CurrentUser = users.Where(a => a.Email == currentUser).FirstOrDefault();
            var leaderUser = users.Where(a => a.Email == eventToJoin.EventOwner).FirstOrDefault();
            List<string> notification = leaderUser.EventRequests;
            if (notification == null)
                notification = new List<string>();
            if (!notification.Contains(currentUser))
                notification.Add(currentUser);

            List<string> eventRequests = eventToJoin.EventRequests;
            if (!eventRequests.Contains(currentUser))
                eventRequests.Add(currentUser);
            await firebase
                .Child("userprofiles")
                .Child(leader.Key)
                .PutAsync(new UserProfile()
                {
                    Name = leader.Object.Name.ToLower(),
                    Email = leader.Object.Email,
                    DateOfBirth = leader.Object.DateOfBirth,
                    Password = leader.Object.Password,
                    Location = leader.Object.Location.ToLower(),
                    Interests = leader.Object.Interests,
                    Friends = leader.Object.Friends,
                    FriendRequests = leader.Object.FriendRequests,
                    PublicAcct = leader.Object.PublicAcct,
                    ProfileImage = leader.Object.ProfileImage,
                    EventRequests = leader.Object.EventRequests,
                    CommunityRequests = notification,
                    CommunityInvites = leader.Object.CommunityInvites
                });

            await firebase
                .Child("Events")
                .Child(event_.Key)
                .PutAsync(new EventProfile
                {
                    EventOwner = event_.Object.EventOwner,
                    Description = event_.Object.Description,
                    Location = event_.Object.Location,
                    Interests = event_.Object.Interests,
                    Name = event_.Object.Name,
                    DateOfEvent = event_.Object.DateOfEvent,
                    StartOfEvent = event_.Object.StartOfEvent,
                    EndOfEvent = event_.Object.EndOfEvent,
                    PublicEvent = event_.Object.PublicEvent,
                    Members = event_.Object.Members,
                    EventID = event_.Object.EventID,
                    EventImage = event_.Object.EventImage,
                    EventRequests = eventRequests
                });

            return true;
        }

        public async Task<List<EventProfile>> GetAllEvents(string InterestTag)
        {
            var events = (await firebase
            .Child("Events")
            .OnceAsync<EventProfile>()).Where(a => a.Object.Interests.Contains(InterestTag)).ToList();

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
                    PublicEvent = eventsList.ElementAt(i).Object.PublicEvent,
                    EventID = eventsList.ElementAt(i).Object.EventID,
                    EventImage = eventsList.ElementAt(i).Object.EventImage,
                    Members = eventsList.ElementAt(i).Object.Members

                });
            }
            return publicEvents;
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
            if (!notification.Contains(currentUser))
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
                    ProfileImage = FriendUser.ProfileImage,
                    EventRequests = FriendUser.EventRequests,
                    CommunityRequests = FriendUser.CommunityRequests,
                    CommunityInvites = FriendUser.CommunityInvites
                });

        }

        public async void SendCommunityInvites(CommunityProfile community, List<string> usersToInvite)
        {
            for (int i = 0; i < usersToInvite.Count; i++)
            {
                var user = (await firebase
                    .Child("userprofiles")
                    .OnceAsync<UserProfile>()).Where(a => a.Object.Email == usersToInvite.ElementAt(i)).FirstOrDefault();

                List<string> commInvites = user.Object.CommunityInvites;
                if (commInvites == null || commInvites.Count == 0)
                    commInvites = new List<string>();

                if (!commInvites.Contains(community.CommunityID))
                    commInvites.Add(community.CommunityID);

                await firebase.Child("userprofiles").Child(user.Key).PutAsync(new UserProfile()
                {
                    Name = user.Object.Name,
                    DateOfBirth = user.Object.DateOfBirth,
                    Email = user.Object.Email,
                    Password = user.Object.Password,
                    Location = user.Object.Location,
                    CommunityInvites = commInvites,
                    CommunityRequests = user.Object.CommunityRequests,
                    ProfileImage = user.Object.ProfileImage,
                    EventRequests = user.Object.EventRequests,
                    Friends = user.Object.Friends,
                    Interests = user.Object.Interests,
                    FriendRequests = user.Object.FriendRequests,
                    PublicAcct = user.Object.PublicAcct

                });

            }
        }

        public async void SendEventInvites(EventProfile Event, List<string> usersToInvite)
        {
            for (int i = 0; i < usersToInvite.Count; i++)
            {
                var user = (await firebase
                    .Child("userprofiles")
                    .OnceAsync<UserProfile>()).Where(a => a.Object.Email == usersToInvite.ElementAt(i)).FirstOrDefault();

                List<string> eventInvites = user.Object.CommunityInvites;
                if (eventInvites == null || eventInvites.Count == 0)
                    eventInvites = new List<string>();

                if (!eventInvites.Contains(Event.EventID))
                    eventInvites.Add(Event.EventID);

                await firebase.Child("userprofiles").Child(user.Key).PutAsync(new UserProfile()
                {
                    Name = user.Object.Name,
                    DateOfBirth = user.Object.DateOfBirth,
                    Email = user.Object.Email,
                    Password = user.Object.Password,
                    Location = user.Object.Location,
                    EventInvites = eventInvites,
                    CommunityRequests = user.Object.CommunityRequests,
                    ProfileImage = user.Object.ProfileImage,
                    EventRequests = user.Object.EventRequests,
                    Friends = user.Object.Friends,
                    Interests = user.Object.Interests,
                    FriendRequests = user.Object.FriendRequests,
                    PublicAcct = user.Object.PublicAcct

                });

            }
        }

        public async Task<List<EventProfile>> GetEventInvites()
        {
            string current = GetCurrentUser();
            var currentUser = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == current).FirstOrDefault();

            List<EventProfile> invites = new List<EventProfile>();
            if (currentUser.Object.EventInvites != null)
            {
                for (int i = 0; i < currentUser.Object.EventInvites.Count; i++)
                {
                    EventProfile Event = await GetEvent(currentUser.Object.EventInvites.ElementAt(i));
                    invites.Add(Event);
                }
            }


            return invites;
        }

        public async Task<List<CommunityProfile>> GetCommunityInvites()
        {
            string current = GetCurrentUser();
            var currentUser = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == current).FirstOrDefault();

            List<CommunityProfile> invites = new List<CommunityProfile>();
            if (currentUser.Object.CommunityInvites != null)
            {
                for (int i = 0; i < currentUser.Object.CommunityInvites.Count; i++)
                {
                    CommunityProfile comm = await GetCommunity(currentUser.Object.CommunityInvites.ElementAt(i));
                    invites.Add(comm);
                }
            }


            return invites;            
        }

        public async Task<CommunityProfile> GetCommunity(string communityID)
        {
            var comm = (await firebase.Child("Communities").OnceAsync<CommunityProfile>()).Where(a => a.Object.CommunityID == communityID).FirstOrDefault();
            CommunityProfile community = new CommunityProfile()
            {
                CommunityID = comm.Object.CommunityID,
                CommunityRequests = comm.Object.CommunityRequests,
                PublicCommunity = comm.Object.PublicCommunity,
                Members = comm.Object.Members,
                Interests = comm.Object.Interests,
                Description = comm.Object.Description,
                InviteOnly = comm.Object.InviteOnly,
                CommunityImage = comm.Object.CommunityImage,
                Leader = comm.Object.Leader,
                Location = comm.Object.Location,
                Name = comm.Object.Name
            };
            return community;
        }

        public async Task<EventProfile> GetEvent(string eventID)
        {
            var Event = (await firebase.Child("Events").OnceAsync<EventProfile>()).Where(a => a.Object.EventID == eventID).FirstOrDefault();
            EventProfile NewEvent = new EventProfile()
            {
                EventID = Event.Object.EventID,
                EventRequests = Event.Object.EventRequests,
                PublicEvent = Event.Object.PublicEvent,
                Members = Event.Object.Members,
                Interests = Event.Object.Interests,
                Description = Event.Object.Description,
                DateOfEvent = Event.Object.DateOfEvent,
                StartOfEvent = Event.Object.StartOfEvent,
                EndOfEvent = Event.Object.EndOfEvent,
                EventImage = Event.Object.EventImage,
                EventOwner = Event.Object.EventOwner,
                Location = Event.Object.Location,
                Name = Event.Object.Name
            };
            return NewEvent;
        }
        public async Task<List<UserProfile>> GetCommunityRequests()
        {
            string current = GetCurrentUser();
            var currentUser = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == current).FirstOrDefault();
            List<UserProfile> users = await GetAllUsers();
            var CurrentUser = users.Where(a => a.Email == current).FirstOrDefault();

            List<string> notifications = new List<string>();
            List<UserProfile> commRequests = new List<UserProfile>();

            if (CurrentUser.CommunityRequests == null)
                return commRequests;

            for (int i = 0; i < currentUser.Object.CommunityRequests.Count; i++)
            {
                UserProfile userToAdd = await GetProfileOfUser(currentUser.Object.CommunityRequests.ElementAt(i));
                commRequests.Add(new UserProfile()
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
                    ProfileImage = userToAdd.ProfileImage,
                    EventRequests = userToAdd.EventRequests,
                    CommunityRequests = userToAdd.CommunityRequests,
                    CommunityInvites = userToAdd.CommunityInvites
                });
            }
            return commRequests;

        }

        public async Task<List<UserProfile>> GetEventRequests()
        {
            string current = GetCurrentUser();
            var currentUser = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == current).FirstOrDefault();
            List<UserProfile> users = await GetAllUsers();
            var CurrentUser = users.Where(a => a.Email == current).FirstOrDefault();
            List<string> notifications = new List<string>();
            List<UserProfile> eventRequests = new List<UserProfile>();
            if (CurrentUser.EventRequests == null)
                return eventRequests;
            for (int i = 0; i < currentUser.Object.EventRequests.Count; i++)
            {
                UserProfile userToAdd = await GetProfileOfUser(currentUser.Object.EventRequests.ElementAt(i));
                eventRequests.Add(new UserProfile()
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
                    ProfileImage = userToAdd.ProfileImage,
                    EventRequests = userToAdd.EventRequests,
                    CommunityRequests = userToAdd.CommunityRequests,
                    CommunityInvites = userToAdd.CommunityInvites
                });
            }
            return eventRequests;

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
                    ProfileImage = userToAdd.ProfileImage,
                    EventRequests = userToAdd.EventRequests,
                    CommunityRequests = userToAdd.CommunityRequests,
                    CommunityInvites = userToAdd.CommunityInvites
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

            if (currentUser.Object.FriendRequests != null && currentUser.Object.FriendRequests.Contains(userToRemove.Email))
            {
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
                        ProfileImage = currentUser.Object.ProfileImage,
                        EventRequests = currentUser.Object.EventRequests,
                        EventInvites = currentUser.Object.EventInvites,
                        CommunityRequests = currentUser.Object.CommunityRequests,
                        CommunityInvites = currentUser.Object.CommunityInvites
                    });
            }
            else if (currentUser.Object.CommunityRequests != null && currentUser.Object.CommunityRequests.Contains(userToRemove.Email))
            {
                List<CommunityProfile> comms = await GetAllCommunities();
                var community = comms.Where(a => a.CommunityRequests.Contains(userToRemove.Email)).FirstOrDefault();
                List<string> commRequests = currentUser.Object.CommunityRequests;
                commRequests.Remove(userToRemove.Email);
                List<string> updated = new List<string>();
                for (int i = 0; i < commRequests.Count; i++)
                {
                    updated.Add(commRequests.ElementAt(i));
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
                        FriendRequests = currentUser.Object.FriendRequests,
                        PublicAcct = currentUser.Object.PublicAcct,
                        ProfileImage = currentUser.Object.ProfileImage,
                        EventRequests = currentUser.Object.EventRequests,
                        EventInvites = currentUser.Object.EventInvites,
                        CommunityRequests = updated,
                        CommunityInvites = currentUser.Object.CommunityInvites
                    });

                List<string> commRequestsinComm = community.CommunityRequests;
                commRequestsinComm.Remove(userToRemove.Email);
                List<string> newRequestList = new List<string>();
                for (int i = 0; i < commRequestsinComm.Count; i++)
                {
                    newRequestList.Add(commRequests.ElementAt(i));
                }
                var commToUpdate = (await firebase.Child("Communities").OnceAsync<CommunityProfile>()).Where(a => a.Object.Leader == community.Leader && a.Object.Name == community.Name.ToLower()).FirstOrDefault();

                await firebase
                        .Child("Communities")
                        .Child(commToUpdate.Key)
                        .PutAsync(new CommunityProfile
                        {
                            Leader = commToUpdate.Object.Leader,
                            Description = commToUpdate.Object.Description,
                            Location = commToUpdate.Object.Location,
                            Interests = commToUpdate.Object.Interests,
                            Name = commToUpdate.Object.Name,
                            PublicCommunity = commToUpdate.Object.PublicCommunity,
                            InviteOnly = commToUpdate.Object.InviteOnly,
                            Members = commToUpdate.Object.Members,
                            CommunityID = commToUpdate.Object.CommunityID,
                            CommunityImage = commToUpdate.Object.CommunityImage,
                            CommunityRequests = newRequestList
                        });


            }
            else if (currentUser.Object.EventRequests != null && currentUser.Object.EventRequests.Contains(userToRemove.Email))
            {
                List<EventProfile> Event = await GetAllEvents();
                var event_ = Event.Where(a => a.EventRequests.Contains(userToRemove.Email)).FirstOrDefault();
                List<string> eventRequests = currentUser.Object.EventRequests;
                eventRequests.Remove(userToRemove.Email);
                List<string> updated = new List<string>();
                for (int i = 0; i < eventRequests.Count; i++)
                {
                    updated.Add(eventRequests.ElementAt(i));
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
                        FriendRequests = currentUser.Object.FriendRequests,
                        PublicAcct = currentUser.Object.PublicAcct,
                        ProfileImage = currentUser.Object.ProfileImage,
                        EventRequests = updated,
                        EventInvites = currentUser.Object.EventInvites,
                        CommunityRequests = currentUser.Object.CommunityRequests,
                        CommunityInvites = currentUser.Object.CommunityInvites
                    });

                List<string> EventRequestsinEvent = event_.EventRequests;
                EventRequestsinEvent.Remove(userToRemove.Email);
                List<string> newRequestList = new List<string>();
                for (int i = 0; i < EventRequestsinEvent.Count; i++)
                {
                    newRequestList.Add(eventRequests.ElementAt(i));
                }
                var EventToUpdate = (await firebase.Child("Events").OnceAsync<EventProfile>()).Where(a => a.Object.EventOwner == event_.EventOwner && a.Object.Name == event_.Name.ToLower()).FirstOrDefault();

                await firebase
                        .Child("Events")
                        .Child(EventToUpdate.Key)
                        .PutAsync(new EventProfile
                        {
                            EventOwner = EventToUpdate.Object.EventOwner,
                            Description = EventToUpdate.Object.Description,
                            Location = EventToUpdate.Object.Location,
                            Interests = EventToUpdate.Object.Interests,
                            Name = EventToUpdate.Object.Name,
                            PublicEvent = EventToUpdate.Object.PublicEvent,
                            Members = EventToUpdate.Object.Members,
                            EventID = EventToUpdate.Object.EventID,
                            EventImage = EventToUpdate.Object.EventImage,
                            DateOfEvent = EventToUpdate.Object.DateOfEvent,
                            StartOfEvent = EventToUpdate.Object.StartOfEvent,
                            EndOfEvent = EventToUpdate.Object.EndOfEvent,
                            EventRequests = newRequestList
                        });


            }




        }

        public async void AcceptRequest(EventProfile evt, CommunityProfile comm, UserProfile newFriend)
        {
            string current = GetCurrentUser();
            var currentUser = (await firebase
                .Child("userprofiles")
                .OnceAsync<UserProfile>()).Where(a => a.Object.Email == current).FirstOrDefault();
            if (currentUser.Object.FriendRequests !=null)
            {
                if (currentUser.Object.FriendRequests.Contains(newFriend.Email))
                {
                    bool added = await AddUserAsFriend(current, newFriend.Email);
                    RemoveRequest(newFriend);
                }
            }
            else if (currentUser.Object.EventRequests != null)
            {
                if (currentUser.Object.EventRequests.Contains(newFriend.Email))
                {
                    List<EventProfile> events = await GetAllEvents();
                    var events_ = events.Where(a => a.EventRequests.Contains(newFriend.Email)).FirstOrDefault();
                    bool added = await AddUserToEvent(newFriend.Email, events_);
                    RemoveRequest(newFriend);
                }
            }
            else if (currentUser.Object.CommunityRequests != null)
            {
                if (currentUser.Object.CommunityRequests.Contains(newFriend.Email))
                {
                    List<CommunityProfile> comms = await GetAllCommunities();
                    var community = comms.Where(a => a.CommunityRequests.Contains(newFriend.Email)).FirstOrDefault();
                    bool added = await AddUserToCommunity(newFriend.Email, community);
                    RemoveRequest(newFriend);
                }
            }

            if (comm != null)
            {
                bool added = await AddUserToCommunity(current, comm);
                RemoveInvite(null, comm.CommunityID, current);
            }
            if (evt != null)
            {
                bool added = await AddUserToEvent(current, evt);
                RemoveInvite(evt.EventID, null, current);
            }



        }

        public async void RemoveInvite(string eventID, string commID, string currentUser)
        {
            if (commID != null)
            {
                var current = (await firebase
                    .Child("userprofiles")
                    .OnceAsync<UserProfile>()).Where(a => a.Object.Email == currentUser).FirstOrDefault();
                List<string> commInvites = current.Object.CommunityInvites;
                List<string> updated = new List<string>();
                if (commInvites != null && commInvites.Contains(commID))
                { 
                    for (int i = 0; i < commInvites.Count; i++)
                    {
                        if (commInvites.ElementAt(i) != commID)
                        {
                            updated.Add(commInvites.ElementAt(i));
                        }
                    }
                    await firebase
                        .Child("userprofiles")
                        .Child(current.Key)
                        .Child("CommunityInvites")
                        .PutAsync(updated);
                }

            }
            if (eventID != null)
            {
                var current = (await firebase
                   .Child("userprofiles")
                   .OnceAsync<UserProfile>()).Where(a => a.Object.Email == currentUser).FirstOrDefault();
                List<string> eventInvites = current.Object.EventInvites;
                List<string> updated = new List<string>();
                if (eventInvites != null && eventInvites.Contains(eventID))
                {
                    for (int i = 0; i < eventInvites.Count; i++)
                    {
                        if (eventInvites.ElementAt(i) != eventID)
                        {
                            updated.Add(eventInvites.ElementAt(i));
                        }
                    }
                    await firebase
                        .Child("userprofiles")
                        .Child(current.Key)
                        .Child("EventInvites")
                        .PutAsync(updated);
                }
            }
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
                    ProfileImage = currentUser.Object.ProfileImage,
                    EventRequests = currentUser.Object.EventRequests,
                    CommunityRequests = currentUser.Object.CommunityRequests,
                    CommunityInvites = currentUser.Object.CommunityInvites
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
                    ProfileImage = friendUser.Object.ProfileImage,
                    EventRequests = friendUser.Object.EventRequests,
                    CommunityRequests = friendUser.Object.CommunityRequests,
                    CommunityInvites = friendUser.Object.CommunityInvites
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
                        ProfileImage = friend.ProfileImage,
                        EventRequests = friend.EventRequests,
                        CommunityRequests = friend.CommunityRequests,
                        CommunityInvites = friend.CommunityInvites
                });
            }

            }
            return friends;


        }

        public async Task<List<UserProfile>> GetCommunityMembers(CommunityProfile community)
        {
            List<UserProfile> users = await GetAllUsers();

            List<UserProfile> members = new List<UserProfile>();
            if (community.Members != null)
            {
                for (int i = 0; i < community.Members.Count; i++)
                {
                    var friend = users.Where(a => a.Email == community.Members.ElementAt(i)).FirstOrDefault();
                    members.Add(new UserProfile()
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
                        ProfileImage = friend.ProfileImage,
                        EventRequests = friend.EventRequests,
                        CommunityRequests = friend.CommunityRequests,
                        CommunityInvites = friend.CommunityInvites
                    });
                }

            }
            return members;


        }


    }
}
