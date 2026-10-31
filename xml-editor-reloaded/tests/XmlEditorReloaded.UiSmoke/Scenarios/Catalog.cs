namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// The automated scenarios and the checklist rows that need a human.
/// </summary>
internal static class ScenarioCatalog
{
    private static readonly Dictionary<string, string> Manual = new(StringComparer.Ordinal)
    {
        ["S22"] = "визуальная проверка STI-превью: кадр и его пиксели оценивает человек",
        ["S71"] = "визуальная проверка превью и миниатюры кадра",
        ["S72"] = "визуальная проверка перестроенного списка кадров",
        ["S73"] = "проверка сообщений у поля индекса вместе с отрисовкой превью",
        ["S74"] = "визуальная проверка обновления миниатюр без перезагрузки",
        ["S75"] = "визуальная проверка выбранного кадра после повторного открытия",
        ["S82"] = "геометрия окна проверяется между запусками и на реальном рабочем столе",
        ["S83"] = "поведение при отключении монитора и испорченных настройках требует ручных действий",
    };

    private static readonly SmokeScenario[] Scenarios =
    [
        new ColdStartScenario(),
        new DatasetListScenario(),
        new DatasetSwitchScenario(),
        new OpenGenericTableScenario(),
        new TableDetailsScenario(),
        new GridSearchScenario(),
        new SaveDirtyTabScenario(),
        new CloseDirtyTabScenario(),
        new SaveAllAndCloseScenario(),
        new MalformedTableScenario(),
    ];

    /// <summary>
    /// Gets every automated scenario.
    /// </summary>
    public static IReadOnlyList<SmokeScenario> All => Scenarios;

    /// <summary>
    /// Determines whether a checklist row needs a human.
    /// </summary>
    /// <param name="id">Scenario identifier.</param>
    /// <returns><see langword="true"/> for a manual scenario.</returns>
    public static bool IsManual(string id) => Manual.ContainsKey(id);

    /// <summary>
    /// Returns the reason a checklist row needs a human.
    /// </summary>
    /// <param name="id">Scenario identifier.</param>
    /// <returns>Reason text, or an empty string.</returns>
    public static string ManualReason(string id) => Manual.TryGetValue(id, out var reason) ? reason : string.Empty;
}
