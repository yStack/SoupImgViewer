using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HalconDotNet;

namespace DemoWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private HImage img = new HImage();

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = @"img|*.bmp;*.png;*.jpeg;*.jpg;*.tiff";
            openFileDialog.ShowDialog();

            try
            {
                img.ReadImage(openFileDialog.FileName);
                imgViewer.DisplayObject2D(img);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Reduce_Click(object sender, RoutedEventArgs e)
        {
            HRegion roi = imgViewer.RoiRegion;
            HImage domain = img.ReduceDomain(roi);
            imgViewer.DisplayObject2D(domain);
        }
    }
}
