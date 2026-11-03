namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// The automated scenarios and the checklist rows that need a human.
/// </summary>
internal static class ScenarioCatalog
{
    private static readonly Dictionary<string, string> Manual = new(StringComparer.Ordinal)
    {
        ["S04"] = "выбор папки без TableData выполняется через нативный диалог обзора",
        ["S22"] = "визуальная проверка STI-превью: кадр и его пиксели оценивает человек",
        ["S53"] = "двойной щёлчок по заголовку строки требует мыши",
        ["S55"] = "правка и добавление значений списка BarrelConfiguration требуют ввода",
        ["S57"] = "смена ключа симметричной пары требует ввода",
        ["S71"] = "визуальная проверка превью и миниатюры кадра",
        ["S72"] = "визуальная проверка перестроенного списка кадров",
        ["S73"] = "проверка сообщений у поля индекса вместе с отрисовкой превью",
        ["S74"] = "визуальная проверка обновления миниатюр без перезагрузки",
        ["S75"] = "визуальная проверка выбранного кадра после повторного открытия",
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
        new GridFilterScenario(),
        new BulkEditScenario(),
        new RowOperationsScenario(),
        new ColumnLayoutScenario(),
        new SaveDirtyTabScenario(),
        new CloseDirtyTabScenario(),
        new SaveAllAndCloseScenario(),
        new MalformedTableScenario(),
        new ItemWorkspaceScenario(),
        new ItemSearchScenario(),
        new ItemAttachmentsScenario(),
        new ItemFieldEditScenario(),
        new ItemSubTablesScenario(),
        new ItemChildRowsScenario(),
        new ItemNestedScenario(),
        new ItemOverlayScenario(),
        new ItemUntranslatedScenario(),
        new ItemLifecycleScenario(),
        new MercGearScenario(),
        new MercKitsScenario(),
        new MercSlotsScenario(),
        new MercOverlayScenario(),
        new ShopkeeperScenario(),
        new ShopkeeperEditScenario(),
        new ShopkeeperDuplicateScenario(),
        new ShopkeeperRemoveScenario(),
        new ShopkeeperDuplicateEditScenario(),
        new ShopkeeperAddScenario(),
        new ShopkeeperDuplicateRemoveScenario(),
        new LookupRowCrudScenario(),
        new AuxTableScenario(),
        new LookupComboScenario(),
        new LookupEditScenario(),
        new LookupDiagnosticsScenario(),
        new CopyRecordScenario(),
        new CopyRelatedScenario(),
        new CopyPoliciesScenario(),
        new CompareRunScenario(),
        new CompareDetailsScenario(),
        new CompareExportScenario(),
        new SettingsScenario(),
        new UiLanguageScenario(),
        new ResetLayoutsScenario(),
        new ExternalChangeCancelScenario(),
        new ExternalChangeResolveScenario(),
        new MalformedCopyScenario(),
        new AttachableItemsScenario(),
        new AmmunitionSectionScenario(),
        new CompatibleFaceItemsScenario(),
        new DictColumnsScenario(),
        new EnemyAmmoScenario(),
        new RobotSkillGrantScenario(),
        new WeaponSoundPickerScenario(),
        new ExplosiveAnimationScenario(),
        new LbePocketPickerScenario(),
        new ExplosionSoundPickerScenario(),
        new SoundTextScenario(),
        new NoRewriteScenario(),
        new LbeClassLimitScenario(),
        new ExclusiveLauncherFlagsScenario(),
        new MercStatusRangeScenario(),
        new FlagBitEditorScenario(),
        new ValidationUndoScenario(),
        new AboutScenario(),
        new WindowGeometryScenario(),
        new ItemDescriptionCounterScenario(),
        new ItemOverlayCounterScenario(),
        new ItemNameLimitScenario(),
        new MercKitRenameScenario(),
        new UnsavedChangesCloseScenario(),
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
