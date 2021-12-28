using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;


namespace Y2SharpWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// '
    
    public partial class MainWindow : Window
    {
        public string videoid;
        public MainWindow()
        {
            InitializeComponent();
            typebox.Items.Add("MP3");
            typebox.Items.Add("MP4");
            typebox.SelectedItem = "MP3";
        }

        public Y2Sharp.Youtube.Video video1;

        private async void typebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if(typebox.SelectedItem.ToString() == "MP3")
            {
                qualitybox.Items.Clear();
                qualitybox.Items.Add("128kbps");
                qualitybox.SelectedItem = "128kbps";
            }
            else if(typebox.SelectedItem.ToString() == "MP4")
            {
                qualitybox.Items.Clear();
                foreach(var res in video1.Resolutions)
                {
                    qualitybox.Items.Add(res);
                }
            }
        }

        public void DownloadButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            downloadAsync();
        }

        public async Task downloadAsync()
        {
            if (URLtextbox.Text == string.Empty)
            {
                MessageBox.Show("Empty url error!", "Y2Sharp error message");
                return;
            }

            if (!URLtextbox.Text.Contains("https://www.youtube.com/watch?v="))
            {
                if (MessageBox.Show("Warning! Url might be formatted incorrectly, Do you want to continue?",
"Y2Sharp info message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Close the window  
                }
                else
                {
                    return;
                }
            }

            

            string quality = "128";
            if (typebox.SelectedItem.ToString() == "MP3")
            {
                quality = qualitybox.SelectedItem.ToString().Substring(0, qualitybox.SelectedItem.ToString().Length - 4);
            }
            if (typebox.SelectedItem.ToString() == "MP4")
            {
                quality = qualitybox.SelectedItem.ToString().Substring(0, qualitybox.SelectedItem.ToString().Length - 1);
            }


            string typetext = "MP3";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = video1.Title;
            if (typebox.SelectedItem.ToString() == "MP3") { typetext = "Audio Files"; }
            if (typebox.SelectedItem.ToString() == "MP4") { typetext = "Video Files"; }
            saveFileDialog.Filter = typetext + " | *." + typebox.SelectedItem.ToString().ToLower();
            saveFileDialog.DefaultExt = "." + typebox.SelectedItem.ToString().ToLower();
            saveFileDialog.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            


            
            if(saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await video1.DownloadAsync(System.IO.Path.GetFullPath(saveFileDialog.FileName).ToString(), typebox.SelectedItem.ToString().ToLower(), quality);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Y2Sharp WPF error message");
                    return;
                }

                MessageBox.Show("Video saved to: " + System.IO.Path.GetFullPath(saveFileDialog.FileName.ToString()), "Y2Sharp info message");
            }
            else
            {
               
                return;
            }

            
        }

        private async void URLtextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(URLtextbox.Text == "Youtube URL...") { return; }
            if(URLtextbox.Text == string.Empty) { return; }
            
            string videourl = URLtextbox.Text;
            videoid = videourl.Remove(0, 32);
            videoid = videoid.Remove(11, videoid.Length - 11);

            await Y2Sharp.Youtube.Video.GetInfo(videoid);

            video1 = new Y2Sharp.Youtube.Video();

            if (typebox.SelectedItem.ToString() == "MP3")
            {
                qualitybox.Items.Clear();
                qualitybox.Items.Add("128kbps");
                qualitybox.SelectedItem = "128kbps";
            }
            else if (typebox.SelectedItem.ToString() == "MP4")
            {
                qualitybox.Items.Clear();
                foreach (var res in video1.Resolutions)
                {
                    qualitybox.Items.Add(res);
                }
                
            }
        }

        private void URLtextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(URLtextbox.Text == "Youtube URL...")
            {
                URLtextbox.Text = string.Empty;
            }
        }

        private void URLtextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(URLtextbox.Text == string.Empty)
            {
                URLtextbox.Text = "Youtube URL...";
            }
        }
    }
}
