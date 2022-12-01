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

        private string _currentPath;

        private string _gamma = "0";

        private string _selectedColorSpace = "RGB";

        private ObservableCollection<string> _items = new ObservableCollection<string>();
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

        private int _lineWidth = 1;

        private bool _errorOccured = false;
        
        private string _errorText = "Неизвестная ошибка";

        private bool _firstChannel = true;
        private bool _secondChannel = true;
        private bool _thirdChannel = true;

        private string _firstChannelValue = "1.0";
        private string _secondChannelValue = "1.0";
        private string _thirdChannelValue = "1.0";

        private string _opacity = "1.0";

        private double _x1;
        private double _x2;
        private double _y1;
        private double _y2;

        #endregion

        #region Constructor

        public MainWindowViewModel(PnmServices model)
        {
            _model = model;
            model.ModelErrorHappened += (s => OnErrorHappened(s));
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
                _model.ChangeColorSpace((ColorSpace) Enum.Parse(typeof(ColorSpace), _selectedColorSpace, true));
                var res = _model.RefreshImage();
                if (res != string.Empty)
                {
                    ImageDisplayViewModel.SetPath(res);
                }
            }
        }

        public string Point1
        {
            get => _x1.ToString(CultureInfo.InvariantCulture) + " " + _y1.ToString(CultureInfo.InvariantCulture);
            set
            {
                var numbers = value.Split();
                this.RaiseAndSetIfChanged(ref _x1, Double.Parse(numbers[0], CultureInfo.InvariantCulture));
                this.RaiseAndSetIfChanged(ref _y1, Double.Parse(numbers[1], CultureInfo.InvariantCulture));
            }
        }
        
        public string Point2
        {
            get => _x2.ToString(CultureInfo.InvariantCulture) + " " + _y2.ToString(CultureInfo.InvariantCulture);
            set
            { 
                var numbers = value.Split();
                this.RaiseAndSetIfChanged(ref _x2, Double.Parse(numbers[0], CultureInfo.InvariantCulture));
                this.RaiseAndSetIfChanged(ref _y2, Double.Parse(numbers[1], CultureInfo.InvariantCulture));
            }
        }
        
        public int LineWidth
        {
            get => _lineWidth;
            set
            {
                this.RaiseAndSetIfChanged(ref _lineWidth, value);
            }
        }

        public bool FirstChannel
        {
            get => _firstChannel;
            set
            {
                this.RaiseAndSetIfChanged(ref _firstChannel, value);
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

        public string GammaValue
        {
            get => _gamma;
            set
            {
                this.RaiseAndSetIfChanged(ref _gamma, value);
            }
        }
        
        public bool SecondChannel
        {
            get => _secondChannel;
            set
            {
                this.RaiseAndSetIfChanged(ref _secondChannel, value);
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
        
        public bool ThirdChannel
        {
            get => _thirdChannel;
            set
            {
                this.RaiseAndSetIfChanged(ref _thirdChannel, value);
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

        public ObservableCollection<string> ColorSpaces
        {
            get => _spaces;
            set
            {
                this.RaiseAndSetIfChanged(ref _spaces, value);
            }
        }
        
        public string FirstChannelValue
        {
            get => _firstChannelValue;
            set => this.RaiseAndSetIfChanged(ref _firstChannelValue, value);
        }
        
        public string SecondChannelValue
        {
            get => _secondChannelValue;
            set => this.RaiseAndSetIfChanged(ref _secondChannelValue, value);
        }
        
        public string ThirdChannelValue
        {
            get => _thirdChannelValue;
            set => this.RaiseAndSetIfChanged(ref _thirdChannelValue, value);
        }
        
        public string Opacity
        {
            get => _opacity;
            set => this.RaiseAndSetIfChanged(ref _opacity, value);
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
                _currentPath = result.First();
                OpenFile(result.First());
            }
        }

        public void ChangeGamma()
        {
            double result;

            //Try parsing in the current culture
            if (!double.TryParse(_gamma, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                //Then try in US english
                !double.TryParse(_gamma, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                //Then in neutral language
                !double.TryParse(_gamma, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = 0;
            }
            
            _model.AssignGamma(result);
            var res = _model.RefreshImage();
            if (res != string.Empty)
            {
                ImageDisplayViewModel.SetPath(res);
            }
        }

        public void ApplyGamma()
        {
            double result;

            //Try parsing in the current culture
            if (!double.TryParse(_gamma, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                //Then try in US english
                !double.TryParse(_gamma, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                //Then in neutral language
                !double.TryParse(_gamma, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = 0;
            }
            _model.ConvertGamma(result);
            var res = _model.RefreshImage();
            if (res != string.Empty)
            {
                ImageDisplayViewModel.SetPath(res);
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

        public void DrawLine()
        {
            double value1, value2, value3, opacity;
            
            if (!double.TryParse(_firstChannelValue, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out value1) &&
                !double.TryParse(_firstChannelValue, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out value1) &&
                !double.TryParse(_firstChannelValue, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out value1))
            {
                value1 = 0;
            }
            
            if (!double.TryParse(_secondChannelValue, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out value2) &&
                !double.TryParse(_secondChannelValue, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out value2) &&
                !double.TryParse(_secondChannelValue, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out value2))
            {
                value2 = 0;
            }
            
            if (!double.TryParse(_thirdChannelValue, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out value3) &&
                !double.TryParse(_thirdChannelValue, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out value3) &&
                !double.TryParse(_thirdChannelValue, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out value3))
            {
                value3 = 0;
            }
            
            if (!double.TryParse(_opacity, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out opacity) &&
                !double.TryParse(_opacity, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out opacity) &&
                !double.TryParse(_opacity, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out opacity))
            {
                opacity = 0;
            }
            
            if (!_x1.Equals(_x2).Equals(_y1).Equals(_y2).Equals(0.0))
            {
                _model.DrawLine((int)_x1, (int)_y1, (int)_x2, (int)_y2, _lineWidth, opacity, new []{value1, value2, value3});
            }
            
            var res = _model.RefreshImage();
            if (res != string.Empty)
            {
                ImageDisplayViewModel.SetPath(res);
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

