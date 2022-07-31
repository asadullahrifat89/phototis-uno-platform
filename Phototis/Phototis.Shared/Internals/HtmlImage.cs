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
                //var js = $"element.src=\"{encodedSource}\"; element.style=\"object-fit: contain;\"";
                //image.ExecuteJavascript(js);
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
                //var js = $"element.height=\"{args.NewValue}\";";
                //image.ExecuteJavascript(js);
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
                //var js = $"element.width=\"{args.NewValue}\";";
                //image.ExecuteJavascript(js);
                image.SetHtmlAttribute("width", $"{args.NewValue}");
            }
        }

        public void SetGrayScale(double value)
        {
            //var js = $"element.style.filter=\"grayscale({value}%)\";";
            //this.ExecuteJavascript(js);
            this.SetCssStyle("filter", $"grayscale({value}%)");
        }

        public void SetContrast(double value)
        {
            //var js = $"element.style.filter=\"contrast({value}%)\";";
            //this.ExecuteJavascript(js);
            this.SetCssStyle("filter", $"contrast({value}%)");
        }

        public void SetBrightness(double value)
        {
            //var js = $"element.style.filter=\"brightness({value}%)\";";
            //this.ExecuteJavascript(js);
            this.SetCssStyle("filter", $"brightness({value}%)");
        }

        public void SetSaturation(double value)
        {
            //var js = $"element.style.filter=\"saturate({value}%)\";";
            //this.ExecuteJavascript(js);
            this.SetCssStyle("filter", $"saturate({value}%)");
        }

        public void SetSepia(double value)
        {
            //var js = $"element.style.filter=\"sepia({value}%)\";";
            //this.ExecuteJavascript(js);
            this.SetCssStyle("filter", $"sepia({value}%)");
        }
    }
}
