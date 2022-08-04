using Microsoft.UI;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System;
using System.Globalization;
using Windows.Foundation;
using Windows.UI;

namespace Phototis
{
    public static class Constants
    {
        /// <summary>
        /// Checks if the provided string is null or empty or white space.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrBlank(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Get initials from a given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetInitials(string name)
        {
            string[] nameSplit = name.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);

            string initials = "";

            foreach (string item in nameSplit)
            {
                initials += item.Substring(0, 1).ToUpper();
            }

            return initials;
        }

        public static void Clone(this PhotoElement source, PhotoElement target)
        {
            target.Source = source.Source;
            target.Id = source.Id;
            target.Grayscale = source.Grayscale;
            target.Contrast = source.Contrast;
            target.Brightness = source.Brightness;
            target.Saturation = source.Saturation;
            target.Sepia = source.Sepia;
            target.Invert = source.Invert;
            target.Hue = source.Hue;
            target.Blur = source.Blur;
            target.Opacity = source.Opacity;
        }
    }
}
