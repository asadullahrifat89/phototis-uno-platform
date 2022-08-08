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
    public sealed class ImgElement : FrameworkElement
    {
        #region Ctor

        public ImgElement()
        {
            this.SetHtmlAttribute("draggable", "false");
            this.SetHtmlAttribute("loading", "lazy");
            this.SetCssStyle("object-fit", "contain");            
        }

        #endregion

        #region Properties

        private string _Id;

        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;

                if (!_Id.IsNullOrBlank())
                {
                    this.SetHtmlAttribute("id", _Id);
                }
            }
        }


        private double _Grayscale = 0;

        public double Grayscale
        {
            get { return _Grayscale; }
            set
            {
                _Grayscale = value;
                SetProperties();
            }
        }

        private double _Contrast = 100;

        public double Contrast
        {
            get { return _Contrast; }
            set
            {
                _Contrast = value;
                SetProperties();
            }
        }

        private double _Brightness = 100;

        public double Brightness
        {
            get { return _Brightness; }
            set
            {
                _Brightness = value;
                SetProperties();
            }
        }

        private double _Saturation = 100;

        public double Saturation
        {
            get { return _Saturation; }
            set
            {
                _Saturation = value;
                SetProperties();
            }
        }

        private double _Sepia = 0;

        public double Sepia
        {
            get { return _Sepia; }
            set
            {
                _Sepia = value;
                SetProperties();
            }
        }

        private double _Invert = 0;

        public double Invert
        {
            get { return _Invert; }
            set
            {
                _Invert = value;
                SetProperties();
            }
        }

        private double _Hue = 0;

        public double Hue
        {
            get { return _Hue; }
            set
            {
                _Hue = value;
                SetProperties();
            }
        }

        private double _Blur = 0;

        public double Blur
        {
            get { return _Blur; }
            set
            {
                _Blur = value;
                SetProperties();
            }
        }

        private double _Opacity = 1;

        public new double Opacity
        {
            get { return _Opacity; }
            set
            {
                _Opacity = value;
                SetProperties();
            }
        }

        private double _Rotation = 0;

        public new double Rotation
        {
            get { return _Rotation; }
            set
            {
                _Rotation = value;
                SetProperties();
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(ImgElement), new PropertyMetadata(default(string), OnSourceChanged));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is ImgElement image)
            {
                var encodedSource = WebAssemblyRuntime.EscapeJs("" + args.NewValue);
                image.SetHtmlAttribute("src", encodedSource);
                image.SetProperties();
            }
        }

        #endregion

        #region Methods
        public string GetCssFilter()
        {
            return $"grayscale({_Grayscale}%) contrast({_Contrast}%) brightness({_Brightness}%) saturate({_Saturation}%) sepia({_Sepia}%) invert({_Invert}%) hue-rotate({_Hue}deg) blur({_Blur}px)";
        }
      
        public void SetProperties()
        {
            this.SetCssStyle(("filter", GetCssFilter() + $" drop-shadow(0 0 0.75rem crimson)"),("opacity", $"{_Opacity}"),("transform", $"rotate({_Rotation}deg)"));
        }

        #endregion
    }
}
