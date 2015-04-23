/*
 *
 * author: jbuddha
 * timestamp: 4/23/2015 9:12 AM
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace JarScanner
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }
        
        void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            this.resultText.Text = textBox.Text;  
            
        }

        void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.searchBox.GotFocus -= searchBox_GotFocus;
            this.searchBox.Text = "";
        }
        
        void Main_Loaded(object sender, RoutedEventArgs e)
        {
            this.resultText.Text = "Search Results";
            this.searchBox.TextChanged += searchBox_TextChanged;
            this.searchBox.GotFocus += searchBox_GotFocus;
        }

    }
}