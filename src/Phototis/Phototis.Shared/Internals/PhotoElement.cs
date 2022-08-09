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

        private ImgElement ImgElement;

        #endregion

        #region Ctor

        public PhotoElement()
        {
            RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            ImgElement = new ImgElement();
            Child = ImgElement;
        }

        #endregion

        #region Properties

        private string id;

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                if (ImgElement is not null)
                    ImgElement.Id = id;
            }
        }


        private double grayscale = 0;

        public double ImageGrayscale
        {
            get { return grayscale; }
            set
            {
                grayscale = value;

                if (ImgElement is not null)
                    ImgElement.Grayscale = grayscale;
            }
        }

        private double contrast = 100;

        public double ImageContrast
        {
            get { return contrast; }
            set
            {
                contrast = value;

                if (ImgElement is not null)
                    ImgElement.Contrast = contrast;
            }
        }

        private double brightness = 100;

        public double ImageBrightness
        {
            get { return brightness; }
            set
            {
                brightness = value;

                if (ImgElement is not null)
                    ImgElement.Brightness = brightness;
            }
        }

        private double saturation = 100;

        public double ImageSaturation
        {
            get { return saturation; }
            set
            {
                saturation = value;

                if (ImgElement is not null)
                    ImgElement.Saturation = saturation;
            }
        }

        private double sepia = 0;

        public double ImageSepia
        {
            get { return sepia; }
            set
            {
                sepia = value;

                if (ImgElement is not null)
                    ImgElement.Sepia = sepia;
            }
        }

        private double invert = 0;

        public double ImageInvert
        {
            get { return invert; }
            set
            {
                invert = value;

                if (ImgElement is not null)
                    ImgElement.Invert = invert;
            }
        }

        private double hue = 0;

        public double ImageHue
        {
            get { return hue; }
            set
            {
                hue = value;

                if (ImgElement is not null)
                    ImgElement.Hue = hue;
            }
        }

        private double blur = 0;

        public double ImageBlur
        {
            get { return blur; }
            set
            {
                blur = value;

                if (ImgElement is not null)
                    ImgElement.Blur = blur;
            }
        }

        private double opacity = 1;

        public double ImageOpacity
        {
            get { return opacity; }
            set
            {
                opacity = value;

                if (ImgElement is not null)
                    ImgElement.Opacity = opacity;
            }
        }

        private double rotation = 0;

        public double ImageRotation
        {
            get { return rotation; }
            set
            {
                rotation = value;

                if (ImgElement is not null)
                    ImgElement.Rotation = rotation;
            }
        }

        private double scaleX = 1;

        public double ImageScaleX
        {
            get { return scaleX; }
            set
            {
                scaleX = value;

                if (ImgElement is not null)
                    ImgElement.ScaleX = scaleX;
            }
        }

        private double scaleY = 1;

        public double ImageScaleY
        {
            get { return scaleY; }
            set
            {
                scaleY = value;

                if (ImgElement is not null)
                    ImgElement.ScaleY = scaleY;
            }
        }

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
                if (image.ImgElement is not null)
                {
                    image.ImgElement.Id = image.Id;
                    image.ImgElement.Source = image.Source;
                }
            }
        }

        public string Extenstion { get; set; }

        #endregion

        #region Methods

        public void Export()
        {
            var id = Id;
            var src = Source;
            var filter = ImgElement.GetCssFilter();
            var rotation = ImgElement.Rotation;
            var extension = Extenstion;

            var function = $"exportImage('{id}','{filter}',{rotation},'{src}','{extension}')";

            WebAssemblyRuntime.InvokeJS(function);
        }

        #endregion
    }
}
