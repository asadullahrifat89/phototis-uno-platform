using Microsoft.UI.Xaml.Controls;

namespace Phototis
{
    public class PhotoElement : Border
    {
        public PhotoElement(string dataUrl)
        {
            Child = new HtmlImageElement() { Source = dataUrl };
        }
    }
}
