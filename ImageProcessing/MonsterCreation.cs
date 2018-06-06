using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageProcessing
{
    class MonsterCreation
    {
        public string ContentRootPath { get; set; } = @"E:\Programming\CSharp\Artur\Monster\monsters-images\src\Monsters.Generator";
        public Random Random { get; set; } = new Random();

        public MonsterCreation()
        {
            for (int i = 0; i < 30; i++)
            {
                Create(new { Level = i/10 }, i + 1);
            }
            Create(new { Level = 1}, 1);
        }

        void Create(dynamic monster, int id)
        {
            MagickImage result = null;
            string bodyColor = GetRandomColor(MonsterColors.BodyArmsLegs);
            if (monster.Level > 0) // not baby
            {
                result = AddImagePart(result, GetPath(monster.Level, FolderNames.LegsMainC, 1), bodyColor);
                //result.Write(Path.Combine(ContentRootPath, "MonsterResources", "Test", $"1.png")); return;
                result = AddImagePart(result, GetPath(monster.Level, FolderNames.LegsPatternC, 1), GetRandomColor(MonsterColors.Fingers_Horn_PatternBodyArmsLegs));
                result = AddImagePart(result, GetPath(monster.Level, FolderNames.LegsFingersC, 1), GetRandomColor(MonsterColors.Fingers_Horn_PatternBodyArmsLegs));
                result = AddImagePart(result, GetPath(monster.Level, FolderNames.LegsShadow, 1));

                result = AddImagePart(result, GetPath(monster.Level, FolderNames.ArmsMainC, 1), bodyColor);
                result = AddImagePart(result, GetPath(monster.Level, FolderNames.ArmsPatternC, 1), GetRandomColor(MonsterColors.Fingers_Horn_PatternBodyArmsLegs));
                result = AddImagePart(result, GetPath(monster.Level, FolderNames.ArmsFingersC, 1), GetRandomColor(MonsterColors.Fingers_Horn_PatternBodyArmsLegs));
                result = AddImagePart(result, GetPath(monster.Level, FolderNames.ArmsShadow, 1));
            }

            result = AddImagePart(result, GetPath(monster.Level, FolderNames.HornsMainC, 1), GetRandomColor(MonsterColors.Fingers_Horn_PatternBodyArmsLegs));
            result = AddImagePart(result, GetPath(monster.Level, FolderNames.HornsPatternC, 1), GetRandomColor(MonsterColors.HornPattern_EyesIris));
            result = AddImagePart(result, GetPath(monster.Level, FolderNames.HornsLight, 1));

            result = AddImagePart(result, GetPath(monster.Level, FolderNames.BodyMainC, 1), bodyColor);
            result = AddImagePart(result, GetPath(monster.Level, FolderNames.BodyPatternC, 1), GetRandomColor(MonsterColors.Fingers_Horn_PatternBodyArmsLegs));
            result = AddImagePart(result, GetPath(monster.Level, FolderNames.BodyShadow, 1));

            result = AddImagePart(result, GetPath(monster.Level, FolderNames.MouthMain, 1));
            result = AddImagePart(result, GetPath(monster.Level, FolderNames.MouthTongueC, 1), GetRandomColor(MonsterColors.Tongue));
            result = AddImagePart(result, GetPath(monster.Level, FolderNames.MouthTeeth, 1));

            result = AddImagePart(result, GetPath(monster.Level, FolderNames.EyesShadow, 1));
            result = AddImagePart(result, GetPath(monster.Level, FolderNames.EyesProteinC, 1), GetRandomColor(MonsterColors.EyeProtein));
            result = AddImagePart(result, GetPath(monster.Level, FolderNames.EyesProteinLight, 1));
            result = AddImagePart(result, GetPath(monster.Level, FolderNames.EyesIrisC, 1), GetRandomColor(MonsterColors.HornPattern_EyesIris));
            result = AddImagePart(result, GetPath(monster.Level, FolderNames.EyesIrisLight, 1));

            result.Write(Path.Combine(ContentRootPath, "MonsterResources", "test",  $"Monster-{id}.png"));
        }

        public MagickImage AddImagePart(MagickImage initial, string addImagePath, string color = null)
        {
            MagickImage addImage;


            if (!string.IsNullOrWhiteSpace(color))
            {
                addImage = ReplaceColor(addImagePath, "#FFFFFF", "#" + color);
            } else
            {
                addImage = new MagickImage(addImagePath);
            }
            addImage.Format = MagickFormat.Png32; // to make translucent pixels work correctly
            addImage.VirtualPixelMethod = VirtualPixelMethod.Transparent;
            if (initial == null)
            {
                return addImage;
            }
            else
            {
                initial.Composite(addImage, CompositeOperator.Over);
                return initial;
            }
        }

        string GetRandomColor(string[] colors)
        {
            return colors[Random.Next(colors.Length)];
        }

        MagickImage ReplaceColor(string addImagePath, string from, string to)
        {
            var bpm = new Bitmap(addImagePath);
            Color colorFrom = ColorTranslator.FromHtml(from);
            Color colorTo = ColorTranslator.FromHtml(to);
            for (int i = 0; i < bpm.Height; i++)
            {
                for (int j = 0; j < bpm.Width; j++)
                {
                    Color pixel = bpm.GetPixel(j, i);
                    if (pixel.A > 0)
                    {
                        bpm.SetPixel(j, i, Color.FromArgb(pixel.A,
                            pixel.R * colorTo.R / 255,
                            pixel.G * colorTo.G / 255,
                            pixel.B * colorTo.B / 255));
                    }
                }
            }
            var result = new MagickImage(ImageToBytes(bpm));
            bpm.Dispose();
            return result;

            /*image.Alpha(AlphaOption.Deactivate);
            image.ColorFuzz = new ImageMagick.Percentage(70); //  color comparison freedom     
            image.Opaque(from, to);
            image.Alpha(AlphaOption.Activate);
            //image.Write(Path.Combine(ContentRootPath, "MonsterResources", "Test", $"1.png"));
            return image;*/
        }

        public void AddOverlay(Bitmap lower, Bitmap upper)
        {
            using (var output = new Bitmap(lower.Width, lower.Height))
            {
                var width = lower.Width;
                var height = lower.Height;

                for (var i = 0; i < height; i++)
                {
                    for (var j = 0; j < width; j++)
                    {
                        var upperPixel = upper.GetPixel(j, i);
                        var lowerPixel = lower.GetPixel(j, i);

                        var lowerColor = new HSLColor(lowerPixel.R, lowerPixel.G, lowerPixel.B);
                        var upperColor = new HSLColor(upperPixel.R, upperPixel.G, upperPixel.B) { Luminosity = lowerColor.Luminosity };
                        var outputColor = (Color)upperColor;

                        output.SetPixel(j, i, outputColor);
                    }
                }
            }
        }

        public static byte[] ImageToBytes(Bitmap img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public string GetPath(long level, string folderName, int imageId)
        {
            string levelFolder = "";
            switch (level)
            {
                case 0: levelFolder = FolderNames.AgeBaby; break;
                case 1: levelFolder = FolderNames.AgeTeen; break;
                case 2: levelFolder = FolderNames.AgeAdult; break;
            }
            string folderPath = Path.Combine(ContentRootPath, "MonsterResources", levelFolder, folderName);
            string imageName = folderName.Split(new char[] { ' ' }).Last() + $"-__.png";
            string resultPath = Path.Combine(folderPath, imageName);
            string imageIdStr = imageId < 10 ? "0" + imageId.ToString() : imageId.ToString();
            resultPath = resultPath.Replace("__", imageIdStr);
            return resultPath;
        }

        public static class FolderNames
        {
            public static readonly string LegsRoot = "1 Legs";
            public static readonly string LegsMainC = Path.Combine(LegsRoot, "1C LegsMain");
            public static readonly string LegsPatternC = Path.Combine(LegsRoot, "2C legs__Pattern");
            public static readonly string LegsFingersC = Path.Combine(LegsRoot, "3C LegsFingers");
            public static readonly string LegsShadow = Path.Combine(LegsRoot, "4 LegsShadow");

            public static readonly string ArmsRoot = "2 Arms";
            public static readonly string ArmsMainC = Path.Combine(ArmsRoot, "1C ArmsMain");
            public static readonly string ArmsPatternC = Path.Combine(ArmsRoot, "2C Arms__Pattern");
            public static readonly string ArmsFingersC = Path.Combine(ArmsRoot, "3C ArmsFingers");
            public static readonly string ArmsShadow = Path.Combine(ArmsRoot, "4 ArmsShadow");

            public static readonly string HornsRoot = "3 Horns";
            public static readonly string HornsMainC = Path.Combine(HornsRoot, "1C HornMain");
            public static readonly string HornsPatternC = Path.Combine(HornsRoot, "2C Horn__Pattern");
            public static readonly string HornsLight = Path.Combine(HornsRoot, "3 HornLight");

            public static readonly string BodyRoot = "4 Body";
            public static readonly string BodyMainC = Path.Combine(BodyRoot, "1C BodyMain");
            public static readonly string BodyPatternC = Path.Combine(BodyRoot, "2C Body__Pattern");
            public static readonly string BodyShadow = Path.Combine(BodyRoot, "3 BodyShadow");

            public static readonly string MouthRoot = "5 Mouth";
            public static readonly string MouthMain = Path.Combine(MouthRoot, "1 Mouth");
            public static readonly string MouthTongueC = Path.Combine(MouthRoot, "2C Tongue");
            public static readonly string MouthTeeth = Path.Combine(MouthRoot, "3 Teeth");

            public static readonly string EyesRoot = "6 Eyes";
            public static readonly string EyesShadow = Path.Combine(EyesRoot, "1 EyesShadow");
            public static readonly string EyesProteinC = Path.Combine(EyesRoot, "2C EyesProtein");
            public static readonly string EyesProteinLight = Path.Combine(EyesRoot, "3 EyesProteinLight");
            public static readonly string EyesIrisC = Path.Combine(EyesRoot, "4C EyesIris");
            public static readonly string EyesIrisLight = Path.Combine(EyesRoot, "5 EyesIrisLight");

            public static readonly string AgeBaby = "BabyMonster Elements";
            public static readonly string AgeTeen = "TeenMonster Elements";
            public static readonly string AgeAdult = "AdultMonster Elements";
        }

        public static class MonsterColors
        {
            public static readonly string[] BodyArmsLegs = new string[] { "4B3A64", "524861", "6b5bce", "757dce", "b062c0", "cc4fdf", "b73bd7", "f23f77", "f55556", "fe6d50", "fd8146", "ff8800", "fad242", "e1e753", "aad82e", "31db80", "4cd9ba", "5ce8eb", "28c9e9", "82c9e0" };
            public static readonly string[] Fingers_Horn_PatternBodyArmsLegs = new string[] { "984ed9", "f95a9c", "ff708d", "fe5a59", "f2a34c", "f9d75a", "4fbd4f", "2bd973", "96e876", "12bee2", "6be9f2" };
            public static readonly string[] HornPattern_EyesIris = new string[] { "473754", "74358a", "b16cd4", "ab5d5d", "fad242", "ebe652", "ffb062", "3b69a4", "2b99ba", "402623", "175c3f" };
            public static readonly string[] EyeProtein = new string[] { "fff2c4", "ffe278", "ffd437", "ffa133" };
            public static readonly string[] Tongue = new string[] { "ff437f" };
        }
    }
}
