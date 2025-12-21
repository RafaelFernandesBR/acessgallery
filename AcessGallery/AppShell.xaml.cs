namespace AcessGallery;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        
        Routing.RegisterRoute("PhotoDetailPage", typeof(PhotoDetailPage));
        Routing.RegisterRoute("AlbumDetailPage", typeof(AlbumDetailPage));
	}
}
