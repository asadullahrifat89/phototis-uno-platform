using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Uno.Storage.Pickers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Phototis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Fields

        private readonly NavigationHelper navigationHelper;

        #endregion

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;

            navigationHelper = new NavigationHelper(
            navigationView: NavView,
            frame: ContentFrame,
            pageMap: new Dictionary<string, Type>() 
            { 
                {"LoginPage", typeof(LoginPage)},
                {"ProjectsPage", typeof(ProjectsPage)},
                {"StagePage", typeof(StagePage)}
            },
            goBackNotAllowedToPages: new List<Type>() { },
            goBackPageRoutes: new List<(Type IfGoingBackTo, Type RouteTo)>());

            DataContext = this;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Navigate(typeof(LoginPage));
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            // log errors
            // show error window
        }

        public void SetAccount()
        {
            AccountUserNameBlock.Text = App.Account.UserName;
            AccountPersonPicture.Initials = Constants.GetInitials(App.Account.UserName);
        }

        public void Navigate(Type page)
        {
            ContentFrame.Navigate(page);
        }

    }
}
