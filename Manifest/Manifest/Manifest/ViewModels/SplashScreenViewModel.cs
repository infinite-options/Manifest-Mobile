using Manifest.Services;
using Manifest.Views;

using System.Threading.Tasks;
using Xamarin.Forms;

namespace Manifest.ViewModels
{
    public class SplashScreenViewModel : BaseViewModel
    {
        Repository repository = Repository.Instance;
        public async void Init()
        {
            await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
        }
    }
}
