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
    public class PhotoElement : Border
    {
        public HtmlImageElement HtmlImageElement { get; set; }

        public PhotoElement()
        {
           
        }

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
                if (image.HtmlImageElement is null)
                {
                    image.HtmlImageElement = new HtmlImageElement() { Source = args.NewValue as string };
                    image.Child = image.HtmlImageElement;
                }
                else
                {
                    image.HtmlImageElement.Source = args.NewValue as string;
                    image.Child = image.HtmlImageElement;
                }
            }
        }
    }
}
