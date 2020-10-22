using Manifest.Login.Apple;
using Manifest.Services;
using Manifest.Views;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manifest.ViewModels
{
    public class SplashScreenViewModel : BaseViewModel
    {
        Repository repository = Repository.Instance;

        public const string LoggedInKey = "LoggedIn";
        public const string AppleUserIdKey = "AppleUserIdKey";
        string userId;

        public async void Init()
        {
            OnStart();

            if (Application.Current.Properties.ContainsKey("user_id"))
            {
                await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }

        async void OnStart()
        {
            var appleSignInService = DependencyService.Get<IAppleSignInService>();

            if (appleSignInService != null)
            {
                userId = await SecureStorage.GetAsync(AppleUserIdKey);

                if (appleSignInService.IsAvailable && !string.IsNullOrEmpty(userId))
                {
                    var credentialState = await appleSignInService.GetCredentialStateAsync(userId);
                    switch (credentialState)
                    {
                        case AppleSignInCredentialState.Authorized:
                            break;
                        case AppleSignInCredentialState.NotFound:
                        case AppleSignInCredentialState.Revoked:
                            //Logout;
                            SecureStorage.Remove(AppleUserIdKey);
                            Preferences.Set(LoggedInKey, false);
                            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                            break;
                    }
                }
            }
        }
    }
}
