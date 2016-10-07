using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Threading.Thread;

namespace Astra_NICNT_Utils.Utils
{
    /// <summary> Service for TextBlock control </summary>
    public static class TextBlockService
    {

        /// <summary> 
        /// Check the text inside Texblock is trimmed 
        /// ! Damn WPF don't have a normal way or property to check it !
        /// </summary>
        public static bool IsTextTrimmed(UIElement textUI)
        {
            if (textUI is TextBlock)
            {
                TextBlock textBlock = (TextBlock) textUI;
                if (!textBlock.IsArrangeValid)
                {
                    return false;
                }

                Typeface typeface = new Typeface(
                    textBlock.FontFamily,
                    textBlock.FontStyle,
                    textBlock.FontWeight,
                    textBlock.FontStretch);

                FormattedText formattedText = new FormattedText(
                    textBlock.Text,
                    CurrentThread.CurrentCulture,
                    textBlock.FlowDirection,
                    typeface,
                    textBlock.FontSize,
                    textBlock.Foreground);

                formattedText.MaxTextWidth = textBlock.ActualWidth - textBlock.Padding.Left - textBlock.Padding.Right;

                return (formattedText.Height > textBlock.ActualHeight || formattedText.MinWidth > formattedText.MaxTextWidth);
            }

            if (textUI is TextBox)
            {
                TextBox textBox = (TextBox) textUI;

                if (!textBox.IsArrangeValid)
                {
                    return false;
                }

                Typeface typeface = new Typeface(
                    textBox.FontFamily,
                    textBox.FontStyle,
                    textBox.FontWeight,
                    textBox.FontStretch);

                FormattedText formattedText = new FormattedText(
                    textBox.Text,
                    CurrentThread.CurrentCulture,
                    textBox.FlowDirection,
                    typeface,
                    textBox.FontSize,
                    textBox.Foreground);

                formattedText.MaxTextWidth = textBox.ActualWidth - textBox.Padding.Left - textBox.Padding.Right;

                return (formattedText.Height > textBox.ActualHeight || formattedText.MinWidth > formattedText.MaxTextWidth);

            }

            return false;
        }
    }
}