using System.Collections.Immutable;
using ApneScan.Scan;

namespace ApneScan.Config;

public interface IProfileManager
{
    ImmutableList<ScanProfile> Profiles { get; }
    void Mutate(ListMutation<ScanProfile> mutation, ISelectable<ScanProfile> selectable);
    void Mutate(ListMutation<ScanProfile> mutation, ListSelection<ScanProfile> selection);
    ScanProfile? DefaultProfile { get; set; }
    void Load();
    void Save();

    event EventHandler ProfilesUpdated;
}