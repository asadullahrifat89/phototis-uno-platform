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
using Windows.ApplicationModel;
using Windows.UI.ViewManagement;
using Pointer = Microsoft.UI.Xaml.Input.Pointer;

namespace Phototis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WorkspacePage : Page
    {
        #region Fields

        private List<Photo> photos = new List<Photo>();

        private double windowWidth, windowHeight;

        private Photo selectedPhotoInGallery;

        private List<Photo> selectedPhotosInGallery;

        bool _isPointerCaptured;
        double _pointerX;
        double _pointerY;
        double _objectLeft;
        double _objectTop;

        private PhotoElement draggingElement;

        private PointerPoint currentPointerPoint;
        private Pointer currentPointer;

        #endregion

        #region Ctor

        public WorkspacePage()
        {
            this.InitializeComponent();
            this.Loaded += WorkspacePage_Loaded;
            this.Unloaded += WorkspacePage_Unloaded;
        }

        private void WorkspacePage_Loaded(object sender, RoutedEventArgs e)
        {
            NumberBoxWidth.Value = Window.Current.Bounds.Width - 10;
            NumberBoxHeight.Value = Window.Current.Bounds.Height - 10;

            SizeChanged += WorkspacePage_SizeChanged;
        }

        private void WorkspacePage_Unloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= WorkspacePage_SizeChanged;
        }

        private void WorkspacePage_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            windowWidth = args.NewSize.Width - 10;
            windowHeight = args.NewSize.Height - 10;

            WorkspaceScroller.Width = windowWidth;
            WorkspaceScroller.Height = windowHeight;

#if DEBUG
            Console.WriteLine($"View Size: {windowWidth} x {windowHeight}");
#endif
        }

        #endregion

        #region Properties

        private PhotoElement _SelectedPhotoElement;

        public PhotoElement SelectedPhotoElement
        {
            get { return _SelectedPhotoElement; }
            set
            {
                _SelectedPhotoElement = value;

                if (_SelectedPhotoElement is not null)
                {
                    GrayScaleSlider.Value = _SelectedPhotoElement.HtmlImageElement.Grayscale;
                    ContrastSlider.Value = _SelectedPhotoElement.HtmlImageElement.Contrast;
                    BrightnessSlider.Value = _SelectedPhotoElement.HtmlImageElement.Brightness;
                    SaturationSlider.Value = _SelectedPhotoElement.HtmlImageElement.Saturation;
                    SepiaSlider.Value = _SelectedPhotoElement.HtmlImageElement.Sepia;
                    InvertSlider.Value = _SelectedPhotoElement.HtmlImageElement.Invert;
                    HueRotateSlider.Value = _SelectedPhotoElement.HtmlImageElement.Hue;
                    BlurSlider.Value = _SelectedPhotoElement.HtmlImageElement.Blur;
                    SizeSlider.Value = _SelectedPhotoElement.Width;
                    OpacitySlider.Value = _SelectedPhotoElement.HtmlImageElement.Opacity;


                    ImageEffectDrawer.Visibility = Visibility.Visible;
                }
                else
                {
                    ImageEffectDrawer.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Methods       

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

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is List<Photo> photos)
            {
                this.photos = photos;
            }

            ImageGallery.ItemsSource = this.photos;

            base.OnNavigatedTo(e);
        }

        private void NumberBoxWidth_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            Workspace.Width = args.NewValue;

            Console.WriteLine($"Workspace Size: {Workspace.Width} x {Workspace.Height}");
        }

        private void NumberBoxHeight_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            Workspace.Height = args.NewValue;

            Console.WriteLine($"Workspace Size: {Workspace.Width} x {Workspace.Height}");
        }

        private void Workspace_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isPointerCaptured && draggingElement is not null)
            {
                currentPointerPoint = e.GetCurrentPoint(Workspace);
                currentPointer = e.Pointer;

                DragElement(draggingElement);
            }

            //Console.WriteLine("Workspace_PointerMoved");
        }

        private void Workspace_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            currentPointerPoint = e.GetCurrentPoint(Workspace);
            currentPointer = e.Pointer;

            // if image drawer is open then insert new new item
            if (ImageGalleryToggleButton.IsChecked.Value)
            {
                switch (ImageGallery.SelectionMode)
                {
                    case ListViewSelectionMode.Single:
                        {
                            if (selectedPhotoInGallery is not null)
                            {
                                PhotoElement photoElement = new PhotoElement() { Width = 400, Height = 400 };
                                photoElement.Source = selectedPhotoInGallery.DataUrl;

                                Canvas.SetLeft(photoElement, currentPointerPoint.Position.X - 200);
                                Canvas.SetTop(photoElement, currentPointerPoint.Position.Y - 200);

                                photoElement.PointerPressed += PhotoElement_PointerPressed;
                                photoElement.PointerReleased += PhotoElement_PointerReleased;

                                Workspace.Children.Add(photoElement);

                                ImageGallery.SelectedItem = null;
                                selectedPhotoInGallery = null;
                            }
                        }
                        break;
                    case ListViewSelectionMode.Multiple:
                        {
                            if (selectedPhotosInGallery is not null && selectedPhotosInGallery.Any())
                            {
                                foreach (var photo in selectedPhotosInGallery)
                                {
                                    PhotoElement photoElement = new PhotoElement() { Width = 400, Height = 400 };
                                    photoElement.Source = photo.DataUrl;

                                    Canvas.SetLeft(photoElement, currentPointerPoint.Position.X - 200);
                                    Canvas.SetTop(photoElement, currentPointerPoint.Position.Y - 200);

                                    photoElement.PointerPressed += PhotoElement_PointerPressed;
                                    photoElement.PointerReleased += PhotoElement_PointerReleased;

                                    Workspace.Children.Add(photoElement);
                                }

                                ImageGallery.SelectedItems.Clear();
                                selectedPhotosInGallery = null;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

#if DEBUG
            Console.WriteLine("Workspace_PointerPressed");
#endif
        }

        private void Workspace_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // this works for mobile and tablets as cursor is not available
            currentPointerPoint = e.GetCurrentPoint(Workspace);
            currentPointer = e.Pointer;

            if (_isPointerCaptured && draggingElement is not null)
            {
                DragElement(draggingElement);
                DragRelease(draggingElement);

                draggingElement = null;
            }

            Console.WriteLine("Workspace_PointerReleased");
        }

        private void PhotoElement_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            currentPointerPoint = e.GetCurrentPoint(Workspace);
            currentPointer = e.Pointer;

            draggingElement = (PhotoElement)sender;
            SelectedPhotoElement = (PhotoElement)sender;

            DragStart(draggingElement);
        }

        private void PhotoElement_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            draggingElement = (PhotoElement)sender;
            DragRelease(draggingElement);
        }

        private void ImageGallery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ImageGallery.SelectionMode)
            {
                case ListViewSelectionMode.Single:
                    selectedPhotoInGallery = ImageGallery.SelectedItem as Photo;
                    break;
                case ListViewSelectionMode.Multiple:
                    selectedPhotosInGallery = ImageGallery.SelectedItems.OfType<Photo>().ToList();
                    break;
                default:
                    break;
            }
        }

        private void GrayScaleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElement.HtmlImageElement.Grayscale = e.NewValue;
        }

        private void ContrastSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElement.HtmlImageElement.Contrast = e.NewValue;
        }

        private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElement.HtmlImageElement.Brightness = e.NewValue;
        }

        private void SaturationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElement.HtmlImageElement.Saturation = e.NewValue;
        }

        private void SepiaSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElement.HtmlImageElement.Sepia = e.NewValue;
        }

        private void InvertSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElement.HtmlImageElement.Invert = e.NewValue;
        }

        private void HueRotateSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElement.HtmlImageElement.Hue = e.NewValue;
        }

        private void BlurSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElement.HtmlImageElement.Blur = e.NewValue;
        }

        private void SizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElement.Width = e.NewValue;
            SelectedPhotoElement.Height = e.NewValue;
        }

        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElement.HtmlImageElement.Opacity = e.NewValue;
        }

        private void GrayscaleToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ContrastToggleButton.IsChecked = false;
            BrightnessToggleButton.IsChecked = false;
            SaturationToggleButton.IsChecked = false;
            SepiaToggleButton.IsChecked = false;
            InvertToggleButton.IsChecked = false;
            HueToggleButton.IsChecked = false;
            BlurToggleButton.IsChecked = false;
            SizeToggleButton.IsChecked = false;
            OpacityToggleButton.IsChecked = false;
        }

        private void ContrastToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GrayscaleToggleButton.IsChecked = false;
            BrightnessToggleButton.IsChecked = false;
            SaturationToggleButton.IsChecked = false;
            SepiaToggleButton.IsChecked = false;
            InvertToggleButton.IsChecked = false;
            HueToggleButton.IsChecked = false;
            BlurToggleButton.IsChecked = false;
            SizeToggleButton.IsChecked = false;
            OpacityToggleButton.IsChecked = false;
        }

        private void BrightnessToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GrayscaleToggleButton.IsChecked = false;
            ContrastToggleButton.IsChecked = false;
            SaturationToggleButton.IsChecked = false;
            SepiaToggleButton.IsChecked = false;
            InvertToggleButton.IsChecked = false;
            HueToggleButton.IsChecked = false;
            BlurToggleButton.IsChecked = false;
            SizeToggleButton.IsChecked = false;
            OpacityToggleButton.IsChecked = false;
        }

        private void SaturationToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GrayscaleToggleButton.IsChecked = false;
            ContrastToggleButton.IsChecked = false;
            BrightnessToggleButton.IsChecked = false;
            SepiaToggleButton.IsChecked = false;
            InvertToggleButton.IsChecked = false;
            HueToggleButton.IsChecked = false;
            BlurToggleButton.IsChecked = false;
            SizeToggleButton.IsChecked = false;
            OpacityToggleButton.IsChecked = false;
        }

        private void SepiaToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GrayscaleToggleButton.IsChecked = false;
            ContrastToggleButton.IsChecked = false;
            BrightnessToggleButton.IsChecked = false;
            SaturationToggleButton.IsChecked = false;
            InvertToggleButton.IsChecked = false;
            HueToggleButton.IsChecked = false;
            BlurToggleButton.IsChecked = false;
            SizeToggleButton.IsChecked = false;
            OpacityToggleButton.IsChecked = false;
        }

        private void InvertToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GrayscaleToggleButton.IsChecked = false;
            ContrastToggleButton.IsChecked = false;
            BrightnessToggleButton.IsChecked = false;
            SaturationToggleButton.IsChecked = false;
            SepiaToggleButton.IsChecked = false;
            HueToggleButton.IsChecked = false;
            BlurToggleButton.IsChecked = false;
            SizeToggleButton.IsChecked = false;
            OpacityToggleButton.IsChecked = false;
        }

        private void HueToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GrayscaleToggleButton.IsChecked = false;
            ContrastToggleButton.IsChecked = false;
            BrightnessToggleButton.IsChecked = false;
            SaturationToggleButton.IsChecked = false;
            SepiaToggleButton.IsChecked = false;
            InvertToggleButton.IsChecked = false;
            BlurToggleButton.IsChecked = false;
            SizeToggleButton.IsChecked = false;
            OpacityToggleButton.IsChecked = false;
        }

        private void BlurToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GrayscaleToggleButton.IsChecked = false;
            ContrastToggleButton.IsChecked = false;
            BrightnessToggleButton.IsChecked = false;
            SaturationToggleButton.IsChecked = false;
            SepiaToggleButton.IsChecked = false;
            InvertToggleButton.IsChecked = false;
            HueToggleButton.IsChecked = false;
            SizeToggleButton.IsChecked = false;
            OpacityToggleButton.IsChecked = false;
        }

        private void SizeToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GrayscaleToggleButton.IsChecked = false;
            ContrastToggleButton.IsChecked = false;
            BrightnessToggleButton.IsChecked = false;
            SaturationToggleButton.IsChecked = false;
            SepiaToggleButton.IsChecked = false;
            InvertToggleButton.IsChecked = false;
            HueToggleButton.IsChecked = false;
            BlurToggleButton.IsChecked = false;
            OpacityToggleButton.IsChecked = false;
        }

        private void OpacityToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GrayscaleToggleButton.IsChecked = false;
            ContrastToggleButton.IsChecked = false;
            BrightnessToggleButton.IsChecked = false;
            SaturationToggleButton.IsChecked = false;
            SepiaToggleButton.IsChecked = false;
            InvertToggleButton.IsChecked = false;
            HueToggleButton.IsChecked = false;
            BlurToggleButton.IsChecked = false;
            SizeToggleButton.IsChecked = false;
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
            SizeSlider.Value = 400;
            OpacitySlider.Value = 1;

            SelectedPhotoElement.HtmlImageElement.SetDefaults();
        }

        //private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    this.Workspace.SetZoom(e.NewValue);
        //}

        private void SelectMultipleToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ImageGallery.SelectionMode = ListViewSelectionMode.Multiple;
        }

        private void SelectMultipleToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageGallery.SelectionMode = ListViewSelectionMode.Single;
        }

        private void FullscreenToggle_Checked(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();

            if (view is not null)
            {
                view.TryEnterFullScreenMode();
            }
        }

        private void SelectAllToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in ImageGallery.Items)
            {
                ImageGallery.SelectedItems.Add(item);
            }
        }

        private void SelectAllToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageGallery.SelectedItems.Clear();
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPhotoElement is not null)
            {
                Workspace.Children.Remove(SelectedPhotoElement);
                SelectedPhotoElement = null;
            }
        }

        private void FullscreenToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();

            if (view is not null)
            {
                view.ExitFullScreenMode();
            }
        }

        #endregion
    }
}
