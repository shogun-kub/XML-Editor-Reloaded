using System.Globalization;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Comparison;

/// <summary>
/// Compares record values and root singleton fields of two loaded tables. Records are matched by the key
/// declared in metadata, or by their order when the table declares no key. Only differing records and
/// fields are reported; counts remain exact when detail lists are capped.
/// </summary>
internal static class TableValueComparer
{
    /// <summary>
    /// Compares the values of two loaded tables of the same metadata table.
    /// </summary>
    /// <param name="left">Baseline document.</param>
    /// <param name="right">Candidate document.</param>
    /// <param name="cancellationToken">Token to cancel the comparison.</param>
    /// <returns>Value-level differences, including root singleton fields.</returns>
    public static TableValueComparison Compare(
        TableDocument left,
        TableDocument right,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        var leftCells = TableCellsReader.Read(left);
        var rightCells = TableCellsReader.Read(right);
        var leftGroups = GroupRecords(leftCells.Records, out var leftOrder);
        var rightGroups = GroupRecords(rightCells.Records, out var rightOrder);
        var identities = UnionIdentities(leftOrder, rightOrder);

        var added = 0;
        var removed = 0;
        var changed = 0;
        var changedFields = 0;
        var truncated = false;
        var detailFieldCount = 0;
        var details = new List<RecordComparison>();

        foreach (var identity in identities)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var leftRecords = leftGroups.GetValueOrDefault(identity);
            var rightRecords = rightGroups.GetValueOrDefault(identity);
            var occurrenceCount = Math.Max(leftRecords?.Count ?? 0, rightRecords?.Count ?? 0);
            for (var occurrence = 0; occurrence < occurrenceCount; occurrence++)
            {
                var leftRecord = leftRecords is not null && occurrence < leftRecords.Count
                    ? leftRecords[occurrence]
                    : null;
                var rightRecord = rightRecords is not null && occurrence < rightRecords.Count
                    ? rightRecords[occurrence]
                    : null;
                var display = (leftRecord ?? rightRecord)!.Display;
                if (occurrenceCount > 1)
                {
                    display = string.Concat(
                        display,
                        " #",
                        (occurrence + 1).ToString(CultureInfo.InvariantCulture));
                }

                if (leftRecord is null)
                {
                    added++;
                    TryAddDetail(new RecordComparison(
                        display,
                        -1,
                        rightRecord!.Order,
                        ComparisonDifferenceKind.RightOnly,
                        Array.Empty<RecordFieldChange>()));
                    continue;
                }

                if (rightRecord is null)
                {
                    removed++;
                    TryAddDetail(new RecordComparison(
                        display,
                        leftRecord.Order,
                        -1,
                        ComparisonDifferenceKind.LeftOnly,
                        Array.Empty<RecordFieldChange>()));
                    continue;
                }

                var fields = CompareFieldValues(leftRecord.Fields, rightRecord.Fields);
                if (fields.Count == 0)
                {
                    continue;
                }

                changed++;
                changedFields += fields.Count;
                TryAddDetail(new RecordComparison(
                    display,
                    leftRecord.Order,
                    rightRecord.Order,
                    ComparisonDifferenceKind.Changed,
                    fields));
            }
        }

        return new TableValueComparison(
            leftCells.IdentityKind,
            added,
            removed,
            changed,
            changedFields,
            truncated,
            details,
            CompareFieldValues(leftCells.RootFields, rightCells.RootFields));

        void TryAddDetail(RecordComparison record)
        {
            if (truncated)
            {
                return;
            }

            if (details.Count >= TableComparisonLimits.MaxRecordDetailsPerTable
                || (record.Kind == ComparisonDifferenceKind.Changed
                    && detailFieldCount + record.Fields.Count > TableComparisonLimits.MaxFieldDetailsPerTable))
            {
                truncated = true;
                return;
            }

            details.Add(record);
            if (record.Kind == ComparisonDifferenceKind.Changed)
            {
                detailFieldCount += record.Fields.Count;
            }
        }
    }

    private static Dictionary<string, List<TableRecordCells>> GroupRecords(
        IReadOnlyList<TableRecordCells> records,
        out List<string> order)
    {
        var groups = new Dictionary<string, List<TableRecordCells>>(StringComparer.Ordinal);
        order = new List<string>();
        foreach (var record in records)
        {
            if (!groups.TryGetValue(record.Identity, out var list))
            {
                list = new List<TableRecordCells>(1);
                groups.Add(record.Identity, list);
                order.Add(record.Identity);
            }

            list.Add(record);
        }

        return groups;
    }

    private static List<string> UnionIdentities(
        IReadOnlyList<string> leftOrder,
        IReadOnlyList<string> rightOrder)
    {
        var identities = new List<string>(leftOrder);
        var seen = new HashSet<string>(leftOrder, StringComparer.Ordinal);
        foreach (var identity in rightOrder)
        {
            if (seen.Add(identity))
            {
                identities.Add(identity);
            }
        }

        return identities;
    }

    private static List<RecordFieldChange> CompareFieldValues(
        IReadOnlyList<TableFieldValue> left,
        IReadOnlyList<TableFieldValue> right)
    {
        var leftValues = GroupValues(left, out var leftOrder);
        var rightValues = GroupValues(right, out var rightOrder);
        var changes = new List<RecordFieldChange>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var name in leftOrder)
        {
            if (seen.Add(name))
            {
                AddChange(name, leftValues[name], rightValues.GetValueOrDefault(name), changes);
            }
        }

        foreach (var name in rightOrder)
        {
            if (seen.Add(name))
            {
                AddChange(name, null, rightValues[name], changes);
            }
        }

        return changes;
    }

    private static Dictionary<string, List<string>> GroupValues(
        IReadOnlyList<TableFieldValue> values,
        out List<string> order)
    {
        var groups = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        order = new List<string>();
        foreach (var value in values)
        {
            if (!groups.TryGetValue(value.Name, out var list))
            {
                list = new List<string>(1);
                groups.Add(value.Name, list);
                order.Add(value.Name);
            }

            list.Add(value.Value);
        }

        return groups;
    }

    private static void AddChange(
        string name,
        IReadOnlyList<string>? left,
        IReadOnlyList<string>? right,
        List<RecordFieldChange> changes)
    {
        if (left is null)
        {
            changes.Add(new RecordFieldChange(
                name,
                ComparisonDifferenceKind.RightOnly,
                null,
                JoinValues(right!)));
            return;
        }

        if (right is null)
        {
            changes.Add(new RecordFieldChange(
                name,
                ComparisonDifferenceKind.LeftOnly,
                JoinValues(left),
                null));
            return;
        }

        if (SequenceEqual(left, right))
        {
            return;
        }

        changes.Add(new RecordFieldChange(
            name,
            ComparisonDifferenceKind.Changed,
            JoinValues(left),
            JoinValues(right)));
    }

    private static bool SequenceEqual(IReadOnlyList<string> left, IReadOnlyList<string> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            if (!string.Equals(left[index], right[index], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static string JoinValues(IReadOnlyList<string> values) =>
        ComparisonText.Truncate(string.Join('\n', values));
}
