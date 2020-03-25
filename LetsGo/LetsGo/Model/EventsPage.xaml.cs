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
    public partial class EventsPage : ContentPage
    {
        readonly FirebaseDB fb = new FirebaseDB();
        public async Task<bool> CreateUserEvent(string name, string details, DateTime dob, TimeSpan start, TimeSpan end, string email, bool publicAcct)
        {
            bool created = await fb.InitializeEvent(name, details, dob, start, end, email, publicAcct);

            return created;
        }
    }
}