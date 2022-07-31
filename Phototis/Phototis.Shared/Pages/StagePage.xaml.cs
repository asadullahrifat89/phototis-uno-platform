using Microsoft.UI.Input;
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
using Pointer = Microsoft.UI.Xaml.Input.Pointer;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Phototis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StagePage : Page
    {
        #region Fields

        private List<Photo> photos = new List<Photo>();

        private double windowWidth, windowHeight;

        private Photo selectedPhoto;

        bool _isPointerCaptured;
        double _pointerX;
        double _pointerY;
        double _objectLeft;
        double _objectTop;

        private UIElement uIElement;
        private PointerPoint currentPointerPoint;
        private Pointer currentPointer;

        #endregion

        #region Ctor

        public StagePage()
        {
            this.InitializeComponent();
            this.Loaded += StagePage_Loaded;
            this.Unloaded += StagePage_Unloaded;
        }

        private void StagePage_Loaded(object sender, RoutedEventArgs e)
        {
            SizeChanged += StagePage_SizeChanged;
        }

        private void StagePage_Unloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= StagePage_SizeChanged;
        }


        private void StagePage_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            windowWidth = args.NewSize.Width - 10; //Window.Current.Bounds.Width;
            windowHeight = args.NewSize.Height - 10; //Window.Current.Bounds.Height;

            StageEnvironment.Width = windowWidth;
            StageEnvironment.Height = windowHeight;

#if DEBUG
            Console.WriteLine($"View Size: {windowWidth} x {windowHeight}");
#endif
        }

        #endregion

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is List<Photo> photos)
            {
                this.photos = photos;
            }

            //double lastHeight = 0;
            //double lastWidth = 0;

            //foreach (var photo in this.photos)
            //{
            //    PhotoElement photoElement = new PhotoElement(photo.DataUrl) { Width = 400, Height = 400 };

            //    Canvas.SetLeft(photoElement, (lastWidth));
            //    Canvas.SetTop(photoElement, 0);

            //    lastHeight = Convert.ToDouble(photoElement.Height);
            //    lastWidth = Convert.ToDouble(photoElement.Width);

            //    StageEnvironment.Children.Add(photoElement);
            //}

            ImageContainer.ItemsSource = this.photos;

            base.OnNavigatedTo(e);
        }

        private void StageEnvironment_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isPointerCaptured && uIElement is not null)
            {
                currentPointerPoint = e.GetCurrentPoint(StageEnvironment);
                currentPointer = e.Pointer;

                DragElement(uIElement);
            }

            Console.WriteLine("StageEnvironment_PointerMoved");
        }

        private void StageEnvironment_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            currentPointerPoint = e.GetCurrentPoint(StageEnvironment);
            currentPointer = e.Pointer;

            // if image drawer is open then insert new new item
            if (ImageDrawer.IsChecked.Value && selectedPhoto is not null)
            {
                PhotoElement photoElement = new PhotoElement(selectedPhoto.DataUrl) { Width = 400, Height = 400 };

                Canvas.SetLeft(photoElement, currentPointerPoint.Position.X - 200);
                Canvas.SetTop(photoElement, currentPointerPoint.Position.Y - 200);

                //photoElement.DoubleTapped += PhotoElement_DoubleTapped;
                
                photoElement.PointerPressed += PhotoElement_PointerPressed;
                //photoElement.PointerMoved += PhotoElement_PointerMoved;
                photoElement.PointerReleased += PhotoElement_PointerReleased;

                photoElement.DragStarting += PhotoElement_DragStarting;

                StageEnvironment.Children.Add(photoElement);

                selectedPhoto = null;
            }

#if DEBUG
            Console.WriteLine("StageEnvironment_PointerPressed");
#endif
        }

        private void PhotoElement_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            Console.WriteLine("PhotoElement_DragStarting");
        }

        private void StageEnvironment_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            currentPointerPoint = e.GetCurrentPoint(StageEnvironment);
            currentPointer = e.Pointer;

            if (_isPointerCaptured && uIElement is not null)
            {
                DragElement(uIElement);

                DragRelease(uIElement);
                uIElement = null;
            }

            Console.WriteLine("StageEnvironment_PointerReleased");
        }

        private void PhotoElement_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            uIElement = (UIElement)sender;

            if (!_isPointerCaptured)
            {
                DragStart(uIElement);
            }
            else
            {
                DragElement(uIElement);
                DragRelease(uIElement);
                uIElement = null;
            }
        }

        private void PhotoElement_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            currentPointerPoint = e.GetCurrentPoint(StageEnvironment);
            currentPointer = e.Pointer;

            uIElement = (UIElement)sender;

            DragStart(uIElement);
        }


        private void PhotoElement_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            uIElement = (UIElement)sender;
            DragRelease(uIElement);
        }

        private void PhotoElement_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            uIElement = (UIElement)sender;
            DragElement(uIElement);
        }


        private void ImageDrawer_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void ImageContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPhoto = ImageContainer.SelectedItem as Photo;
        }

        //private void ChooseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Image1.Source = photos[0].ImageUrl;
        //    Image2.Source = photos[0].ImageUrl;
        //}

        //private void GrayScaleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetGrayScale(e.NewValue);
        //}

        //private void ContrastSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetContrast(e.NewValue);
        //}

        //private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetBrightness(e.NewValue);
        //}

        //private void SaturationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetSaturation(e.NewValue);
        //}

        //private void SepiaSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetSepia(e.NewValue);
        //}

        //private void InvertSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetInvert(e.NewValue);
        //}

        //private void HueRotateSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetHueRotate(e.NewValue);
        //}

        //private void BlurSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetBlur(e.NewValue);
        //}

        //private void ResetButton_Click(object sender, RoutedEventArgs e)
        //{
        //    GrayScaleSlider.Value = 0;
        //    ContrastSlider.Value = 100;
        //    BrightnessSlider.Value = 100;
        //    SaturationSlider.Value = 100;
        //    SepiaSlider.Value = 0;
        //    InvertSlider.Value = 0;
        //    HueRotateSlider.Value = 0;
        //    BlurSlider.Value = 0;
        //} 

        #endregion

        #region Methods

        //private async Task LoadImage()
        //{
        //    try
        //    {
        //        if (FileSystemAccessApiInformation.IsOpenPickerSupported)
        //        {
        //            // File System Access API open picker is available.

        //            var fileOpenPicker = new FileOpenPicker
        //            {
        //                SuggestedStartLocation = PickerLocationId.PicturesLibrary
        //            };
        //            fileOpenPicker.FileTypeFilter.Add(".png");
        //            fileOpenPicker.FileTypeFilter.Add(".jpg");
        //            fileOpenPicker.FileTypeFilter.Add(".jpeg");
        //            fileOpenPicker.FileTypeFilter.Add(".webp");
        //            StorageFile pickedFile = await fileOpenPicker.PickSingleFileAsync();

        //            if (pickedFile != null)
        //            {
        //                // File was picked, you can now use it
        //                var stream = await pickedFile.OpenStreamForReadAsync();

        //                var props = await pickedFile.GetBasicPropertiesAsync();

        //                var fileName = pickedFile.Name;

        //                Console.WriteLine(fileName);

        //                if (!string.IsNullOrEmpty(fileName))
        //                {
        //                    var ms = new MemoryStream();
        //                    stream.Seek(0, SeekOrigin.Begin);
        //                    await stream.CopyToAsync(ms);

        //                    //BitmapImage bitmapImage = new BitmapImage();
        //                    //bitmapImage.SetSource(ms);

        //                    //Image1.Source = bitmapImage;

        //                    Console.WriteLine("Image set!");

        //                    ms.Seek(0, SeekOrigin.Begin);
        //                    var base64String = "data:application/octet-stream;base64," + Convert.ToBase64String(ms.ToArray());

        //                    //Image2.Width = Image1.ActualWidth.ToString();
        //                    //Image2.Height = Image1.ActualHeight.ToString();

        //                    //Photo photo = new Photo() { Source = base64String, Name = "Ori", };
        //                    //Photo photo2 = new Photo() { Source = base64String, Name = "Edit", };

        //                    //var photos = new List<Photo>();
        //                    //photos.Add(photo);
        //                    //photos.Add(photo2);

        //                    //ImageContainer.ItemsSource = photos;

        //                    Image1.Source = base64String;
        //                    //Image1.Width = Window.Current.Bounds.Width.ToString();
        //                    //Image1.Height = Window.Current.Bounds.Height.ToString();

        //                    Image2.Source = base64String;

        //                    ////Image2.GrayScale(100);
        //                    //Image2.SetContrast(100);
        //                    //Image2.SetBrightness(100);
        //                    //Image2.SetSepia(100);

        //                    //Image2.Width = Window.Current.Bounds.Width.ToString();
        //                    //Image2.Height = Window.Current.Bounds.Height.ToString();

        //                    Image1.Visibility = Visibility.Visible;
        //                    Image2.Visibility = Visibility.Visible;
        //                }
        //            }
        //            else
        //            {
        //                // No file was picked or the dialog was cancelled.
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("ERROR: " + ex.Message);
        //    }
        //}

        public void DragStart(UIElement uielement)
        {
            // Drag start of a constuct
            _objectLeft = Canvas.GetLeft(uielement);
            _objectTop = Canvas.GetTop(uielement);

            //var currentPoint = e.GetCurrentPoint(canvas);

            // Remember the pointer position:
            _pointerX = currentPointerPoint.Position.X;
            _pointerY = currentPointerPoint.Position.Y;

            uielement.CapturePointer(currentPointer);
            uielement.Opacity = 0.6d;

            _isPointerCaptured = true;

#if DEBUG
            Console.WriteLine("DragStart");
#endif
        }

        public void DragElement(UIElement uielement)
        {
            if (_isPointerCaptured)
            {
                //var currentPoint = e.GetCurrentPoint(canvas);

                // Calculate the new position of the object:
                double deltaH = currentPointerPoint.Position.X - _pointerX;
                double deltaV = currentPointerPoint.Position.Y - _pointerY;

                _objectLeft = deltaH + _objectLeft;
                _objectTop = deltaV + _objectTop;

                // Update the object position:
                Canvas.SetLeft(uielement, _objectLeft);
                Canvas.SetTop(uielement, _objectTop);

                // Remember the pointer position:
                _pointerX = currentPointerPoint.Position.X;
                _pointerY = currentPointerPoint.Position.Y;
            }
        }

        public void DragRelease(UIElement uielement)
        {
            _isPointerCaptured = false;
            uielement.ReleasePointerCapture(currentPointer);
            uielement.Opacity = 1;

#if DEBUG
            Console.WriteLine("DragRelease");
#endif
        }

        #endregion
    }
}
