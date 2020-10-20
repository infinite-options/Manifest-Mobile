using Manifest.Services;
using Manifest.Views;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Manifest.ViewModels
{
    public class SplashScreenViewModel : BaseViewModel
    {
        Repository repository = Repository.Instance;
        public async void Init()
        {
            if(Application.Current.Properties.ContainsKey("access_token")
                && Application.Current.Properties.ContainsKey("refresh_token"))
            {
                await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}
