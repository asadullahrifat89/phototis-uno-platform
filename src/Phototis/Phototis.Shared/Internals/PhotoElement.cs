using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Uno.Extensions;
using Uno.Foundation;
using Uno.UI.Runtime.WebAssembly;

namespace Phototis
{
    public sealed class PhotoElement : Border
    {
        #region Fields

        private ImageElement htmlImageElement;

        private CompositeTransform compositeTransform;

        #endregion

        #region Ctor

        public PhotoElement()
        {
            RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            compositeTransform = new CompositeTransform() { ScaleX = 1, ScaleY = 1 };
            RenderTransform = compositeTransform;
            htmlImageElement = new ImageElement();
            Child = htmlImageElement;
        }

        #endregion

        #region Properties

        public string Id { get; set; }


        private double scaleX;

        public double ImageScaleX
        {
            get { return scaleX; }
            set
            {
                scaleX = value;
                compositeTransform.ScaleX = scaleX;
            }
        }

        private double scaleY;

        public double ImageScaleY
        {
            get { return scaleY; }
            set
            {
                scaleY = value;
                compositeTransform.ScaleY = scaleY;
            }
        }

        private double grayscale = 0;

        public double ImageGrayscale
        {
            get { return grayscale; }
            set
            {
                grayscale = value;

                if (htmlImageElement is not null)
                    htmlImageElement.Grayscale = grayscale;
            }
        }

        private double contrast = 100;

        public double ImageContrast
        {
            get { return contrast; }
            set
            {
                contrast = value;

                if (htmlImageElement is not null)
                    htmlImageElement.Contrast = contrast;
            }
        }

        private double brightness = 100;

        public double ImageBrightness
        {
            get { return brightness; }
            set
            {
                brightness = value;

                if (htmlImageElement is not null)
                    htmlImageElement.Brightness = brightness;
            }
        }

        private double saturation = 100;

        public double ImageSaturation
        {
            get { return saturation; }
            set
            {
                saturation = value;

                if (htmlImageElement is not null)
                    htmlImageElement.Saturation = saturation;
            }
        }

        private double sepia = 0;

        public double ImageSepia
        {
            get { return sepia; }
            set
            {
                sepia = value;

                if (htmlImageElement is not null)
                    htmlImageElement.Sepia = sepia;
            }
        }

        private double invert = 0;

        public double ImageInvert
        {
            get { return invert; }
            set
            {
                invert = value;

                if (htmlImageElement is not null)
                    htmlImageElement.Invert = invert;
            }
        }

        private double hue = 0;

        public double ImageHue
        {
            get { return hue; }
            set
            {
                hue = value;

                if (htmlImageElement is not null)
                    htmlImageElement.Hue = hue;
            }
        }

        private double blur = 0;

        public double ImageBlur
        {
            get { return blur; }
            set
            {
                blur = value;

                if (htmlImageElement is not null)
                    htmlImageElement.Blur = blur;
            }
        }

        private double opacity = 1;

        public double ImageOpacity
        {
            get { return opacity; }
            set
            {
                opacity = value;

                if (htmlImageElement is not null)
                    htmlImageElement.Opacity = opacity;
            }
        }

        //private string source;

        //public string Source
        //{
        //    get { return source; }
        //    set
        //    {
        //        source = value;

        //        if (htmlImageElement is not null)
        //        {
        //            htmlImageElement.Id = Id;
        //            htmlImageElement.Source = source;
        //        }
        //    }
        //}


        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(PhotoElement), new PropertyMetadata(default(string), OnSourceChanged));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is PhotoElement image)
            {
                if (image.htmlImageElement is not null)
                {
                    image.htmlImageElement.Id = image.Id;
                    image.htmlImageElement.Source = image.Source;
                }
            }
        } 

        #endregion

        #region Methods

        public void Export()
        {
            var id = Id;
            var src = Source;
            var filter = htmlImageElement.GetCssFilter();
            var function = $"exportImage('{id}','{filter}','{src}')";

            WebAssemblyRuntime.InvokeJS(function);
        }

        public void Reset()
        {
            ImageGrayscale = 0;
            ImageContrast = 100;
            ImageBrightness = 100;
            ImageSaturation = 100;
            ImageSepia = 0;
            ImageInvert = 0;
            ImageHue = 0;
            ImageBlur = 0;
            ImageOpacity = 1;
            ImageScaleX = 1;
            ImageScaleY = 1;
        }

        #endregion
    }
}
