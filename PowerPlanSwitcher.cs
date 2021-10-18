using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;

namespace PowerPlanSwitcher
{
    public class PowerPlanSwitcher : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private PowerPlanSwitcherSettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("8d0af913-8f5f-4f8c-adef-527be337a619");

        private const string POWERCFG_EXEC = "powercfg.exe";
        private List<Guid> runningGames;

        private void ChangePowerPlan(string powerCfgId)
        {
            string args = "/s " + powerCfgId;
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = POWERCFG_EXEC,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            using (Process process = Process.Start(start))
            {
                process.WaitForExit();
            }
        }

        private void RegisterRunningGame(Game game)
        {
            runningGames.Add(game.Id);
        }

        private void UnregisterRunningGames(Game game)
        {
            if (!runningGames.Remove(game.Id))
            {
                throw new Exception("Game with ID \"" + game.Id + "\" was not previously running.");
            }
        }

        public PowerPlanSwitcher(IPlayniteAPI api) : base(api)
        {
            settings = new PowerPlanSwitcherSettingsViewModel(this);
            runningGames = new List<Guid>();

            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            string gameSource;
            if (args.Game.Source == null)
            {
                gameSource = "";
            }
            else
            {
                gameSource = args.Game.Source.Name;
            }
            string[] ignoredSources = settings.Settings.IgnoredSources.Split(',');
            if (new List<string>(ignoredSources).Any(x => x.Equals(gameSource, StringComparison.InvariantCultureIgnoreCase)))
            {
                // Cancel power plan change.
                return;
            }
            RegisterRunningGame(args.Game);
            ChangePowerPlan(settings.Settings.GamingPowerPlan);
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            string gameSource;
            if (args.Game.Source == null)
            {
                gameSource = "";
            }
            else
            {
                gameSource = args.Game.Source.Name;
            }
            string[] ignoredSources = settings.Settings.IgnoredSources.Split(',');
            if (new List<string>(ignoredSources).Any(x => x.Equals(gameSource, StringComparison.InvariantCultureIgnoreCase)))
            {
                // Cancel power plan change.
                return;
            }
            try
            {
                UnregisterRunningGames(args.Game);
            }
            catch (Exception e)
            {
                PlayniteApi.Dialogs.ShowErrorMessage(e.Message, "Power Plan Switcher");
            }
            if (runningGames.Count == 0)
            {
                ChangePowerPlan(settings.Settings.DefaultPowerPlan);
            }
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new PowerPlanSwitcherSettingsView();
        }
    }
}