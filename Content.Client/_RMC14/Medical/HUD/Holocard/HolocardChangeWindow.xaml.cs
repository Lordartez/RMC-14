using Content.Shared._RMC14.Medical.HUD;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using static Robust.Client.UserInterface.Controls.ItemList;

namespace Content.Client._RMC14.Medical.HUD.Holocard;

/// <summary>
///     A window that allows you to change the holocard of the associated entity
/// </summary>
[GenerateTypedNameReferences]
public sealed partial class HolocardChangeWindow : DefaultWindow
{
    [Dependency] private readonly IEntityManager _entities = default!;

    private readonly HolocardChangeBoundUserInterface _owner;

    public HolocardChangeWindow(HolocardChangeBoundUserInterface owner)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        _owner = owner;

        Title = Loc.GetString("ui-holocard-change-title");

        HolocardStateList.OnItemSelected += OnHolocardStateSelect;
        HolocardStateList.OnItemDeselected += OnHolocardStateDeselect;

        SearchBar.OnTextChanged += (_) => UpdateHolocardStateList(SearchBar.Text);

        UpdateHolocardStateList();
    }

    private void OnHolocardStateSelect(ItemListSelectedEventArgs obj)
    {
        var newSelectedHolocard = (HolocardStatus?) obj.ItemList[obj.ItemIndex].Metadata;

        if (newSelectedHolocard is { } newHolocard)
        {
            _owner.ChangeHolocard(newHolocard);
            _owner.Close();
        }
    }

    private void OnHolocardStateDeselect(ItemListDeselectedEventArgs obj)
    {
    }

    private void UpdateHolocardStateList(string? filter = null)
    {
        HolocardStateList.Clear();

        var holoCard = _entities.System<ShowHolocardIconsSystem>();
        foreach (var status in Enum.GetValues<HolocardStatus>())
        {
            if (!holoCard.TryGetHolocardName(status, out var holocardName))
            {
                continue;
            }

            if (!string.IsNullOrEmpty(filter) &&
                !holocardName.ToLowerInvariant().Contains(filter.Trim().ToLowerInvariant()))
            {
                continue;
            }

            ItemList.Item listEntry = new(HolocardStateList)
            {
                Text = holocardName,
                Metadata = status,
            };

            HolocardStateList.Add(listEntry);
        }
    }
}
