using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.Storage;

namespace Phototis
{
    public class Photo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public string DataUrl { get; set; }

        //public StorageFile StorageFile { get; set; }

        public BitmapImage Source { get; set; }
    }
}
