using System.Drawing;

namespace FlyffUAutoFSPro._Script
{
    public class ColorHelper
    {

        public static bool IsPixelColorEnemyYellowOrRed(int r, int g, int b)
        {
            return IsPixelColorEnemyYellow(r, g, b) || IsPixelColorEnemyRed(r, g, b);
        }
        public static bool IsPixelColorEnemyHpWhite(int r, int g, int b)
        {
            return (Utils.IsNumberInRange(r, 255, 80) && Utils.IsNumberInRange(g, 255, 80) && Utils.IsNumberInRange(b, 255, 80));
        }
        public static bool IsPixelColorEnemyYellow(int r, int g, int b)
        {
            return (Utils.IsNumberInRange(r, 240, 60) && Utils.IsNumberInRange(g, 240, 60) && Utils.IsNumberInRange(b, 155, 20));
        }
        public static bool IsPixelColorEnemyRed(int r, int g, int b)
        {
            return (Utils.IsNumberInRange(r, 245, 60) && Utils.IsNumberInRange(g, 10, 20) && Utils.IsNumberInRange(b, 10, 20));
        }
        public static bool IsPixelColorEnemyRedEnergybar(int r, int g, int b)
        {
            return (Utils.IsNumberInRange(r, 245, 60) && Utils.IsNumberInRange(g, 10, 20) && Utils.IsNumberInRange(b, 10, 20));
        }
        public static bool IsPixelColorEnemyBlack(int r, int g, int b)
        {
            return (Utils.IsNumberInRange(r, 0, 10) && Utils.IsNumberInRange(g, 0, 10) && Utils.IsNumberInRange(b, 0, 10));
        }

        public static bool IsPixelColorEnergyWhite(Color pixel)
        {
            return (Utils.IsNumberInRange(pixel.R, 255, 80) && Utils.IsNumberInRange(pixel.G, 255, 80) && Utils.IsNumberInRange(pixel.B, 255, 80));
        }

        public static bool IsEnergyLineStartEndColor(Color pixel)
        {
            return (Utils.IsNumberInRange(pixel.R, 125, 5) && Utils.IsNumberInRange(pixel.G, 110, 5) && Utils.IsNumberInRange(pixel.B, 57, 5));
        }

        public static bool IsPixelColorEnergyHP(Color pixel)
        {
            return (Utils.IsNumberInRange(pixel.R, 210, 30) && Utils.IsNumberInRange(pixel.G, 35, 10) && Utils.IsNumberInRange(pixel.B, 78, 10));
        }

        public static bool IsPixelColorEnergyMP(Color pixel)
        {
            return (Utils.IsNumberInRange(pixel.R, 38, 20) && Utils.IsNumberInRange(pixel.G, 160, 30) && Utils.IsNumberInRange(pixel.B, 220, 30));
        }

        public static bool IsPixelColorEnergyFP(Color pixel)
        {
            return (Utils.IsNumberInRange(pixel.R, 40, 10) && Utils.IsNumberInRange(pixel.G, 210, 30) && Utils.IsNumberInRange(pixel.B, 27, 10));
        }

        public static bool IsPixelColorEnergyBlack(Color pixel)
        {
            return (Utils.IsNumberInRange(pixel.R, 0, 10) && Utils.IsNumberInRange(pixel.G, 0, 10) && Utils.IsNumberInRange(pixel.B, 0, 10));
        }




    }
}
