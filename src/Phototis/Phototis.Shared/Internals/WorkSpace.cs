using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Phototis
{
    public class WorkSpace : Canvas
    {
        //private CompositeTransform compositeTransform;

        public WorkSpace()
        {
            //this.compositeTransform = new CompositeTransform() { ScaleX = 1, ScaleY = 1, };
            ////this.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            //this.RenderTransform = compositeTransform;
        }

        public void SetSize(double height, double width)
        {
            this.Height = height;
            this.Width = width;
        }

        //public void SetZoom(double value)
        //{
        //    compositeTransform.ScaleX = value;
        //    compositeTransform.ScaleY = value;
        //}
    }
}
