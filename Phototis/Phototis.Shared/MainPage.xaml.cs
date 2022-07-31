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
            await LoadImage();
        }

        private async Task LoadImage()
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
                    fileOpenPicker.FileTypeFilter.Add(".webp");
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

                            //Image1.Source = bitmapImage;

                            Console.WriteLine("Image set!");

                            ms.Seek(0, SeekOrigin.Begin);
                            var base64String = "data:application/octet-stream;base64," + Convert.ToBase64String(ms.ToArray());

                            //Image2.Width = Image1.ActualWidth.ToString();
                            //Image2.Height = Image1.ActualHeight.ToString();

                            //Photo photo = new Photo() { Source = base64String, Name = "Ori", };
                            //Photo photo2 = new Photo() { Source = base64String, Name = "Edit", };

                            //var photos = new List<Photo>();
                            //photos.Add(photo);
                            //photos.Add(photo2);

                            //ImageContainer.ItemsSource = photos;

                            Image1.Source = base64String;
                            //Image1.Width = Window.Current.Bounds.Width.ToString();
                            //Image1.Height = Window.Current.Bounds.Height.ToString();

                            Image2.Source = base64String;

                            ////Image2.GrayScale(100);
                            //Image2.SetContrast(100);
                            //Image2.SetBrightness(100);
                            //Image2.SetSepia(100);

                            //Image2.Width = Window.Current.Bounds.Width.ToString();
                            //Image2.Height = Window.Current.Bounds.Height.ToString();

                            Image1.Visibility = Visibility.Visible;
                            Image2.Visibility = Visibility.Visible;
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

        private void GrayScaleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Image2.SetGrayScale(e.NewValue);
        }

        private void ContrastSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Image2.SetContrast(e.NewValue);
        }

        private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Image2.SetBrightness(e.NewValue);
        }

        private void SaturationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Image2.SetSaturation(e.NewValue);
        }

        private void SepiaSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Image2.SetSepia(e.NewValue);
        }

        private void InvertSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Image2.SetInvert(e.NewValue);
        }

        private void HueRotateSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Image2.SetHueRotate(e.NewValue);
        }

        private void BlurSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Image2.SetBlur(e.NewValue);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            GrayScaleSlider.Value = 0;
            ContrastSlider.Value = 100;            
            BrightnessSlider.Value = 100;
            SaturationSlider.Value = 100;
            SepiaSlider.Value = 0;
            InvertSlider.Value = 0;
            HueRotateSlider.Value = 0;
            BlurSlider.Value = 0;
        }
    }

    public class Photo
    {
        public string Name { get; set; }

        public string Source { get; set; }
    }
}
