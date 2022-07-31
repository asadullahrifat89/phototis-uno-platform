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
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Phototis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectsPage : Page
    {
        private List<Photo> photos;

        public ProjectsPage()
        {
            this.InitializeComponent();
        }

        private async void SelectImagesButton_Click(object sender, RoutedEventArgs e)
        {
            photos = new List<Photo>();

            var fileOpenPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".jpeg");
            fileOpenPicker.FileTypeFilter.Add(".webp");

            var pickedFiles = await fileOpenPicker.PickMultipleFilesAsync();

            if (pickedFiles.Count > 0)
            {
                // At least one file was picked, you can use them
                foreach (var file in pickedFiles)
                {
                    var stream = await file.OpenStreamForReadAsync();
                    stream.Seek(0, SeekOrigin.Begin);

                    var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);

                    ms.Seek(0, SeekOrigin.Begin);
                    var base64String = "data:application/octet-stream;base64," + Convert.ToBase64String(ms.ToArray());

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(ms);

                    Photo photo = new Photo()
                    {
                        Name = file.Name,
                        DataUrl = base64String,
                        StorageFile = file,
                        Source = bitmapImage
                    };

                    photos.Add(photo);
                }

                this.ImagesList.ItemsSource = photos.OrderBy(x => x.Name).ToList();
            }
            else
            {
                // No file was picked or the dialog was cancelled.
            }

            ImagesCount.Text = $"{photos.Count} image(s) selected";
        }

        private void ProceedButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigateToPage(typeof(StagePage), photos);
        }
    }
}
