using MauiPaletteCreator.ViewModels;

namespace MauiPaletteCreator.Views;

public partial class PgEnd : ContentPage
{
	public PgEnd(PgEndViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}