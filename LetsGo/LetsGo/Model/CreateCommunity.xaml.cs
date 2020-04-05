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
    public partial class CreateCommunityPage : ContentPage
    {
        readonly FirebaseDB fb = new FirebaseDB();
        public async Task<bool> CreateCommunity(string userEmail, string description, string location, string interests, string name, bool publicCommunity, bool isInviteOnly, List<string> mems, Guid id)
        {
            bool created = await fb.InitializeCommunity(userEmail, description, location, interests, name, publicCommunity, isInviteOnly, mems, id);

            return created;
        }
    }
}
