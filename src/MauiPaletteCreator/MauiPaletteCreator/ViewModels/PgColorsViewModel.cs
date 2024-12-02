using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPaletteCreator.ViewModels;

public partial class PgColorsViewModel : ObservableObject
{
    [RelayCommand]
    async Task GoToNext()
    {
        await Shell.Current.GoToAsync(nameof(PgView), true);
    }
}
