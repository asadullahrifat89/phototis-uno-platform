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
            target.ImageGrayscale = source.ImageGrayscale;
            target.ImageContrast = source.ImageContrast;
            target.ImageBrightness = source.ImageBrightness;
            target.ImageSaturation = source.ImageSaturation;
            target.ImageSepia = source.ImageSepia;
            target.ImageInvert = source.ImageInvert;
            target.ImageHue = source.ImageHue;
            target.ImageBlur = source.ImageBlur;
            target.ImageOpacity = source.ImageOpacity;
        }
    }
}
