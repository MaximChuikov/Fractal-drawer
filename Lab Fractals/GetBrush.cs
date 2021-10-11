using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Threading.Tasks;

namespace Lab_Fractals
{
    enum SystemBrush
    {
        Maroon,
        DarkRed,
        Firebrick,
        Brown,
        Chocolate,
        Coral,
        SandyBrown,
        Orange,
        Peru,
        Gold,
        Yellow,
        Khaki,
        DarkKhaki,
        MediumTurquoise,
        MediumAquamarine,
        MediumSeaGreen,
        LightSeaGreen,
        DarkTurquoise,
        LightSkyBlue,
        DeepSkyBlue,
        DodgerBlue,
        MediumSlateBlue,
        BlueViolet,
        DarkOrchid,
        Orchid,
        Violet,
        Magenta,
        Fuchsia,
        DeepPink,
        HotPink,
        IndianRed,
        MediumVioletRed
    }
    static class GetBrush
    {
        public static SolidColorBrush Get(SystemBrush brush)
        {
            return (int)brush switch
            {
                0 => Brushes.Maroon,
                1 => Brushes.DarkRed,
                2 => Brushes.Firebrick,
                3 => Brushes.Brown,
                4 => Brushes.Chocolate,
                5 => Brushes.Coral,
                6 => Brushes.SandyBrown,
                7 => Brushes.Orange,
                8 => Brushes.Peru,
                9 => Brushes.Gold,
                10 => Brushes.Yellow,
                11 => Brushes.Khaki,
                12 => Brushes.DarkKhaki,
                13 => Brushes.MediumTurquoise,
                14 => Brushes.MediumAquamarine,
                15 => Brushes.MediumSeaGreen,
                16 => Brushes.LightSeaGreen,
                17 => Brushes.DarkTurquoise,
                18 => Brushes.LightSkyBlue,
                19 => Brushes.DeepSkyBlue,
                20 => Brushes.DodgerBlue,
                21 => Brushes.MediumSlateBlue,
                22 => Brushes.BlueViolet,
                23 => Brushes.DarkOrchid,
                24 => Brushes.Orchid,
                25 => Brushes.Violet,
                26 => Brushes.Magenta,
                27 => Brushes.Fuchsia,
                28 => Brushes.DeepPink,
                29 => Brushes.HotPink,
                30 => Brushes.IndianRed,
                31 => Brushes.MediumVioletRed,
                _ => Brushes.White,
            };
        }
        public static void AddBrush(ref SystemBrush brush, uint num)
        {
            brush += (int)num;
            if ((int)brush > 31)
            {
                int i = (int)brush;
                i %= 32;
                brush = (SystemBrush)i;
            }


        }
    }
}
