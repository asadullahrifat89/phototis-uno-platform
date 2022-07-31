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
    public sealed class HtmlImage : Border
    {
        private double grayscale = 0;
        private double contrast = 100;
        private double brightness = 100;
        private double saturation = 100;
        private double sepia = 0;

        public HtmlImage()
        {
            //Background = new SolidColorBrush(Colors.Transparent);
            BorderThickness = new Thickness(10);
        }

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
           "Source", typeof(string), typeof(HtmlImage), new PropertyMetadata(default(string), OnSourceChanged));

        private static void OnSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is HtmlImage image)
            {
                var encodedSource = WebAssemblyRuntime.EscapeJs("" + args.NewValue);                
                image.SetHtmlAttribute("src", encodedSource);
                image.SetCssStyle("object-fit", "contain");
            }
        }

        public new string Height
        {
            get => (string)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public static readonly new DependencyProperty HeightProperty = DependencyProperty.Register(
          "Height", typeof(string), typeof(HtmlImage), new PropertyMetadata(default(string), OnHeightChanged));

        private static void OnHeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is HtmlImage image)
            {
                image.SetHtmlAttribute("height", $"{args.NewValue}");
            }
        }

        public new string Width
        {
            get => (string)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static readonly new DependencyProperty WidthProperty = DependencyProperty.Register(
         "Width", typeof(string), typeof(HtmlImage), new PropertyMetadata(default(string), OnWidthChanged));

        private static void OnWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is HtmlImage image)
            {                
                image.SetHtmlAttribute("width", $"{args.NewValue}");
            }
        }

        public void SetGrayScale(double value)
        {
            grayscale = value;
            SetFilter();
        }      

        public void SetContrast(double value)
        {
            contrast = value;
            SetFilter();
        }

        public void SetBrightness(double value)
        {
            brightness = value;
            SetFilter();
        }

        public void SetSaturation(double value)
        {
            saturation = value;
            SetFilter();
        }

        public void SetSepia(double value)
        {
            sepia = value;
            SetFilter();
        }

        private void SetFilter()
        {
            this.SetCssStyle("filter", $"grayscale({grayscale}%) contrast({contrast}%) brightness({brightness}%) saturate({saturation}%) sepia({sepia}%)");
        }
    }
}
