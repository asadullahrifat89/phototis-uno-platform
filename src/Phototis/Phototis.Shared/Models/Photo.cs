using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.Storage;

namespace Phototis
{
    public class Photo
    {
        public Photo()
        {
            Id = Guid.NewGuid().ToString().Split('-')[0];
        }

        /// <summary>
        /// Unique id of the photo.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// File name of the photo.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Source for HTML img tag.
        /// </summary>
        public string DataUrl { get; set; }

        /// <summary>
        /// Source for XAML Image control.
        /// </summary>
        public BitmapImage Source { get; set; }
    }
}
