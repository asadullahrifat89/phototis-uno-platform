using Microsoft.UI.Xaml.Controls;
using System;

namespace Phototis
{
    public class PhotoElement : Border
    {
        public HtmlImageElement HtmlImageElement;

        public PhotoElement(string dataUrl)
        {
            HtmlImageElement = new HtmlImageElement() { Source = dataUrl };
            Child = HtmlImageElement;
            CanDrag = false;
            AllowDrop = false;
        }
    }
}
