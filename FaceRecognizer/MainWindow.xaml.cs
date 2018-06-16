using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace FaceRecognizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly CascadeClassifier ClassifierForEyes
           = new CascadeClassifier("_haar/haarcascade_eye.xml");

        private static readonly CascadeClassifier ClassifierForNose
            = new CascadeClassifier("_haar/haarcascade_nose.xml");

        private int currentImg;
        private int prevRand = 0;

        private string imagePath;

        public MainWindow()
        {
            InitializeComponent();

            if (Directory.Exists(AppContext.BaseDirectory
                + Properties.Settings.Default.PathToProcImages))
            {
                Directory.Delete(AppContext.BaseDirectory
                    + Properties.Settings.Default.PathToProcImages, true);
            }

            var images = Directory.GetFiles(AppContext.BaseDirectory
                + Properties.Settings.Default.PathToImages);

            var rand = new Random();
            var randImg = rand.Next(1, images.Count() + 1);
            prevRand = randImg;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            currentImg = randImg;
            bitmapImage.UriSource = new Uri($"\\_faces\\{randImg}.jpg", UriKind.Relative);
            bitmapImage.EndInit();

            ImageWindow.Source = bitmapImage;
        }

        /// <summary>
        /// Устанавливаем рандомное изображение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetRandomImageButton_Click(object sender, RoutedEventArgs e)
        {
            RBEyes.Checked += RBEyes_Checked;
            RBNose.Checked += RBNose_Checked;

            RBEyes.IsChecked = false;
            RBNose.IsChecked = false;

            var images = Directory.GetFiles(AppContext.BaseDirectory
                + Properties.Settings.Default.PathToImages);

            var rand = new Random();
            var randImg = rand.Next(1, images.Count() + 1);

            while (prevRand == randImg)
                randImg = rand.Next(1, images.Count() + 1);

            prevRand = randImg;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            currentImg = randImg;
            bitmapImage.UriSource = new Uri($"\\_faces\\{randImg}.jpg", UriKind.Relative);
            bitmapImage.EndInit();

            ImageWindow.Source = bitmapImage;
        }

        /// <summary>
        /// Определение глаз
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RBEyes_Checked(object sender, RoutedEventArgs e)
        {
            string procImagePath;

            var capture = new Capture();
            var viewer = new ImageViewer();

            using (var image = new Mat($"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToImages}\\{currentImg}.jpg",
                LoadImageType.Color))
            using (Image<Gray, byte> gray = image.ToImage<Gray, byte>())
            {
                var rectangles = ClassifierForEyes.DetectMultiScale(gray,
                                            scaleFactor: 1.2,
                                            minNeighbors: 12);

                foreach (var rectangle in rectangles)
                {
                    gray.Draw(rectangle, new Gray(50), 3);
                }

                procImagePath = $"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}\\{currentImg}_{Guid.NewGuid().ToString()}_.jpg";

                if (!Directory.Exists($"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}"))
                {
                    Directory.CreateDirectory($"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}");
                }

                gray.Save(procImagePath);

                viewer.Image = gray;
            }

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(procImagePath, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            ImageWindow.Source = bitmapImage;
        }

        /// <summary>
        /// Определение носа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RBNose_Checked(object sender, RoutedEventArgs e)
        {
            string procImagePath;

            var capture = new Capture();
            var viewer = new ImageViewer();

            using (var image = new Mat($"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToImages}\\{currentImg}.jpg",
                LoadImageType.Color))
            using (Image<Gray, byte> gray = image.ToImage<Gray, byte>())
            {
                var rectangles = ClassifierForNose.DetectMultiScale(gray,
                                            scaleFactor: 1.2,
                                            minNeighbors: 12);

                foreach (var rectangle in rectangles)
                {
                    gray.Draw(rectangle, new Gray(50), 3);
                }

                procImagePath = $"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}\\{currentImg}_{Guid.NewGuid().ToString()}_.jpg";

                if (!Directory.Exists($"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}"))
                {
                    Directory.CreateDirectory($"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}");
                }

                gray.Save(procImagePath);

                viewer.Image = gray;
            }

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(procImagePath, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            ImageWindow.Source = bitmapImage;
        }

        /// <summary>
        /// Выбираем изображение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            RBEyes.Checked += RBEyes_Checked1;
            RBNose.Checked += RBNose_Checked1;

            RBEyes.IsChecked = false;
            RBNose.IsChecked = false;

            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    imagePath = fileDialog.FileName;
                    ImageWindow.Source = new BitmapImage(new Uri(imagePath));
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }

        /// <summary>
        /// Определение носа на выбранном изображении
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RBNose_Checked1(object sender, RoutedEventArgs e)
        {
            string procImagePath;

            var capture = new Capture();
            var viewer = new ImageViewer();

            using (var image = new Mat(imagePath,
                LoadImageType.Color))
            using (Image<Gray, byte> gray = image.ToImage<Gray, byte>())
            {
                var rectangles = ClassifierForNose.DetectMultiScale(gray,
                                            scaleFactor: 1.2,
                                            minNeighbors: 12);

                foreach (var rectangle in rectangles)
                {
                    gray.Draw(rectangle, new Gray(50), 3);
                }

                procImagePath = $"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}\\_{Guid.NewGuid().ToString()}_.jpg";

                if (!Directory.Exists($"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}"))
                {
                    Directory.CreateDirectory($"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}");
                }

                gray.Save(procImagePath);

                viewer.Image = gray;
            }

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(procImagePath, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            ImageWindow.Source = bitmapImage;
        }

        /// <summary>
        /// Определение глаз на выбранном изображении
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RBEyes_Checked1(object sender, RoutedEventArgs e)
        {
            string procImagePath;

            var capture = new Capture();
            var viewer = new ImageViewer();

            using (var image = new Mat(imagePath,
                LoadImageType.Color))
            using (Image<Gray, byte> gray = image.ToImage<Gray, byte>())
            {
                var rectangles = ClassifierForEyes.DetectMultiScale(gray,
                                            scaleFactor: 1.2,
                                            minNeighbors: 12);

                foreach (var rectangle in rectangles)
                {
                    gray.Draw(rectangle, new Gray(50), 3);
                }

                procImagePath = $"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}\\_{Guid.NewGuid().ToString()}_.jpg";

                if (!Directory.Exists($"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}"))
                {
                    Directory.CreateDirectory($"{AppContext.BaseDirectory}{Properties.Settings.Default.PathToProcImages}");
                }

                gray.Save(procImagePath);

                viewer.Image = gray;
            }

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(procImagePath, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            ImageWindow.Source = bitmapImage;
        }
    }
}
