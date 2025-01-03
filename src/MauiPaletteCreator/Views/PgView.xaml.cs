using MauiPaletteCreator.ViewModels;

namespace MauiPaletteCreator.Views;

public partial class PgView : ContentPage
{
	public PgView(PgViewViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}