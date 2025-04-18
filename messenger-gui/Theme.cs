using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace messenger_gui
{
    class Theme
    {

        int SchemeColor = 0;

        public double saturation = 0;

        List<int> ColorBrightnessData = new List<int>();
        public Theme(ResourceDictionary resources, int colorsCount)
        {
            ParseData(resources, colorsCount);
        }

        private void ParseData(ResourceDictionary resources, int colorsCount)
        {
            for (int i = 0; i < colorsCount; i++)
            {
                ColorBrightnessData.Add(GetThemeColor(resources, i).Color.R);
            }
        }

        public SolidColorBrush GetThemeColor(ResourceDictionary resources, int i)
        {
            return (SolidColorBrush)(resources[$"StyleColor{i + 1}"]);
        }

        private void SetThemeColor(ResourceDictionary resources, int i, SolidColorBrush newColor)
        {
            resources[$"StyleColor{i + 1}"] = newColor;
        }

        private static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        public void UpdateColorScheme(int hue, ResourceDictionary resources)
        {
            SchemeColor = hue;

            for (int i = 0; i < ColorBrightnessData.Count; i++)
            {
                //SolidColorBrush ColorBrush = GetThemeColor(i);
                //double brightness = Math.Clamp(ColorData[i], 0, 255)/255d;
                SetThemeColor(resources, i, new SolidColorBrush(MakeColor(ColorBrightnessData[i])));
            }

        }

        public Color MakeColor(double value)
        {
            return ColorFromHSV(SchemeColor, saturation, value / 255d);
        }
    }
}
