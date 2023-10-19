public static class Prefs
{
    public static bool IsVibratorOn
    {
        get => CPlayerPrefs.GetBool(PrefConstants.VIBRATOR_STATE, true);

        set => CPlayerPrefs.SetBool(PrefConstants.VIBRATOR_STATE, value);
    }

    public static bool IsSoundOn
    {
        get => CPlayerPrefs.GetBool(PrefConstants.SOUND_STATE, true);

        set => CPlayerPrefs.SetBool(PrefConstants.SOUND_STATE, value);
    }

    public static bool IsRemoveAds
    {
        get => CPlayerPrefs.GetBool(PrefConstants.ADS_REMOVE, false);

        set => CPlayerPrefs.SetBool(PrefConstants.ADS_REMOVE, value);
    }

    public static int MoneyValue
    {
        get => CPlayerPrefs.GetInt(PrefConstants.MONEY, 0);
        set => CPlayerPrefs.SetInt(PrefConstants.MONEY, value);
    }

    public static void DeleteAllData()
    {
        CPlayerPrefs.DeleteAll();
    }

    public static int CurrentLevel
    {
        get => CPlayerPrefs.GetInt(PrefConstants.CURRENT_LEVEL, 1);
        set => CPlayerPrefs.SetInt(PrefConstants.CURRENT_LEVEL, value);
    }

    public static int LevelPrefab
    {
        get => CPlayerPrefs.GetInt(PrefConstants.LEVEL_PREFAB, 1);
        set => CPlayerPrefs.SetInt(PrefConstants.LEVEL_PREFAB, value);
    }

    public static int MoneyGetInWave
	{
        get => CPlayerPrefs.GetInt(PrefConstants.MONEY_GET_IN_WAVE, 0);
        set => CPlayerPrefs.SetInt(PrefConstants.MONEY_GET_IN_WAVE, value);
	}

    public static int MonetGetLastWave
	{
        get => CPlayerPrefs.GetInt(PrefConstants.MONEY_GET_LAST_WAVE, 0);
        set => CPlayerPrefs.SetInt(PrefConstants.MONEY_GET_LAST_WAVE, value);
    }
}