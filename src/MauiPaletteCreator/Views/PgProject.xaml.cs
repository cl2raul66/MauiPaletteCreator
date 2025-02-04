using MauiPaletteCreator.ViewModels;

namespace MauiPaletteCreator.Views;

public partial class PgProject : ContentPage
{
	public PgProject(PgProjectViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}