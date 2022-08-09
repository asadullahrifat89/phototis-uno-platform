using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Phototis
{
    public class WorkSpace : Canvas
    {
        public WorkSpace()
        {
           
        }

        public void SetSize(double height, double width)
        {
            Height = height;
            Width = width;
        }
    }
}
