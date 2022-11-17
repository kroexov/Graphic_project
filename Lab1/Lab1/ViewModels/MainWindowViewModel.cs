using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Lab1.Models;
using Lab1.Views;
using ReactiveUI;

namespace Lab1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Private fields

        private string _data;

        private string _selectedImage = string.Empty;

        private ObservableCollection<string> _items = new ObservableCollection<string>();
        private ObservableCollection<string> _spaces = new ObservableCollection<string>()
        {
            "RGB",
            "HSL",
            "HSV",
            "YCbCr.601",
            "YCbCr.709",
            "YCoCg",
            "CMY"
        };

        private PortableAnyMapModel _model;

        private bool _errorOccured = false;
        
        private string _errorText = "Неизвестная ошибка";

        private bool _rgbMode = true;

        #endregion

        #region Constructor

        public MainWindowViewModel(PortableAnyMapModel model)
        {
            _model = model;
            model.ModelErrorHappened += (s => OnErrorHappened(s));
            ImageDisplayViewModel = new ImageDisplayViewModel();
        }

        #endregion

        #region Public properties

        public string SelectedImage
        {
            get => _selectedImage;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedImage, value);
                RaisePropertyChanged(nameof(IsImageSelected));
            }
        }

        public bool RgbMode
        {
            get => _rgbMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _rgbMode, value);
                _model.ColorType = _rgbMode;
            } 
        }

        public bool IsErrorOccured
        {
            get => _errorOccured;
            set
            {
                this.RaiseAndSetIfChanged(ref _errorOccured, value);
            }
        }
        
        public string ErrorText
        {
            get => _errorText;
            set
            {
                this.RaiseAndSetIfChanged(ref _errorText, value);
            }
        }

        public bool IsImageSelected => SelectedImage != String.Empty;
        
        public ImageDisplayViewModel ImageDisplayViewModel { get; }

        public ObservableCollection<string> ColorSpaces
        {
            get => _spaces;
            set
            {
                this.RaiseAndSetIfChanged(ref _spaces, value);
                RaisePropertyChanged(nameof(IsImageSelected));
            }
        }

        public string Data
        {
            get => _data;
            set => this.RaiseAndSetIfChanged(ref _data, value);
        }

        #endregion

        #region Public methods

        public async void AddNewFile()
        {
            var ofd = new OpenFileDialog();
            string[]? result = null;
            ofd.Filters.Add(new FileDialogFilter() {Name = "Другой файл", Extensions = {"*"}});
            ofd.Filters.Add(new FileDialogFilter() {Name = "Файлы P6", Extensions = {"ppm"}});
            ofd.Filters.Add(new FileDialogFilter() {Name = "Файлы P5", Extensions = {"pgm"}});
            
            result = await ofd.ShowAsync(new Window());
            if (result != null)
            {
                
                _items.Add(result.First());
                Data = File.ReadAllText(result.First());
                _selectedImage = result.First(); // TODO: rework, delete this
                OpenFile();
                return;
            }
        }

        public async void SaveFile()
        {
            var ofd = new SaveFileDialog();
            ofd.Filters.Add(new FileDialogFilter() {Name = "Другой файл", Extensions = {"*"}});
            ofd.Filters.Add(new FileDialogFilter() {Name = "Файлы P6", Extensions = {"ppm"}});
            ofd.Filters.Add(new FileDialogFilter() {Name = "Файлы P5", Extensions = {"pgm"}});
            
            var result = await ofd.ShowAsync(new Window());
            if (result != null)
            {
                File.WriteAllText(result, Data);
            }
        }

        public void DeleteFile()
        {
            if (_items.Contains(_selectedImage))
            {
                _items.Remove(_selectedImage);
                _selectedImage = String.Empty;
                RaisePropertyChanged(nameof(IsImageSelected));
            }
        }

        public void OpenFile()
        {
            bool isok = true;
            try
            {
                _model.ReadFile(_selectedImage);
            }
            catch (Exception e)
            {
                isok = false;
                DeleteFile();
                OnErrorHappened(e.Message);
            }

            if (isok)
            {
                string _pathFile = _model.AfterOpenFileLogic(_selectedImage);
                ImageDisplayViewModel.SetPath(_pathFile);
            }
        }

        #endregion

        #region Private methods

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Events

        public event Action<string> OnErrorHappened;
        
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        
    }
}

