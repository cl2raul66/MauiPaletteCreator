using MauiPaletteCreator.ViewModels;

namespace MauiPaletteCreator.Views;

public partial class PgColors : ContentPage
{
	public PgColors(PgColorsViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}