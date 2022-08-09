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

        private double windowWidth, windowHeight;

        private ImageFile selectedPhotoInGallery;

        private List<ImageFile> selectedPhotosInGallery;

        bool _isPointerCaptured;
        double _pointerX;
        double _pointerY;
        double _objectLeft;
        double _objectTop;

        private PhotoElement draggingElement;

        private PointerPoint currentPointerPoint;
        private Pointer currentPointer;

        private readonly IDictionary<string, PhotoElement> photoElementsCache = new Dictionary<string, PhotoElement>();
        private readonly IDictionary<string, PersonPicture> personPicturesCache = new Dictionary<string, PersonPicture>();

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

            SetIsBusy(false);
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

        private List<ImageFile> imageFiles = new List<ImageFile>();
        public List<ImageFile> ImageFiles
        {
            get { return imageFiles; }
            set
            {
                imageFiles = value;
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
                    PersonPicture personPicture;

                    // cache images to improve performance
                    if (personPicturesCache.ContainsKey(selectedPhotoElementInWorkspace.Id))
                    {
                        personPicture = personPicturesCache[selectedPhotoElementInWorkspace.Id];
                    }
                    else
                    {
                        // set image source for the selected image                    
                        var imageFile = ImageFiles.FirstOrDefault(x => x.Id == selectedPhotoElementInWorkspace.Id);

                        personPicture = new PersonPicture();
                        personPicture.ProfilePicture = imageFile.Source;
                        personPicturesCache.Add(selectedPhotoElementInWorkspace.Id, personPicture);
                    }

                    SelectedPicture.Child = personPicture;

                    // if image galler is not open then show editing tools
                    if (!ImageGalleryToggleButton.IsChecked.Value)
                    {
                        SelectedPicture.Visibility = Visibility.Visible;
                        ImageToolsDrawer.Visibility = Visibility.Visible;
                    }
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

        private void AddPhotoElementToWorkspace(ImageFile imageFile)
        {
            var scalingFactor = GetScalingFactor();

            PhotoElement photoElement = new PhotoElement()
            {
                Id = imageFile.Id,
                Width = 400 * scalingFactor,
                Height = 400 * scalingFactor,
                Extenstion = imageFile.Extension,
            };
            photoElement.Source = imageFile.DataUrl;

            var lastElement = Workspace.Children.OfType<PhotoElement>().LastOrDefault();
            double zIndex = lastElement is not null ? Canvas.GetZIndex(lastElement) : 0;

            Canvas.SetLeft(photoElement, currentPointerPoint.Position.X - 200 * scalingFactor);
            Canvas.SetTop(photoElement, currentPointerPoint.Position.Y - 200 * scalingFactor);
            Canvas.SetZIndex(photoElement, zIndex++);

            photoElement.PointerPressed += PhotoElement_PointerPressed;
            photoElement.PointerReleased += PhotoElement_PointerReleased;

            Workspace.Children.Add(photoElement);
        }

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

        public void SetIsBusy(bool isBusy, string message = null)
        {
            BusyIndicatorText.Text = isBusy ? message : null;
            App.SetIsBusy(isBusy);
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
                SelectedPhotoElementInWorkspace = null;
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
                                AddPhotoElementToWorkspace(selectedPhotoInGallery); // single drop

                                ImageGallery.SelectedItem = null;
                                selectedPhotoInGallery = null;
                            }
                        }
                        break;
                    case ListViewSelectionMode.Multiple:
                        {
                            if (selectedPhotosInGallery is not null && selectedPhotosInGallery.Any())
                            {
                                if (Parallel.ForEach(selectedPhotosInGallery, (imageFile) =>
                                {
                                    AddPhotoElementToWorkspace(imageFile); // multiple drop

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
            if (/*!ImageGalleryToggleButton.IsChecked.Value && (selectedPhotoInGallery is null || selectedPhotosInGallery is null || !selectedPhotosInGallery.Any()) && */!ImageEditToggle.IsChecked.Value)
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
            //if (!ImageGalleryToggleButton.IsChecked.Value && (selectedPhotoInGallery is null || selectedPhotosInGallery is null || !selectedPhotosInGallery.Any()))
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

        #region Edit

        #region Toggles

        #region Edit

        private void GrayscaleToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, EditToolsToggleButtonsPanel);
        }

        private void ContrastToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, EditToolsToggleButtonsPanel);
        }

        private void BrightnessToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, EditToolsToggleButtonsPanel);
        }

        private void SaturationToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, EditToolsToggleButtonsPanel);
        }

        private void SepiaToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, EditToolsToggleButtonsPanel);
        }

        private void InvertToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, EditToolsToggleButtonsPanel);
        }

        private void HueToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, EditToolsToggleButtonsPanel);
        }

        private void BlurToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, EditToolsToggleButtonsPanel);
        }

        private void ZoomToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, EditToolsToggleButtonsPanel);
        }

        private void RotateToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, EditToolsToggleButtonsPanel);
        }

        private void FlipHToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageScaleX = -1;
        }

        private void FlipVToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageScaleY = -1;
        }

        private void FlipHToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageScaleX = 1;
        }

        private void FlipVToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageScaleY = 1;
        }

        #endregion

        #region Settings

        private void SizeToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, SettingsToolsToggleButtonsPanel);
        }

        private void OpacityToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UnCheckAllToggleButtonsExcept(sender as ToggleButton, SettingsToolsToggleButtonsPanel);
        }

        #endregion

        private void UnCheckAllToggleButtonsExcept(ToggleButton senderToggleButton, StackPanel buttonsPanel)
        {
            foreach (ToggleButton toggleButton in buttonsPanel.Children.OfType<ToggleButton>().Where(x => x.Name != senderToggleButton.Name))
            {
                toggleButton.IsChecked = false;
            }
        }

        #endregion

        #region Sliders

        #region Edit

        private void GrayScaleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageGrayscale = e.NewValue;
        }

        private void ContrastSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageContrast = e.NewValue;
        }

        private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageBrightness = e.NewValue;
        }

        private void SaturationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageSaturation = e.NewValue;
        }

        private void SepiaSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageSepia = e.NewValue;
        }

        private void InvertSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageInvert = e.NewValue;
        }

        private void HueRotateSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageHue = e.NewValue;
        }

        private void BlurSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageBlur = e.NewValue;
        }

        private void RotateSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).ImageRotation = e.NewValue;
        }

        private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInEditingContext.Height = e.NewValue;
            SelectedPhotoElementInEditingContext.Width = e.NewValue;
        }

        #endregion

        #region Settings

        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.ImageOpacity = e.NewValue;
        }

        private void SizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedPhotoElementInWorkspace.Width = e.NewValue;
            SelectedPhotoElementInWorkspace.Height = e.NewValue;
        }

        #endregion

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

            SetIsBusy(true, "Importing files...");

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

            var pickedFiles = await fileOpenPicker.PickMultipleFilesAsync();

            if (pickedFiles.Count > 0)
            {
                SetIsBusy(true, "Processing files...");

                // At least one file was picked, we can use them
                foreach (var file in pickedFiles)
                {
                    if (!ImageFiles.Any(x => x.Name == file.Name))
                    {
                        var data = await GetImageData(file);

                        var bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(data.MemoryStream);

                        ImageFile imageFile = new ImageFile()
                        {
                            Name = file.Name,
                            DataUrl = data.DataUrl,
                            Source = bitmapImage,
                            Extension = file.Name.Split('.').LastOrDefault()
                        };

                        ImageFiles.Add(imageFile);
                    }
                }
            }
            else
            {
                // No file was picked or the dialog was cancelled.
            }

            ImageGallery.ItemsSource = null;
            ImageGallery.ItemsSource = ImageFiles;

            if (isInFullScreen)
                App.EnterFullScreen(true);

            SetIsBusy(false);
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
                    selectedPhotoInGallery = ImageGallery.SelectedItem as ImageFile;
                    break;
                case ListViewSelectionMode.Multiple:
                    selectedPhotosInGallery = ImageGallery.SelectedItems.OfType<ImageFile>().ToList();
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
            if (SelectedPhotoElementInWorkspace is not null && ImageEditToggle.IsChecked.Value)
            {
                Workspace.Opacity = 0.3;

                // set height and width for the image container
                SelectedPhotoElementInEditingContext.Height = windowHeight - 270;
                SelectedPhotoElementInEditingContext.Width = windowWidth - 100;

                PhotoElement photoElement;

                // cache the images for performance improvement
                if (photoElementsCache.ContainsKey(SelectedPhotoElementInWorkspace.Id))
                {
                    photoElement = photoElementsCache[SelectedPhotoElementInWorkspace.Id];
                }
                else
                {
                    photoElement = new PhotoElement() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    SelectedPhotoElementInWorkspace.Clone(photoElement);
                    photoElementsCache.Add(photoElement.Id, photoElement);
                }

                // set the photo element in editing context
                SelectedPhotoElementInEditingContext.Opacity = 1;
                SelectedPhotoElementInEditingContext.Child = photoElement;

                // set the slider values
                GrayScaleSlider.Value = SelectedPhotoElementInWorkspace.ImageGrayscale;
                ContrastSlider.Value = SelectedPhotoElementInWorkspace.ImageContrast;
                BrightnessSlider.Value = SelectedPhotoElementInWorkspace.ImageBrightness;
                SaturationSlider.Value = SelectedPhotoElementInWorkspace.ImageSaturation;
                SepiaSlider.Value = SelectedPhotoElementInWorkspace.ImageSepia;
                InvertSlider.Value = SelectedPhotoElementInWorkspace.ImageInvert;
                HueRotateSlider.Value = SelectedPhotoElementInWorkspace.ImageHue;
                BlurSlider.Value = SelectedPhotoElementInWorkspace.ImageBlur;
                SizeSlider.Value = SelectedPhotoElementInWorkspace.Width;
                OpacitySlider.Value = SelectedPhotoElementInWorkspace.ImageOpacity;
                RotateSlider.Value = SelectedPhotoElementInWorkspace.ImageRotation;

                FlipHToggleButton.IsChecked = SelectedPhotoElementInWorkspace.ImageScaleX < 0;
                FlipVToggleButton.IsChecked = SelectedPhotoElementInWorkspace.ImageScaleY < 0;

                // Hide the cirle picture
                SelectedPicture.Visibility = Visibility.Collapsed;

                // hide the image settings
                if (ImageSettingsToggle.IsChecked.Value)
                    ImageSettingsToggle.IsChecked = false;
            }
        }

        private void ImageEditToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Workspace.Opacity = 1;
            SelectedPicture.Visibility = Visibility.Visible;
        }

        private void ImageUndoButton_Click(object sender, RoutedEventArgs e)
        {
            GrayScaleSlider.Value = 0;
            ContrastSlider.Value = 100;
            BrightnessSlider.Value = 100;
            SaturationSlider.Value = 100;
            SepiaSlider.Value = 0;
            InvertSlider.Value = 0;
            HueRotateSlider.Value = 0;
            BlurSlider.Value = 0;
            RotateSlider.Value = 0;

            OpacitySlider.Value = 1;

            FlipHToggleButton.IsChecked = false;
            FlipVToggleButton.IsChecked = false;

            ZoomSlider.Value = 550 * GetScalingFactor();

            // set height and width for the image container
            SelectedPhotoElementInEditingContext.Height = windowHeight - 270;
            SelectedPhotoElementInEditingContext.Width = windowWidth - 100;
        }

        private void ImageCommitButton_Click(object sender, RoutedEventArgs e)
        {
            (SelectedPhotoElementInEditingContext.Child as PhotoElement).Clone(SelectedPhotoElementInWorkspace);
            ImageEditToggle.IsChecked = false;
        }

        private void ImageCopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPhotoElementInWorkspace is not null)
            {
                AddPhotoElementToWorkspace(ImageFiles.FirstOrDefault(x => x.Id == SelectedPhotoElementInWorkspace.Id)); // copy
            }
        }

        private void ImageExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedPhotoElementInEditingContext is not null)
                    (SelectedPhotoElementInEditingContext.Child as PhotoElement).Export();
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

                Workspace.Children.Remove(SelectedPhotoElementInWorkspace);
                SelectedPhotoElementInWorkspace = null;
            }
        }

        private void ImageSettingsToggle_Checked(object sender, RoutedEventArgs e)
        {
            //SettingsTools.Visibility = Visibility.Visible;
        }

        private void ImageSettingsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            //SettingsTools.Visibility = Visibility.Collapsed;
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
