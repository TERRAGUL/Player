using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading; 
using Microsoft.Win32;

namespace ggg
{
    public partial class MainWindow : Window
    {
        private List<string> _audioFiles = new List<string>();
        private Random _random = new Random();
        private bool _isShuffled = false;
        private bool _isRepeat = false;
        private bool _isPlaying = false;
        private int _currentIndex = 0;
        private bool _isSliderDragging = false;
        private DispatcherTimer _sliderTimer; 
        private DispatcherTimer _textTimer; 

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
           
            _sliderTimer = new DispatcherTimer();
            _sliderTimer.Interval = TimeSpan.FromMilliseconds(100); 
            _sliderTimer.Tick += SliderThread_Tick;
            _sliderTimer.Start();

            
            _textTimer = new DispatcherTimer();
            _textTimer.Interval = TimeSpan.FromSeconds(1); 
            _textTimer.Tick += TextThread_Tick;
            _textTimer.Start();
        }

        private void SliderThread_Tick(object sender, EventArgs e)
        {
          
            if (!_isSliderDragging && mediaElement.NaturalDuration.HasTimeSpan)
            {
                sliderPosition.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                sliderPosition.Value = mediaElement.Position.TotalSeconds;
            }
        }

        private void TextThread_Tick(object sender, EventArgs e)
        {
         
            if (_isPlaying && mediaElement.NaturalDuration.HasTimeSpan)
            {
                TimeSpan currentTime = mediaElement.Position;
                TimeSpan totalTime = mediaElement.NaturalDuration.TimeSpan;

                lblCurrentTime.Content = $"Current Time: {currentTime:mm\\:ss}";
                lblRemainingTime.Content = $"Remaining Time: {(totalTime):mm\\:ss}";
            }
        }

        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Audio Files|*.mp3;*.wav;*.m4a;*.flac|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                _audioFiles.Clear();
                foreach (string filename in openFileDialog.FileNames)
                {
                    _audioFiles.Add(filename);
                }
                _currentIndex = 0;
                PlayAudio(_audioFiles[_currentIndex]);
            }
        }

        private void PlayAudio(string filename)
        {
            mediaElement.Source = new Uri(filename);
            mediaElement.Play();
            _isPlaying = true;
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                mediaElement.Pause();
                _isPlaying = false;
            }
            else
            {
                mediaElement.Play();
                _isPlaying = true;
            }
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (_audioFiles.Count == 0)
                return;

            if (_currentIndex > 0)
            {
                _currentIndex--;
                PlayAudio(_audioFiles[_currentIndex]);
            }
            else if (_isRepeat)
            {
                PlayAudio(_audioFiles[_currentIndex]);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_audioFiles.Count == 0)
                return;

            if (_currentIndex < _audioFiles.Count - 1)
            {
                _currentIndex++;
                PlayAudio(_audioFiles[_currentIndex]);
            }
            else if (_isRepeat)
            {
                _currentIndex = 0;
                PlayAudio(_audioFiles[_currentIndex]);
            }
        }

        private void BtnRepeat_Click(object sender, RoutedEventArgs e)
        {
            _isRepeat = !_isRepeat;
            mediaElement.MediaEnded -= MediaElement_MediaEnded; 
            if (_isRepeat)
            {
                mediaElement.MediaEnded += MediaElement_MediaEnded; 
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_isRepeat)
            {
                PlayAudio(_audioFiles[_currentIndex]);
            }
        }

        private void BtnShuffle_Click(object sender, RoutedEventArgs e)
        {
            _isShuffled = !_isShuffled;
            if (_isShuffled)
            {
                ShufflePlaylist();
            }
            else
            {
                _audioFiles.Sort();
            }
        }

        private void ShufflePlaylist()
        {
            int n = _audioFiles.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                string value = _audioFiles[k];
                _audioFiles[k] = _audioFiles[n];
                _audioFiles[n] = value;
            }
        }

        private void SliderPosition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isSliderDragging)
            {
                mediaElement.Position = TimeSpan.FromSeconds(sliderPosition.Value);
            }
        }

        private void SliderPosition_DragStarted(object sender, RoutedEventArgs e)
        {
            _isSliderDragging = true;
        }

        private void SliderPosition_DragCompleted(object sender, RoutedEventArgs e)
        {
            _isSliderDragging = false;
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = sliderVolume.Value;
        }
    }
}
