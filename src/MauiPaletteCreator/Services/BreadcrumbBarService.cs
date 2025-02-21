using MauiPaletteCreator.Models.View;
using MauiPaletteCreator.Views;

namespace MauiPaletteCreator.Services;

public interface IBreadcrumbBarService
{
    IReadOnlyList<BreadcrumbBarItem> Items { get; }

    void Init();
    void GoBack(string pageName);
    void GoNext(string pageName);
}

public sealed class BreadcrumbBarService : IBreadcrumbBarService
{
    private readonly BreadcrumbBarItem[] _items;

    public BreadcrumbBarService()
    {
        _items = [
            CreateItem(nameof(PgMain), "lang:LbPositionMain"),
            CreateItem(nameof(PgProject), "lang:LbPositionProject"),
            CreateItem(nameof(PgColors), "lang:LbPositionColors"),
            CreateItem(nameof(PgView), "lang:LbPositionView"),
            CreateItem(nameof(PgEnd), "lang:LbPositionEnd")
        ];
    }

    public IReadOnlyList<BreadcrumbBarItem> Items => _items;

    public void Init()
    {
        GoNext(nameof(PgMain));
    }

    public void GoNext(string pageName)
    {
        var (_, idx) = FindItemByPageName(pageName);
        if (idx == -1) return;

        if (idx == _items.Length - 1 && !_items[^2].IsVisited)
        {
            _items[^2].IsVisited = true;
        }

        _items[idx].IsVisited = true;
    }

    public void GoBack(string pageName)
    {
        var (_, idx) = FindItemByPageName(pageName);
        if (idx == -1) return;

        for (int i = idx + 1; i < _items.Length; i++)
        {
            _items[i].IsVisited = false;
        }
    }

    private static BreadcrumbBarItem CreateItem(string pageName, string resourceKey) =>
        new()
        {
            PageName = pageName,
            IsVisited = false,
            Title = App.Current?.Resources[resourceKey] as string
        };

    private (BreadcrumbBarItem? item, int index) FindItemByPageName(string pageName)
    {
        var index = Array.FindIndex(_items, x => x.PageName == pageName);
        return index != -1 ? (_items[index], index) : (null, -1);
    }
}
