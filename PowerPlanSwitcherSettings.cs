using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlanSwitcher
{
    public class PowerPlanSwitcherSettings : ObservableObject
    {
        public string DefaultPowerPlan { get; set; } = string.Empty;
        public string GamingPowerPlan { get; set; } = string.Empty;
        public string IgnoredSources { get; set; } = string.Empty; 
    }

    public class PowerPlanSwitcherSettingsViewModel : ObservableObject, ISettings
    {
        private readonly PowerPlanSwitcher plugin;
        private PowerPlanSwitcherSettings editingClone { get; set; }
        private PowerPlanSwitcherSettings settings;

        public PowerPlanSwitcherSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public PowerPlanSwitcherSettingsViewModel(PowerPlanSwitcher plugin)
        {
            this.plugin = plugin;
            var savedSettings = plugin.LoadPluginSettings<PowerPlanSwitcherSettings>();

            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new PowerPlanSwitcherSettings();
            }
        }

        public void BeginEdit()
        {
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            Settings = editingClone;
        }

        public void EndEdit()
        {
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            errors = new List<string>();
            return true;
        }
    }
}