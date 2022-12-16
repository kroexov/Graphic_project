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
        
        private string _selectedFilter = "ThresholdFiltering";
        private ObservableCollection<string>  _filters = new ObservableCollection<string>()
        {
            "ThresholdFiltering",
            "ThresholdFilteringByOcu",
            "MedianFiltering",
            "GaussFiltering",
            "BoxBlurFiltering",
            "SobelFiltering",
            "ContrastAdaptiveSharpening"
        };

        private PnmServices _model;

        private bool _errorOccured = false;
        
        private string _errorText = "Неизвестная ошибка";

        private bool _firstChannel = true;
        private bool _secondChannel = true;
        private bool _thirdChannel = true;

        private int _filtrationThreshold;

        private string _coreRadius;
        private string _sigma;
        private string _sharpness;

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
        
        public ObservableCollection<string> Filters
        {
            get => _filters;
        }
        
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFilter, value);
                RaisePropertyChanged(nameof(IsThreshold));
                RaisePropertyChanged(nameof(IsRadius));
                RaisePropertyChanged(nameof(IsSigma));
                RaisePropertyChanged(nameof(IsSharpness));
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

        public bool IsThreshold => _selectedFilter == "ThresholdFiltering";
        public bool IsRadius => (_selectedFilter == "MedianFiltering" || _selectedFilter == "BoxBlurFiltering");
        public bool IsSigma => _selectedFilter == "GaussFiltering";
        public bool IsSharpness => _selectedFilter == "ContrastAdaptiveSharpening";
        
        public string Sharpness{
            get => _sharpness;
            set => this.RaiseAndSetIfChanged(ref _sharpness, value);
        }
        public int FiltrationThreshold
        {
            get => _filtrationThreshold;
            set => this.RaiseAndSetIfChanged(ref _filtrationThreshold, value);
        }
        public string CoreRadius
        {
            get => _coreRadius;
            set => this.RaiseAndSetIfChanged(ref _coreRadius, value);
        }
        public string Sigma
        {
            get => _sigma;
            set => this.RaiseAndSetIfChanged(ref _sigma, value);
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
        
        public string ApplySelectedFiler()
        {
            TypeFilter Filter = (TypeFilter) Enum.Parse(typeof(TypeFilter), _selectedFilter, true);
            double value = 0;
            
            switch (Filter)
            {
                case TypeFilter.ThresholdFiltering:
                    value = _filtrationThreshold;
                    break;
                case TypeFilter.MedianFiltering:
                    if (!double.TryParse(_coreRadius, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out value) &&
                        !double.TryParse(_coreRadius, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out value) &&
                        !double.TryParse(_coreRadius, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    {
                        value = 0;
                    }
                    break;
                case TypeFilter.BoxBlurFiltering:
                    if (!double.TryParse(_coreRadius, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out value) &&
                        !double.TryParse(_coreRadius, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out value) &&
                        !double.TryParse(_coreRadius, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    {
                        value = 0;
                    }
                    break;
                case TypeFilter.GaussFiltering:
                    if (!double.TryParse(_sigma, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out value) &&
                        !double.TryParse(_sigma, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out value) &&
                        !double.TryParse(_sigma, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    {
                        value = 0;
                    }
                    break;
                case TypeFilter.ContrastAdaptiveSharpening:
                    if (!double.TryParse(_sharpness, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out value) &&
                        !double.TryParse(_sharpness, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out value) &&
                        !double.TryParse(_sharpness, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    {
                        value = 0;
                    }
                    break;
            }
            return _model.FilterImage(Filter, value);
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

