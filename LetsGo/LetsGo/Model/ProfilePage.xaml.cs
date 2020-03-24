using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetsGo.Model.Authentication;
using Firebase.Database;
using Firebase.Database.Query;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LetsGo.Model
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {

        }
        readonly FirebaseDB fb = new FirebaseDB();
        public bool LogoutUser()
        {
            bool SignedOut = fb.SignOutUser();
            return SignedOut;
        }

        public async Task<bool> UpdateProfile(string userName,  string userLocation, bool isPublic, List<string> interests)
        {
            
            bool updated = await fb.UpdateUserProfile(userName, userLocation, isPublic, interests);
            return updated;
            
        }

        public async Task<bool> DeleteUser()
        {
            bool deleted = await fb.DeleteUserAccount();
            return deleted;
        }
    }
}