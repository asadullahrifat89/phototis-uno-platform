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
using Microsoft.UI.Text;
using Microsoft.UI;

namespace Phototis
{
    public sealed partial class WorkspacePage : Page
    {
        #region Fields

        //private List<Photo> Photos = new List<Photo>();

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
            InitializeComponent();
            Loaded += WorkspacePage_Loaded;
            Unloaded += WorkspacePage_Unloaded;
        }

        private void WorkspacePage_Loaded(object sender, RoutedEventArgs e)
        {
            NumberBoxWidth.Value = Window.Current.Bounds.Width - 10;
            NumberBoxHeight.Value = Window.Current.Bounds.Height - 10;

            SizeChanged += WorkspacePage_SizeChanged;

            //await Task.Delay(200);

            App.SetIsBusy(false);
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

        private List<Photo> photos = new List<Photo>();
        public List<Photo> Photos
        {
            get { return photos; }
            set
            {
                photos = value;
            }
        }


        private PhotoElement selectedPhotoElementInWorkspace;
        public PhotoElement SelectedPhotoElementInWorkspace
        {
            get { return selectedPhotoElementInWorkspace; }
            set
            {
                selectedPhotoElementInWorkspace = value;

                if (selectedPhotoElementInWorkspace is not null)
                {
                    GrayScaleSlider.Value = selectedPhotoElementInWorkspace.ImageGrayscale;
                    ContrastSlider.Value = selectedPhotoElementInWorkspace.ImageContrast;
                    BrightnessSlider.Value = selectedPhotoElementInWorkspace.ImageBrightness;
                    SaturationSlider.Value = selectedPhotoElementInWorkspace.ImageSaturation;
                    SepiaSlider.Value = selectedPhotoElementInWorkspace.ImageSepia;
                    InvertSlider.Value = selectedPhotoElementInWorkspace.ImageInvert;
                    HueRotateSlider.Value = selectedPhotoElementInWorkspace.ImageHue;
                    BlurSlider.Value = selectedPhotoElementInWorkspace.ImageBlur;
                    SizeSlider.Value = selectedPhotoElementInWorkspace.Width;
                    OpacitySlider.Value = selectedPhotoElementInWorkspace.ImageOpacity;

                    // set image source for the selected image                    
                    var photo = Photos.FirstOrDefault(x => x.Id == selectedPhotoElementInWorkspace.Id);

                    SelectedPicture.ProfilePicture = null;
                    SelectedPicture.ProfilePicture = photo?.Source;
                    SelectedPicture.Visibility = Visibility.Visible;
                    ImageToolsDrawer.Visibility = Visibility.Visible;

                    SetPhotoElementEditingContext();
                }
                else
                {
                    SelectedPicture.Visibility = Visibility.Collapsed;
                    ImageToolsDrawer.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Methods

        private double GetScalingFactor()
        {
            return windowWidth < 400 ? 0.6 : windowWidth < 600 ? 0.7 : windowWidth < 800 ? 0.8 : windowWidth < 1000 ? 0.9 : 1;
        }

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

        private void AddPhotoElementToWorkspace(Photo photo)
        {
            var scalingFactor = GetScalingFactor();

            PhotoElement photoElement = new PhotoElement()
            {
                Id = photo.Id,
                Width = 400 * scalingFactor,
                Height = 400 * scalingFactor,
            };
            photoElement.Source = photo.DataUrl;

            Canvas.SetLeft(photoElement, currentPointerPoint.Position.X - 200 * scalingFactor);
            Canvas.SetTop(photoElement, currentPointerPoint.Position.Y - 200 * scalingFactor);

            photoElement.PointerPressed += PhotoElement_PointerPressed;
            photoElement.PointerReleased += PhotoElement_PointerReleased;

            Workspace.Children.Add(photoElement);
        }

        private void SetPhotoElementEditingContext()
        {
            if (SelectedPhotoElementInWorkspace is not null && ImageEditToggle.IsChecked.Value)
            {
                Workspace.Opacity = 0.3;

                ZoomSlider.Value = 550 * GetScalingFactor();

                SelectedPhotoElementInEditingContext.Height = double.NaN;
                SelectedPhotoElementInEditingContext.Width = double.NaN;

                SelectedPhotoElementInWorkspace.Clone(SelectedPhotoElementInEditingContext);
                SelectedPhotoElementInEditingContext.Opacity = 1;

                SelectedPicture.Visibility = Visibility.Collapsed;

                if (ImageSettingsToggle.IsChecked.Value)
                    ImageSettingsToggle.IsChecked = false;
            }
        }

        private void UnsetPhotoElementEditingContext()
        {
            Workspace.Opacity = 1;
            //ImageGalleryToggleButton.Visibility = Visibility.Visible;
            SelectedPicture.Visibility = Visibility.Visible;
        }

        private void CommitPhotoElementEditingContext()
        {
            SelectedPhotoElementInEditingContext.Clone(SelectedPhotoElementInWorkspace);
            ImageEditToggle.IsChecked = false;
        }

        private async Task<ContentDialogResult> ShowContentDialog(string title, object content, string okButtonText = "Ok", string cancelButtonText = "Cancel")
        {
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = title,
                PrimaryButtonText = okButtonText,
                CloseButtonText = cancelButtonText,
                DefaultButton = ContentDialogButton.Primary,
                Content = content,
            };

            var result = await dialog.ShowAsync();

            return result;
        }

        #endregion

        #region Events

        #region Workspace

        private async void WorkSpaceSizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var content = this.Resources["WorkspaceSizePanel"] as WrapPanel;

                var result = await ShowContentDialog("Set workspace size...", content);
            }
            catch (Exception)
            {
                throw;
            }
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

        private void FullscreenToggle_Checked(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();

            if (view is not null)
            {
                view.TryEnterFullScreenMode();
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

        private async void WorkSpaceClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await ShowContentDialog(title: "Clear workspace...", content: "Current workspace will be cleared.");  //await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Workspace.Children.Clear();
            }
        }

        #endregion

        #region Pointer

        #region Workspace

        private void Workspace_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            currentPointerPoint = e.GetCurrentPoint(Workspace);
            currentPointer = e.Pointer;

            // if image drawer is open then insert new new item
            if (ImageGalleryToggleButton.IsChecked.Value && (selectedPhotoInGallery is not null || (selectedPhotosInGallery is not null && selectedPhotosInGallery.Any())))
            {
                switch (ImageGallery.SelectionMode)
                {
                    case ListViewSelectionMode.Single:
                        {
                            if (selectedPhotoInGallery is not null)
                            {
                                AddPhotoElementToWorkspace(selectedPhotoInGallery);

                                ImageGallery.SelectedItem = null;
                                selectedPhotoInGallery = null;
                            }
                        }
                        break;
                    case ListViewSelectionMode.Multiple:
                        {
                            if (selectedPhotosInGallery is not null && selectedPhotosInGallery.Any())
                            {
                                //foreach (var photo in selectedPhotosInGallery)
                                //{
                                //    AddPhotoElementToWorkspace(photo);
                                //}

                                if (Parallel.ForEach(selectedPhotosInGallery, (photo) =>
                                {
                                    AddPhotoElementToWorkspace(photo);
                                }).IsCompleted)
                                {
                                    ImageGallery.SelectedItems.Clear();
                                    selectedPhotosInGallery = null;

                                    if (SelectAllToggleButton.IsChecked.Value)
                                        SelectAllToggleButton.IsChecked = false;
                                }
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

        #endregion

        #region Photo Element

        private void PhotoElement_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // if image gallery is open or a image is being edited then do not start dragging
            if (!ImageGalleryToggleButton.IsChecked.Value && (selectedPhotoInGallery is null || selectedPhotosInGallery is null || !selectedPhotosInGallery.Any()) && !ImageEditToggle.IsChecked.Value)
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

        #endregion 

        #endregion

        #region Filters

        #region Sliders

        private void GrayScaleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInEditingContext.ImageGrayscale = e.NewValue;
        }

        private void ContrastSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInEditingContext.ImageContrast = e.NewValue;
        }

        private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInEditingContext.ImageBrightness = e.NewValue;
        }

        private void SaturationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInEditingContext.ImageSaturation = e.NewValue;
        }

        private void SepiaSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInEditingContext.ImageSepia = e.NewValue;
        }

        private void InvertSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInEditingContext.ImageInvert = e.NewValue;
        }

        private void HueRotateSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInEditingContext.ImageHue = e.NewValue;
        }

        private void BlurSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInEditingContext.ImageBlur = e.NewValue;
        }

        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.ImageOpacity = e.NewValue;
        }

        private void SizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Width = e.NewValue;
            SelectedPhotoElementInWorkspace.Height = e.NewValue;
        }

        private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //SelectedPhotoElementInEditingContext.ImageScaleX = e.NewValue;
            //SelectedPhotoElementInEditingContext.ImageScaleY = e.NewValue;
            SelectedPhotoElementInEditingContext.Height = e.NewValue;
            SelectedPhotoElementInEditingContext.Width = e.NewValue;
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
            ZoomToggleButton.IsChecked = false;
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
            ZoomToggleButton.IsChecked = false;
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
            ZoomToggleButton.IsChecked = false;
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
            ZoomToggleButton.IsChecked = false;
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
            ZoomToggleButton.IsChecked = false;
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
            ZoomToggleButton.IsChecked = false;
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
            ZoomToggleButton.IsChecked = false;
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
            ZoomToggleButton.IsChecked = false;
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
            ZoomToggleButton.IsChecked = false;
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
            ZoomToggleButton.IsChecked = false;
        }

        private void ZoomToggleButton_Checked(object sender, RoutedEventArgs e)
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

        #endregion

        #endregion

        #region Gallery

        private void ImageGalleryToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ImageToolsDrawer.Visibility = Visibility.Collapsed;
        }

        private async void ImageImportButton_Click(object sender, RoutedEventArgs e)
        {
            bool isInFullScreen = App.IsFullScreenToggled;

            if (isInFullScreen)
                App.EnterFullScreen(false);

            App.SetIsBusy(true, "Importing files...");

            var fileOpenPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".jpeg");
            fileOpenPicker.FileTypeFilter.Add(".bmp");
            fileOpenPicker.FileTypeFilter.Add(".webp");
            fileOpenPicker.FileTypeFilter.Add(".gif");
            fileOpenPicker.FileTypeFilter.Add(".arw");

            var pickedFiles = await fileOpenPicker.PickMultipleFilesAsync();

            if (pickedFiles.Count > 0)
            {
                App.SetIsBusy(true, "Processing files...");

                // At least one file was picked, we can use them
                foreach (var file in pickedFiles)
                {
                    if (!Photos.Any(x => x.Name == file.Name))
                    {
                        var data = await GetImageData(file);

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(data.MemoryStream);

                        Photo photo = new Photo()
                        {
                            Name = file.Name,
                            DataUrl = data.DataUrl,
                            Source = bitmapImage
                        };

                        Photos.Add(photo);
                    }
                }
            }
            else
            {
                // No file was picked or the dialog was cancelled.
            }

            ImageGallery.ItemsSource = null;
            ImageGallery.ItemsSource = Photos;

            if (isInFullScreen)
                App.EnterFullScreen(true);

            App.SetIsBusy(false);
        }

        private async Task<(string DataUrl, MemoryStream MemoryStream)> GetImageData(StorageFile file)
        {
            var stream = await file.OpenStreamForReadAsync();
            stream.Seek(0, SeekOrigin.Begin);

            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            ms.Seek(0, SeekOrigin.Begin);
            var base64String = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

            return (base64String, ms);
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

        private void SelectMultipleToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ImageGallery.SelectionMode = ListViewSelectionMode.Multiple;
        }

        private void SelectMultipleToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SelectAllToggleButton.IsChecked.Value)
            {
                SelectAllToggleButton.IsChecked = false;
            }

            ImageGallery.SelectionMode = ListViewSelectionMode.Single;
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

        #endregion

        #region Image

        private void ImageEditToggle_Checked(object sender, RoutedEventArgs e)
        {
            SetPhotoElementEditingContext();
        }

        private void ImageEditToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            UnsetPhotoElementEditingContext();
        }

        private void ImageCommitButton_Click(object sender, RoutedEventArgs e)
        {
            CommitPhotoElementEditingContext();
        }

        private void ImageCopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPhotoElementInWorkspace is not null)
            {
                AddPhotoElementToWorkspace(Photos.FirstOrDefault(x => x.Id == SelectedPhotoElementInWorkspace.Id));
            }
        }

        private void ImageUndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPhotoElementInEditingContext is not null && !SelectedPhotoElementInEditingContext.Source.IsNullOrBlank())
                SelectedPhotoElementInEditingContext.Reset();
        }

        private void ImageExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedPhotoElementInEditingContext is not null)
                    SelectedPhotoElementInEditingContext.Export();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ImageDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPhotoElementInWorkspace is not null)
            {
                //var content = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Left };
                //content.Children.Add(new TextBlock()
                //{
                //    Text = this.Photos.FirstOrDefault(x => x.Id == SelectedPhotoElementInWorkspace.Id)?.Name,
                //    TextAlignment = TextAlignment.Left,
                //    TextTrimming = TextTrimming.CharacterEllipsis,
                //    FontWeight = FontWeights.SemiBold,
                //    MaxWidth = 300,
                //});
                //content.Children.Add(new TextBlock()
                //{
                //    Text = "will be removed from current workspace.",
                //});

                //var result = await ShowContentDialog(title: "Remove image...", content: content); //await dialog.ShowAsync();

                //if (result == ContentDialogResult.Primary)
                //{
                Workspace.Children.Remove(SelectedPhotoElementInWorkspace);
                SelectedPhotoElementInWorkspace = null;
                //}
            }
        }

        private void ImageSettingsToggle_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ImageSettingsToggle_Unchecked(object sender, RoutedEventArgs e)
        {

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

        #endregion

        #endregion
    }
}
