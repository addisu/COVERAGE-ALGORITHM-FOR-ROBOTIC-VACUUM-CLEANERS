using System;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace CoverageAlgorithm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CoverageSpace matrix;
        Collection collection;
        CollectionEditWindow collectionEditWindow;

        public MainWindow()
        {
            InitializeComponent();

            matrix = new CoverageSpace(new Tuple<int, int>(30, 15), MatrixGrid, StartCleaningButton);
        }


        private void EnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            matrix.SetAll(true);
        }


        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            ResizeWindow resizeWindow = new ResizeWindow(matrix);
            resizeWindow.Show();
        }

    }
}
