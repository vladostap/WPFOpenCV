using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace PhotoEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private string ImagePath;
        private Image<Bgr, Byte> imageToSave;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Choose/Save File

        private void BtnChooseImage_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    ImagePath = fileDialog.FileName;
                    ImgMainWindow.Source = new BitmapImage(new Uri(ImagePath));
                    ComboBoxFilters.Visibility = Visibility.Visible;
                    BtnSaveImg.Visibility = Visibility.Visible;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    ImagePath = "";
                    break;
            }
        }

        private void BtnSaveImg_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "JPEG Image (.jpeg)|*.jpeg",
                DefaultExt = "jpeg",
                AddExtension = true
            };
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    imageToSave.Save(fileDialog.FileName);
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }

        #endregion

        #region EmguWPF

        /// <summary>
        /// Delete a GDI object
        /// </summary>
        /// <param name="o">The poniter to the GDI object to be deleted</param>
        /// <returns></returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        #endregion

        private void ComboBoxFilters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Slider1.Visibility = Visibility.Hidden;
            Slider1.Minimum = 1;
            Slider1.Value = 1;

            BtnHFlip.Visibility = Visibility.Hidden;
            BtnVFlip.Visibility = Visibility.Hidden;

            if (Dilate.IsSelected)
            {
                Slider1.ValueChanged += Slider1_ValueChanged1;
                Slider1.Visibility = Visibility.Visible;
            }

            if (Erode.IsSelected)
            {
                Slider1.ValueChanged += Slider1_ValueChanged2;
                Slider1.Visibility = Visibility.Visible;
            }

            if (Mul.IsSelected)
            {
                Slider1.Minimum = 2;
                Slider1.ValueChanged += Slider1_ValueChanged3;
                Slider1.Visibility = Visibility.Visible;
            }

            if (Flip.IsSelected)
            {
                BtnHFlip.Visibility = Visibility.Visible;
                BtnVFlip.Visibility = Visibility.Visible;
            }

            if (GammaCorrect.IsSelected)
            {
                Slider1.ValueChanged += Slider1_ValueChanged4;
                Slider1.Visibility = Visibility.Visible;
            }
        }

        private void Slider1_ValueChanged4(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var image = new Image<Bgr, Byte>(ImagePath);
            image._GammaCorrect(Convert.ToDouble(Slider1.Value));
            ImgMainWindow.Source = ToBitmapSource(image);
            imageToSave = image;
        }

        private void Slider1_ValueChanged3(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var image = new Image<Bgr, Byte>(ImagePath);
            image._Mul(Convert.ToInt32(Slider1.Value));
            ImgMainWindow.Source = ToBitmapSource(image);
            imageToSave = image;
        }

        private void Slider1_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var image = new Image<Bgr, Byte>(ImagePath);
            image._Erode(Convert.ToInt32(Slider1.Value));
            ImgMainWindow.Source = ToBitmapSource(image);
            imageToSave = image; ;
        }

        private void Slider1_ValueChanged1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var image = new Image<Bgr, Byte>(ImagePath);
            image._Dilate(Convert.ToInt32(Slider1.Value));
            ImgMainWindow.Source = ToBitmapSource(image);
            imageToSave = image;
        }

        private void Slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //TODO
        }

        private void BtnHFlip_Click(object sender, RoutedEventArgs e)
        {
            var image = new Image<Bgr, Byte>(ImagePath);
            image._Flip(Emgu.CV.CvEnum.FlipType.Horizontal);
            ImgMainWindow.Source = ToBitmapSource(image);
            imageToSave = image;
        }

        private void BtnVFlip_Click(object sender, RoutedEventArgs e)
        {
            var image = new Image<Bgr, Byte>(ImagePath);
            image._Flip(Emgu.CV.CvEnum.FlipType.Vertical);
            ImgMainWindow.Source = ToBitmapSource(image);
            imageToSave = image;
        }
    }
}
