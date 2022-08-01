using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Uno.Foundation;
using Uno.UI.Runtime.WebAssembly;

namespace Phototis
{
    [HtmlElement("img")]
    public sealed class HtmlImageElement : FrameworkElement
    {
        #region Ctor

        public HtmlImageElement()
        {

        } 

        #endregion

        #region Properties

        private double _Grayscale = 0;

        public double Grayscale
        {
            get { return _Grayscale; }
            set
            {
                _Grayscale = value;
                SetFilter();
            }
        }

        private double _Contrast = 100;

        public double Contrast
        {
            get { return _Contrast; }
            set
            {
                _Contrast = value;
                SetFilter();
            }
        }

        private double _Brightness = 100;

        public double Brightness
        {
            get { return _Brightness; }
            set
            {
                _Brightness = value;
                SetFilter();
            }
        }

        private double _Saturation = 100;

        public double Saturation
        {
            get { return _Saturation; }
            set
            {
                _Saturation = value;
                SetFilter();
            }
        }

        private double _Sepia = 0;

        public double Sepia
        {
            get { return _Sepia; }
            set
            {
                _Sepia = value;
                SetFilter();
            }
        }

        private double _Invert = 0;

        public double Invert
        {
            get { return _Invert; }
            set
            {
                _Invert = value;
                SetFilter();
            }
        }

        private double _Hue = 0;

        public double Hue
        {
            get { return _Hue; }
            set
            {
                _Hue = value;
                SetFilter();
            }
        }

        private double _Blur = 0;

        public double Blur
        {
            get { return _Blur; }
            set
            {
                _Blur = value;
                SetFilter();
            }
        }

        private double _Opacity = 1;

        public new double Opacity
        {
            get { return _Opacity; }
            set
            {
                _Opacity = value;
                SetFilter();
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(HtmlImageElement), new PropertyMetadata(default(string), OnSourceChanged));
        //public static readonly new DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(string), typeof(HtmlImage), new PropertyMetadata(default(string), OnHeightChanged));
        //public static readonly new DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(string), typeof(HtmlImage), new PropertyMetadata(default(string), OnWidthChanged));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        //public new string Height
        //{
        //    get => (string)GetValue(HeightProperty);
        //    set => SetValue(HeightProperty, value);
        //}

        //public new string Width
        //{
        //    get => (string)GetValue(WidthProperty);
        //    set => SetValue(WidthProperty, value);
        //}

        private static void OnSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is HtmlImageElement image)
            {
                var encodedSource = WebAssemblyRuntime.EscapeJs("" + args.NewValue);
                image.SetHtmlAttribute("src", encodedSource);
                image.SetHtmlAttribute("draggable", "false");
                image.SetCssStyle("object-fit", "contain");
                image.SetFilter();
            }
        }

        //private static void OnHeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        //{
        //    if (dependencyObject is HtmlImage image)
        //    {
        //        image.SetHtmlAttribute("height", $"{args.NewValue}");
        //    }
        //}

        //private static void OnWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        //{
        //    if (dependencyObject is HtmlImage image)
        //    {
        //        image.SetHtmlAttribute("width", $"{args.NewValue}");
        //    }
        //}

        //public void SetGrayScale(double value)
        //{
        //    grayscale = value;
        //    SetFilter();
        //}

        //public void SetContrast(double value)
        //{
        //    contrast = value;
        //    SetFilter();
        //}

        //public void SetBrightness(double value)
        //{
        //    brightness = value;
        //    SetFilter();
        //}

        //public void SetSaturation(double value)
        //{
        //    saturation = value;
        //    SetFilter();
        //}

        //public void SetSepia(double value)
        //{
        //    sepia = value;
        //    SetFilter();
        //}

        //public void SetInvert(double value)
        //{
        //    invert = value;
        //    SetFilter();
        //}

        //public void SetHueRotate(double value)
        //{
        //    hue_rotate = value;
        //    SetFilter();
        //}

        //public void SetBlur(double value)
        //{
        //    blur = value;
        //    SetFilter();
        //} 

        #endregion

        #region Methods

        public void SetDefaults()
        {
            _Grayscale = 0;
            _Contrast = 100;
            _Brightness = 100;
            _Saturation = 100;
            _Sepia = 0;
            _Invert = 0;
            _Hue = 0;
            _Blur = 0;
            _Opacity = 1;

            SetFilter();
        }

        public void SetFilter()
        {
            this.SetCssStyle("filter", $"grayscale({_Grayscale}%) contrast({_Contrast}%) brightness({_Brightness}%) saturate({_Saturation}%) sepia({_Sepia}%) invert({_Invert}%) hue-rotate({_Hue}deg) blur({_Blur}px) drop-shadow(0 0 0.75rem crimson)");
            this.SetCssStyle("opacity", $"{_Opacity}");
        } 

        #endregion
    }
}
