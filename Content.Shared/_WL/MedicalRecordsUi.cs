using Content.Shared.Med;
using Content.Shared.StationRecords;
using Robust.Shared.Serialization;

namespace Content.Shared.MedicalRecords;

[Serializable, NetSerializable]
public enum MedicalRecordsConsoleKey : byte
{
    Key
}

/// <summary>
///     Medical records console state. There are a few states:
///     - SelectedKey null, Record null, RecordListing null
///         - The station record database could not be accessed.
///     - SelectedKey null, Record null, RecordListing non-null
///         - Records are populated in the database, or at least the station has
///           the correct component.
///     - SelectedKey non-null, Record null, RecordListing non-null
///         - The selected key does not have a record tied to it.
///     - SelectedKey non-null, Record non-null, RecordListing non-null
///         - The selected key has a record tied to it, and the record has been sent.
///
///     - there is added new filters and so added new states
///         -SelectedKey null, Record null, RecordListing null, filters non-null
///            the station may have data, but they all did not pass through the filters
///
///     Other states are erroneous.
/// </summary>
[Serializable, NetSerializable]
public sealed class MedicalRecordsConsoleState : BoundUserInterfaceState
{
    /// <summary>
    /// Currently selected crewmember record key.
    /// </summary>
    public uint? SelectedKey = null;

    public MedicalRecord? MedicalRecord = null;
    public GeneralStationRecord? StationRecord = null;
    public readonly Dictionary<uint, string>? RecordListing;
    public readonly StationRecordsFilter? Filter;

    public MedicalRecordsConsoleState(Dictionary<uint, string>? recordListing, StationRecordsFilter? newFilter)
    {
        RecordListing = recordListing;
        Filter = newFilter;
    }

    /// <summary>
    /// Default state for opening the console
    /// </summary>
    public MedicalRecordsConsoleState() : this(null, null)
    {
    }

    public bool IsEmpty() => SelectedKey == null && StationRecord == null && MedicalRecord == null && RecordListing == null;
}

/// <summary>
/// Used to change status, respecting the wanted/reason nullability rules in <see cref="MedicalRecord"/>.
/// </summary>
[Serializable, NetSerializable]
public sealed class MedicalRecordChangeStatus : BoundUserInterfaceMessage
{
    public readonly MedStatus Status;
    public readonly string? Reason;

    public MedicalRecordChangeStatus(MedStatus status, string? reason)
    {
        Status = status;
        Reason = reason;
    }
}

/// <summary>
/// Used to add a single line to the record's crime history.
/// </summary>
[Serializable, NetSerializable]
public sealed class MedicalRecordAddHistory : BoundUserInterfaceMessage
{
    public readonly string Line;

    public MedicalRecordAddHistory(string line)
    {
        Line = line;
    }
}

/// <summary>
/// Used to delete a single line from the crime history, by index.
/// </summary>
[Serializable, NetSerializable]
public sealed class MedicalRecordDeleteHistory : BoundUserInterfaceMessage
{
    public readonly uint Index;

    public MedicalRecordDeleteHistory(uint index)
    {
        Index = index;
    }
}
