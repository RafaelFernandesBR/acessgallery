using System.Windows.Input;
using AcessGallery.ViewModels;

namespace AcessGallery.Views.Controls;

public partial class PhotoGridControl : ContentView
{
    public PhotoGridControl()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty PhotosProperty = BindableProperty.Create(
        nameof(Photos),
        typeof(IEnumerable<PhotoItemViewModel>),
        typeof(PhotoGridControl));

    public IEnumerable<PhotoItemViewModel> Photos
    {
        get => (IEnumerable<PhotoItemViewModel>)GetValue(PhotosProperty);
        set => SetValue(PhotosProperty, value);
    }

    public static readonly BindableProperty PhotoTappedCommandProperty = BindableProperty.Create(
        nameof(PhotoTappedCommand),
        typeof(ICommand),
        typeof(PhotoGridControl));

    public ICommand PhotoTappedCommand
    {
        get => (ICommand)GetValue(PhotoTappedCommandProperty);
        set => SetValue(PhotoTappedCommandProperty, value);
    }
    
    public static readonly BindableProperty ShowEmptyViewProperty = BindableProperty.Create(
        nameof(ShowEmptyView),
        typeof(bool),
        typeof(PhotoGridControl),
        defaultValue: true);
        
    public bool ShowEmptyView
    {
        get => (bool)GetValue(ShowEmptyViewProperty);
        set => SetValue(ShowEmptyViewProperty, value);
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is PhotoItemViewModel selectedPhoto && PhotoTappedCommand != null)
        {
             if (PhotoTappedCommand.CanExecute(selectedPhoto))
             {
                 PhotoTappedCommand.Execute(selectedPhoto);
             }
        }
        
        // Clear selection to allow re-selecting the same item
        if (sender is CollectionView cv)
        {
            cv.SelectedItem = null;
        }
    }
}
