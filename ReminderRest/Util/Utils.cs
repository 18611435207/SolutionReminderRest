using System.Configuration;
using System.Windows.Forms;

namespace ReminderRest.Util
{
    public class Utils
    {
        public static string GetAppSetting(string key, string defaultValue = "")
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;

            if (settings[key] == null)
            {
                settings.Add(key, defaultValue ?? "");
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }

            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public static void ClearAppSettingsValue(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        public static void InitCmb(int maxValue, ComboBox cmb)
        {
            for (int i = 0; i <= maxValue; i++)
            {
                cmb.Items.Add(i.ToString("D2"));
            }
        }
    }


    public enum RestType
    {
        WorkRest = 0,
        NoonBreak = 1,
        Custom = 2
    }

}
