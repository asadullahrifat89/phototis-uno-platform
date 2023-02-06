using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Phototis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        #region Ctor
        
        public StartPage()
        {
            InitializeComponent();
            Loaded += StartPage_Loaded;
        } 

        #endregion

        #region Events  

        private void StartPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {            
            App.NavigateToPage(typeof(WorkspacePage));
            //App.EnterFullScreen(true);
        }

        #endregion
    }
}
