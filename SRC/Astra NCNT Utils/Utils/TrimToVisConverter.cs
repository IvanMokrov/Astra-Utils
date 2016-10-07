using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Astra_NICNT_Utils.Utils
{

    /// <summary> Control will Visible when TextBlock text is trimmed </summary>
    public class TrimToVisConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var TT = value as ToolTip;
            if (TT == null) return Visibility.Collapsed;

            TextBlock   textBlock   = (TT.PlacementTarget as TextBlock);
            TextBox     textBox     = (TT.PlacementTarget as TextBox);
            Button      button      = (TT.PlacementTarget as Button);

            if (textBlock != null)
            {
                return TextBlockService.IsTextTrimmed(textBlock) 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }

            if (textBox != null)
            {
                return TextBlockService.IsTextTrimmed(textBox)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            if (button != null) // ToolTip for buttons enabled too
            {
                return Visibility.Visible;
            }

            // ToolTips for other controls is restricted
            return Visibility.Collapsed;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
