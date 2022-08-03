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

        private PhotoElement _selectedPhotoElementInWorkspace;

        public PhotoElement SelectedPhotoElementInWorkspace
        {
            get { return _selectedPhotoElementInWorkspace; }
            set
            {
                _selectedPhotoElementInWorkspace = value;

                if (_selectedPhotoElementInWorkspace is not null)
                {
                    GrayScaleSlider.Value = _selectedPhotoElementInWorkspace.Grayscale;
                    ContrastSlider.Value = _selectedPhotoElementInWorkspace.Contrast;
                    BrightnessSlider.Value = _selectedPhotoElementInWorkspace.Brightness;
                    SaturationSlider.Value = _selectedPhotoElementInWorkspace.Saturation;
                    SepiaSlider.Value = _selectedPhotoElementInWorkspace.Sepia;
                    InvertSlider.Value = _selectedPhotoElementInWorkspace.Invert;
                    HueRotateSlider.Value = _selectedPhotoElementInWorkspace.Hue;
                    BlurSlider.Value = _selectedPhotoElementInWorkspace.Blur;
                    SizeSlider.Value = _selectedPhotoElementInWorkspace.Width;
                    OpacitySlider.Value = _selectedPhotoElementInWorkspace.Opacity;

                    //TODO: set image source for the selected image                    
                    var photo = this.photos.FirstOrDefault(x => x.Id == _selectedPhotoElementInWorkspace.Id);

                    SelectedPicture.ProfilePicture = null;
                    SelectedPicture.ProfilePicture = photo?.Source;
                    SelectedPicture.Visibility = Visibility.Visible;
                    ImageEffectDrawer.Visibility = Visibility.Visible;
                }
                else
                {
                    SelectedPicture.Visibility = Visibility.Collapsed;
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
#if DEBUG
                Console.WriteLine("DragElement");
#endif
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
#if DEBUG
            Console.WriteLine("DragRelease");
#endif
            _isPointerCaptured = false;
            uielement.ReleasePointerCapture(currentPointer);
            uielement.Opacity = 1;
        }

        #endregion

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is List<Photo> photos)
            {
                this.photos = photos;
                ImageGallery.ItemsSource = this.photos;
            }

            base.OnNavigatedTo(e);
        }

        private void NumberBoxWidth_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            Workspace.Width = args.NewValue;
#if DEBUG
            Console.WriteLine($"Workspace Size: {Workspace.Width} x {Workspace.Height}");
#endif
        }

        private void NumberBoxHeight_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            Workspace.Height = args.NewValue;
#if DEBUG
            Console.WriteLine($"Workspace Size: {Workspace.Width} x {Workspace.Height}");
#endif
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
                                PhotoElement photoElement = new PhotoElement()
                                {
                                    Id = selectedPhotoInGallery.Id,
                                    Width = 400,
                                    Height = 400
                                };
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
                                    PhotoElement photoElement = new PhotoElement()
                                    {
                                        Id = photo.Id,
                                        Width = 400,
                                        Height = 400,
                                    };
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
            if (_isPointerCaptured && draggingElement is not null)
            {
#if DEBUG
                Console.WriteLine("Workspace_PointerReleased");
#endif
                // this works for mobile and tablets as cursor is not available
                currentPointerPoint = e.GetCurrentPoint(Workspace);
                currentPointer = e.Pointer;

                DragElement(draggingElement);
                DragRelease(draggingElement);

                draggingElement = null;
            }
        }

        private void PhotoElement_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // if image gallery is open then do not start dragging
            if (!ImageGalleryToggleButton.IsChecked.Value && (selectedPhotoInGallery is null || selectedPhotosInGallery is null || !selectedPhotosInGallery.Any()))
            {
#if DEBUG
                Console.WriteLine("PhotoElement_PointerPressed");
#endif
                currentPointerPoint = e.GetCurrentPoint(Workspace);
                currentPointer = e.Pointer;

                draggingElement = (PhotoElement)sender;
                SelectedPhotoElementInWorkspace = (PhotoElement)sender;

                DragStart(draggingElement);
            }
        }

        private void PhotoElement_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // if image gallery is open then do not start dragging
            if (!ImageGalleryToggleButton.IsChecked.Value && (selectedPhotoInGallery is null || selectedPhotosInGallery is null || !selectedPhotosInGallery.Any()))
            {
#if DEBUG
                Console.WriteLine("PhotoElement_PointerReleased");
#endif
                currentPointerPoint = e.GetCurrentPoint(Workspace);
                currentPointer = e.Pointer;

                draggingElement = (PhotoElement)sender;
                DragRelease(draggingElement);
            }
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

        #region Sliders

        private void GrayScaleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Grayscale = e.NewValue;
        }

        private void ContrastSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Contrast = e.NewValue;
        }

        private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Brightness = e.NewValue;
        }

        private void SaturationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Saturation = e.NewValue;
        }

        private void SepiaSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Sepia = e.NewValue;
        }

        private void InvertSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Invert = e.NewValue;
        }

        private void HueRotateSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Hue = e.NewValue;
        }

        private void BlurSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Blur = e.NewValue;
        }

        private void SizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Width = e.NewValue;
            SelectedPhotoElementInWorkspace.Height = e.NewValue;
        }

        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Opacity = e.NewValue;
        }

        #endregion

        #region Toggles

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

        #endregion

        private void ImageResetButton_Click(object sender, RoutedEventArgs e)
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
        }

        private void ImageExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedPhotoElementInWorkspace is not null)
                {
                    var dataUrl = SelectedPhotoElementInWorkspace.Export();

#if DEBUG
                    Console.WriteLine($"ImageExportButton_Click: {dataUrl}");
#endif
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
            if (SelectedPhotoElementInWorkspace is not null)
            {
                Workspace.Children.Remove(SelectedPhotoElementInWorkspace);
                SelectedPhotoElementInWorkspace = null;
            }
        }

        private void ImageSendBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPhotoElementInWorkspace is not null)
            {
                Canvas.SetZIndex(SelectedPhotoElementInWorkspace, Canvas.GetZIndex(SelectedPhotoElementInWorkspace) - 1);
            }
        }

        private void ImageBringForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPhotoElementInWorkspace is not null)
            {
                Canvas.SetZIndex(SelectedPhotoElementInWorkspace, Canvas.GetZIndex(SelectedPhotoElementInWorkspace) + 1);
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

        //private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    this.Workspace.SetZoom(e.NewValue);
        //}

        #endregion
    }
}
