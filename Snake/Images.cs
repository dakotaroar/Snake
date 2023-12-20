using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snake
{
    public static class Images
    {
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource Body = LoadImage("Body.png");
        public readonly static ImageSource Head = LoadImage("Head.png");
        public readonly static ImageSource Food = LoadImage("C:\\Users\\802630ctc\\source\\repos\\Snake\\Snake\\Assets\\Food.png");
        public readonly static ImageSource DeadBody = LoadImage("DeadBody.png");
        public readonly static ImageSource DeadHead = LoadImage("DeadHead.png");
        public readonly static ImageSource Joe = LoadImage("C:\\Users\\802630ctc\\source\\repos\\Snake\\Snake\\Assets\\joe_scare.png");
        public readonly static ImageSource Wall = LoadImage("C:\\Users\\802630ctc\\source\\repos\\Snake\\Snake\\Assets\\wall.jpg");
        private static ImageSource LoadImage(string filename)
        {
            return new BitmapImage(new Uri($"Assets/{filename}",UriKind.Relative));
        }
    }
}
