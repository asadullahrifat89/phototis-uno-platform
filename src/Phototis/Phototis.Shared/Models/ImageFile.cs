using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.Storage;

namespace Phototis
{
    public class ImageFile
    {
        public ImageFile()
        {
            Id = Guid.NewGuid().ToString().Split('-')[0];
        }

        /// <summary>
        /// Unique id of the image.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// File name of image.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Source for HTML img tag.
        /// </summary>
        public string DataUrl { get; set; }

        /// <summary>
        /// Extension of the image.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Source for XAML Image control.
        /// </summary>
        public BitmapImage Source { get; set; }
    }
}
