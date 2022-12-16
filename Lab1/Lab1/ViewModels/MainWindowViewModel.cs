using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

        private string _selectedColorSpace = "RGB";
        private ObservableCollection<string> _spaces = new ObservableCollection<string>()
        {
            "RGB",
            "HSL",
            "HSV",
            "YCbCr601",
            "YCbCr709",
            "YСoCg",
            "CMY"
        };

        private PnmServices _model;

        private bool _errorOccured = false;
        
        private string _errorText = "Неизвестная ошибка";

        private string _ignoranceRate = "0";

        private bool _firstChannel = true;
        private bool _secondChannel = true;
        private bool _thirdChannel = true;

        #endregion

        #region Constructor

        public MainWindowViewModel(PnmServices model)
        {
            _model = model;
            model.ModelErrorHappened += (s => OnErrorHappened(s));
            ImageDisplayViewModel = new ImageDisplayViewModel();
            HistogramDisplayViewModel = new HistogramDisplayViewModel();
        }

        #endregion

        #region Public properties

        public string SelectedColorSpace
        {
            get => _selectedColorSpace;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedColorSpace, value);
                _model.ChangeColorSpace((ColorSpace) Enum.Parse(typeof(ColorSpace), _selectedColorSpace, true));
                var res = _model.RefreshImage();
                if (res != string.Empty)
                {
                    ImageDisplayViewModel.SetPath(res);
                }
            }
        }

        public bool FirstChannel
        {
            get => _firstChannel;
            set
            {
                this.RaiseAndSetIfChanged(ref _firstChannel, value);
                if (!value)
                {
                    HistogramDisplayViewModel.ClearChannel1();
                }
                _model.ChangeColorChannel(new bool[3]
                {
                    _firstChannel, _secondChannel, _thirdChannel
                });
                var res = _model.RefreshImage();
                if (res != string.Empty)
                {
                    ImageDisplayViewModel.SetPath(res);
                }
                
            } 
        }
        
        public bool SecondChannel
        {
            get => _secondChannel;
            set
            {
                this.RaiseAndSetIfChanged(ref _secondChannel, value);
                if (!value)
                {
                    HistogramDisplayViewModel.ClearChannel2();
                }
                _model.ChangeColorChannel(new bool[3]
                {
                    _firstChannel, _secondChannel, _thirdChannel
                });
                var res = _model.RefreshImage();
                if (res != string.Empty)
                {
                    ImageDisplayViewModel.SetPath(res);
                }
            } 
        }
        
        public string IgnoranceRate
        {
            get => _ignoranceRate;
            set
            {
                this.RaiseAndSetIfChanged(ref _ignoranceRate, value);
            }
        }
        
        public bool ThirdChannel
        {
            get => _thirdChannel;
            set
            {
                this.RaiseAndSetIfChanged(ref _thirdChannel, value);
                if (!value)
                {
                    HistogramDisplayViewModel.ClearChannel3();
                }
                _model.ChangeColorChannel(new bool[3]
                {
                    _firstChannel, _secondChannel, _thirdChannel
                });
                var res = _model.RefreshImage();
                if (res != string.Empty)
                {
                    ImageDisplayViewModel.SetPath(res);
                }
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
        
        public HistogramDisplayViewModel HistogramDisplayViewModel { get; }

        public ObservableCollection<string> ColorSpaces
        {
            get => _spaces;
            set
            {
                this.RaiseAndSetIfChanged(ref _spaces, value);
            }
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
                File.WriteAllBytes(result,_model.SaveFile());
            }
        }

        public void OpenFile(string path)
        {
            try
            {
                string altpath = _model.ReadFile(path, new bool[] {_firstChannel, _secondChannel, _thirdChannel}, (ColorSpace) Enum.Parse(typeof(ColorSpace), _selectedColorSpace, true));
                ImageDisplayViewModel.SetPath(altpath);
            }
            catch (Exception e)
            {
                OnErrorHappened(e.Message);
            }
        }

        public void CreateHistogram()
        {
            // test part, change this
            double result;

            //Try parsing in the current culture
            if (!double.TryParse(_ignoranceRate, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                //Then try in US english
                !double.TryParse(_ignoranceRate, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                //Then in neutral language
                !double.TryParse(_ignoranceRate, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = 0;
            }

            var fullFileName=_model.CreateHistogram(result);
            if (_firstChannel)
            {
                HistogramDisplayViewModel.SetPathForChannel1(fullFileName);
            }
            
            if (_secondChannel)
            {
                HistogramDisplayViewModel.SetPathForChannel2(fullFileName);
            }
            
            if (_thirdChannel)
            {
                HistogramDisplayViewModel.SetPathForChannel3(fullFileName);
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

