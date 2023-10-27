using System;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;


namespace CoverageAlgorithm
{
    /// <summary>
    /// Interaction logic for ResizeWindow.xaml
    /// </summary>
    public partial class ResizeWindow : Window
    {
        CoverageSpace matrix;

        public ResizeWindow(CoverageSpace matrixToResize)
        {
            InitializeComponent();
            matrix = matrixToResize;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Tuple<int, int> size = new Tuple<int, int>(Convert.ToInt32(XTextBox.Text), Convert.ToInt32(YTextBox.Text));
            matrix.CreateMatrix(size);
            //matrix.UpdateCode(MatrixBase.CodeType.HEX, true);
            Close();
        }
    }
}
