using System;
using ImageMagick;

namespace ImageProcessing
{
    class Program
    {
        static string basePath = @"..\..\..\images\";

        static void Main(string[] args)
        {
            MonsterCreation m = new MonsterCreation();
            return;
            var colorYellow = MagickColor.FromRgb(255, 242, 0);
            var colorBlue = MagickColor.FromRgb(0, 0, 255);
            ReplaceColor(new MagickImage($"{basePath}rectYellow.png"), 
                colorYellow, colorBlue, "rectBlue.png");
            
            CombimeImages(new MagickImage($"{basePath}partial1.png"), new MagickImage($"{basePath}partial2.png"), "combine.png");
        }

        static void CombimeImages(MagickImage image1, MagickImage image2, string name)
        {
            image1.Composite(image2, CompositeOperator.Over);
            Save(image1, name);
        }

        static void ReplaceColor(MagickImage image, MagickColor from, MagickColor to, string name)
        {
            image.ColorFuzz = new ImageMagick.Percentage(5); //  color comparison freedom
            image.Opaque(from, to);
            Save(image, name);
        }

        static void Save(MagickImage image, string name)
        {
            image.Write($"{basePath}{name}");
        }
    }
}
