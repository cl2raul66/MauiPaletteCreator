using CommunityToolkit.Mvvm.Messaging;
using MauiPaletteCreator.ViewModels;

namespace MauiPaletteCreator.Views;

public partial class PgMain : ContentPage
{
	public PgMain(PgMainViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
	}
}