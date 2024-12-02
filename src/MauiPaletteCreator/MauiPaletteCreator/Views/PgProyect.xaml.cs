using MauiPaletteCreator.ViewModels;

namespace MauiPaletteCreator.Views;

public partial class PgProyect : ContentPage
{
	public PgProyect(PgProyectViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}