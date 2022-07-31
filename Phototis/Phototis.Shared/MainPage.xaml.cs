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
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FileSystemAccessApiInformation.IsOpenPickerSupported)
                {
                    // File System Access API open picker is available.

                    var fileOpenPicker = new FileOpenPicker
                    {
                        SuggestedStartLocation = PickerLocationId.PicturesLibrary
                    };
                    fileOpenPicker.FileTypeFilter.Add(".png");
                    fileOpenPicker.FileTypeFilter.Add(".jpg");
                    fileOpenPicker.FileTypeFilter.Add(".jpeg");
                    StorageFile pickedFile = await fileOpenPicker.PickSingleFileAsync();

                    if (pickedFile != null)
                    {
                        // File was picked, you can now use it
                        var stream = await pickedFile.OpenStreamForReadAsync();

                        var props = await pickedFile.GetBasicPropertiesAsync();

                        var fileName = pickedFile.Name;

                        Console.WriteLine(fileName);

                        if (!string.IsNullOrEmpty(fileName))
                        {
                            var ms = new MemoryStream();
                            stream.Seek(0, SeekOrigin.Begin);
                            await stream.CopyToAsync(ms);

                            //BitmapImage bitmapImage = new BitmapImage();
                            //bitmapImage.SetSource(ms);

                            //ImageOriginal.Source = bitmapImage;

                            Console.WriteLine("Image set!");

                            ms.Seek(0, SeekOrigin.Begin);
                            var base64String = "data:application/octet-stream;base64," + Convert.ToBase64String(ms.ToArray());

                            //ImageEdited.Width = ImageOriginal.ActualWidth.ToString();
                            //ImageEdited.Height = ImageOriginal.ActualHeight.ToString();

                            Photo photo = new Photo() { Source = base64String, Name = "Ori", };
                            Photo photo2 = new Photo() { Source = base64String, Name = "Edit", };

                            //ImageContainer.ItemsSource = new List<Photo>() { photo, photo2 };

                            ImageOriginal.Source = base64String;
                            ImageOriginal.Width = Window.Current.Bounds.Width.ToString();
                            ImageOriginal.Height = Window.Current.Bounds.Height.ToString();

                            ImageEdited.Source = base64String;

                            //ImageEdited.GrayScale(100);
                            ImageEdited.Contrast(150);
                            ImageEdited.Brightness(150);
                            ImageEdited.Sepia(150);

                            ImageEdited.Width = Window.Current.Bounds.Width.ToString();
                            ImageEdited.Height = Window.Current.Bounds.Height.ToString();
                        }
                    }
                    else
                    {
                        // No file was picked or the dialog was cancelled.
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }


    }

    public class Photo
    {
        public string Name { get; set; }

        public string Source { get; set; }
    }
}
