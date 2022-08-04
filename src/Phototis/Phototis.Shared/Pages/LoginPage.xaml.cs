using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Phototis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        #region Ctor

        public LoginPage()
        {
            InitializeComponent();
            Loaded += LoginPage_Loaded;
        }

        #endregion

        #region Events

        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            //App.EnterFullScreen(false);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginButton.IsEnabled)
                Login();
        }

        private void UserNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableLoginButton();
        }

        private void PasswordBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && LoginButton.IsEnabled)
                Login();
        }

        private void PasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableLoginButton();
        }

        private void UserNameBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && LoginButton.IsEnabled)
                Login();

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

        }


        #endregion

        #region Methods

        private async void Login()
        {
            App.SetIsBusy(true, "Preparing studio...");

            await Task.Delay(200);

            App.Account = new Account() { UserName = UserNameBox.Text, /*Password = PasswordBox.Password,*/ };

            App.SetAccount();

            App.NavigateToPage(typeof(WorkspacePage));
        }

        private void EnableLoginButton()
        {
            LoginButton.IsEnabled = !UserNameBox.Text.IsNullOrBlank() /*&& !PasswordBox.Text.IsNullOrBlank()*/;
        }

        #endregion
    }
}
