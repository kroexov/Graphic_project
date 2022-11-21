using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Lab1.Models;
using ReactiveUI;

namespace Lab1.ViewModels;

public class AlgorithmWindowViewModel : ViewModelBase
{
    private string _selectedAlg = "Ordered";
    private int _bitn = 1;
    private PnmServices _services;
    private Bitmap? _imageToLoad;
    
    private ObservableCollection<string> _algorithms = new ObservableCollection<string>()
    {
        "Ordered",
        "Random",
        "Floyd-Steinberg",
        "Atkinson"
    };

    public AlgorithmWindowViewModel(PnmServices services)
    {
        _services = services;
    }

    public string SelectedAlg
    {
        get => _selectedAlg;
        set
        {
            _selectedAlg = value;
            this.RaiseAndSetIfChanged(ref _selectedAlg, value);
            SetPath(_services.UseDither(_bitn, _selectedAlg));
        }
    }
    
    public int Bitn
    {
        get => _bitn;
        set
        {
            _bitn = value;
            this.RaiseAndSetIfChanged(ref _bitn, value);
            SetPath(_services.UseDither(_bitn, _selectedAlg));
        }
    }
    
    public ObservableCollection<string> Algorithms
    {
        get => _algorithms;
    }
    
    public Bitmap? ImageToLoadPublic
    {
        get => _imageToLoad;
        private set => this.RaiseAndSetIfChanged(ref _imageToLoad, value);
    }
    
    private void SetPath(string path)
    {
        ImageToLoadPublic = new Bitmap(path);
    }
}