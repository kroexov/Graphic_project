using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Lab1.Models;
using Lab1.Views;
using ReactiveUI;

namespace Lab1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Private fields

        private string _selectedColorSpace = "RGB";

        private double _width;
        private double _height;
        private double _xOffset;
        private double _yOffset;
        private string B;
        private string C;
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
        
        private string _selectedScaling = "Closest point";
        private ObservableCollection<string>  _scalings = new ObservableCollection<string>()
        {
            "Closest point",
            "Bilinear",
            "Lanczos3",
            "BC-splines"
        };

        private PnmServices _model;

        private bool _errorOccured = false;
        
        private string _errorText = "Неизвестная ошибка";

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
        
        public ObservableCollection<string> Scalings
        {
            get => _scalings;
        }

        public string BValue
        {
            get => B;
            set
            {
                this.RaiseAndSetIfChanged(ref B, value);
            }
        }
        
        public string CValue
        {
            get => C;
            set
            {
                this.RaiseAndSetIfChanged(ref C, value);
            }
        }
        
        public string SelectedScaling
        {
            get => _selectedScaling;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedScaling, value);
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
        
        public double Xoffset
        {
            get => _xOffset;
            set
            {
                this.RaiseAndSetIfChanged(ref _xOffset, value);
            }
        }
        public double Yoffset
        {
            get => _yOffset;
            set
            {
                this.RaiseAndSetIfChanged(ref _yOffset, value);
            }
        }
        public double ImageWidth
        {
            get => _width;
            set
            {
                this.RaiseAndSetIfChanged(ref _width, value);
            }
        }
        public double ImageHeight
        {
            get => _height;
            set
            {
                this.RaiseAndSetIfChanged(ref _height, value);
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
                WidthChanged?.Invoke(new Bitmap(altpath).Size.Width);
                HeightChanged?.Invoke(new Bitmap(altpath).Size.Height);
                if (!altpath.Equals(String.Empty) && (_height > 500 && _width > 500))
                {
                    ImageDisplayViewModel.SetImage(new CroppedBitmap(new Bitmap(altpath), new PixelRect(Convert.ToInt32(_width/2 - 250 + _xOffset), Convert.ToInt32(_height/2 - 250 + _yOffset), 500, 500)));
                }
                else if (!altpath.Equals(String.Empty) && _height < 500 && _width < 500)
                {
                    ImageDisplayViewModel.SetImage(new CroppedBitmap(new Bitmap(altpath), new PixelRect(0, 0, Convert.ToInt32(_width), Convert.ToInt32(_height))));
                }
            }
            catch (Exception e)
            {
                OnErrorHappened(e.Message);
            }
        }

        public void ResizeImage()
        {
            _model.ResizeImage(Convert.ToInt32(_height), Convert.ToInt32(_width), _xOffset, _yOffset, _selectedScaling);
            var path = _model.RefreshImage();
            if (!path.Equals(String.Empty) && (_height > 500 && _width > 500))
            {
                ImageDisplayViewModel.SetImage(new CroppedBitmap(new Bitmap(path), new PixelRect(Convert.ToInt32(_width/2 - 250 + _xOffset), Convert.ToInt32(_height/2 - 250 + _yOffset), 500, 500)));
            }
            else if (!path.Equals(String.Empty) && _height < 500 && _width < 500)
            {
                ImageDisplayViewModel.SetImage(new CroppedBitmap(new Bitmap(path), new PixelRect(0, 0, Convert.ToInt32(_width), Convert.ToInt32(_height))));
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
        
        public event Action<double>? HeightChanged;
        public event Action<double>? WidthChanged;

        #endregion
        
    }
}

