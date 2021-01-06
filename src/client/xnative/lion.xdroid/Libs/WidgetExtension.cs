using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;

namespace Lion.XDroid.Libs
{
    /// <summary>
    /// 
    /// </summary>
    public static class WidgetExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text_view"></param>
        /// <param name="text_value"></param>
        public static void SetText(this TextView text_view, string text_value)
        {
            text_view.SetText(text_value, TextView.BufferType.Normal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text_view"></param>
        /// <param name="color_value"></param>
        public static void SetTextColor(this TextView text_view, int color_value)
        {
            text_view.SetTextColor(new Color(color_value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text_view"></param>
        /// <param name="tf"></param>
        /// <param name="type_value"></param>
        public static void SetTypeface(this TextView text_view, Typeface tf, int type_value)
        {
            text_view.SetTypeface(tf, (TypefaceStyle)type_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="background"></param>
        /// <param name="color_value"></param>
        public static void SetColorFilter(this Drawable background, uint color_value)
        {
            background.SetColorFilter(IntToRgbColor(color_value), PorterDuff.Mode.SrcOver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image_view"></param>
        /// <param name="color_value"></param>
        public static void SetColorFilter(this ImageView image_view, uint color_value)
        {
            image_view.SetColorFilter(IntToRgbColor(color_value), PorterDuff.Mode.SrcOver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argb"></param>
        /// <returns></returns>
        public static Color IntToRgbColor(uint argb)
        {
            return Color.Argb(
                                (byte)((argb & -16777216) >> 0x18),
                                (byte)((argb & 0xff0000) >> 0x10),
                                (byte)((argb & 0xff00) >> 8),
                                (byte)(argb & 0xff)
                             );

            //var red = (int)(argb >> 0) & 255;
            //var green = (int)(argb >> 8) & 255;
            //var blue = (int)(argb >> 16) & 255;
            //return new Color(red, green, blue);
        }
    }
}