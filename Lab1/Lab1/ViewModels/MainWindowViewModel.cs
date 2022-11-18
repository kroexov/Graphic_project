﻿using System;
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

        private string _selectedColorSpace;

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

        private PnmServices _model;

        private bool _errorOccured = false;
        
        private string _errorText = "Неизвестная ошибка";

        private bool _redChannel = true;
        private bool _greenChannel = true;
        private bool _blueChannel = true;

        #endregion

        #region Constructor

        public MainWindowViewModel(PnmServices model)
        {
            _model = model;
            //model.ModelErrorHappened += (s => OnErrorHappened(s));
            ImageDisplayViewModel = new ImageDisplayViewModel();
        }

        #endregion

        #region Public properties

        public string SelectedColorSpace
        {
            get => _selectedColorSpace;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedColorSpace, value);
            }
        }

        public bool RedChannel
        {
            get => _redChannel;
            set
            {
                this.RaiseAndSetIfChanged(ref _redChannel, value);
                //_model.ColorType = _redChannel;
            } 
        }
        
        public bool GreenChannel
        {
            get => _greenChannel;
            set
            {
                this.RaiseAndSetIfChanged(ref _greenChannel, value);
                //_model.ColorType = _greenChannel;
            } 
        }
        
        public bool BlueChannel
        {
            get => _blueChannel;
            set
            {
                this.RaiseAndSetIfChanged(ref _blueChannel, value);
                //_model.ColorType = _blueChannel;
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
        
        public ImageDisplayViewModel ImageDisplayViewModel { get; }

        public ObservableCollection<string> ColorSpaces
        {
            get => _spaces;
            set
            {
                this.RaiseAndSetIfChanged(ref _spaces, value);
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
                OpenFile(result.First());
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

        public void OpenFile(string path)
        {
            bool isok = true;
            try
            {
                _model.ReadFile(path);
            }
            catch (Exception e)
            {
                isok = false;
                OnErrorHappened(e.Message);
            }

            if (isok)
            {
                string altpath = _model.ReadFile(path);
                //string pathFile = _model.AfterOpenFileLogic(path);
                ImageDisplayViewModel.SetPath(altpath);
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

