using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LetsGo.Model
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateEvent : ContentPage
    {
        readonly FirebaseDB fb = new FirebaseDB();
        public async Task<bool> CreateUserEvent(string name, string details, DateTime dob, string start, string end, string location, string email, string interests, bool publicAcct)
        {
            bool created = await fb.InitializeEvent(name, details, dob, start, end, location, email,  interests, publicAcct);

            return created;
        }
    }
}