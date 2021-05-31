using Playnite.SDK;
using Playnite.SDK.Models;
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
    public class PowerPlanSwitcher : Plugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private PowerPlanSwitcherSettings settings { get; set; }

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
            settings = new PowerPlanSwitcherSettings(this);
            runningGames = new List<Guid>();
        }

        public override void OnGameInstalled(Game game)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(Game game)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(Game game)
        {
            string gameSource;
            if (game.Source == null)
            {
                gameSource = "";
            }
            else
            {
                gameSource = game.Source.Name;
            }
            string[] ignoredSources = settings.IgnoredSources.Split(',');
            if (new List<string>(ignoredSources).Any(x => x.Equals(gameSource, StringComparison.InvariantCultureIgnoreCase)))
            {
                // Cancel power plan change.
                return;
            }
            RegisterRunningGame(game);
            ChangePowerPlan(settings.GamingPowerPlan);
        }

        public override void OnGameStopped(Game game, long elapsedSeconds)
        {
            string gameSource;
            if (game.Source == null)
            {
                gameSource = "";
            }
            else
            {
                gameSource = game.Source.Name;
            }
            string[] ignoredSources = settings.IgnoredSources.Split(',');
            if (new List<string>(ignoredSources).Any(x => x.Equals(gameSource, StringComparison.InvariantCultureIgnoreCase)))
            {
                // Cancel power plan change.
                return;
            }
            try
            {
                UnregisterRunningGames(game);
            }
            catch (Exception e)
            {
                PlayniteApi.Dialogs.ShowErrorMessage(e.Message, "Power Plan Switcher");
            }
            if (runningGames.Count == 0)
            {
                ChangePowerPlan(settings.DefaultPowerPlan);
            }
        }

        public override void OnGameUninstalled(Game game)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted()
        {
            // Add code to be executed when Playnite is initialized.
        }

        public override void OnApplicationStopped()
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated()
        {
            // Add code to be executed when library is updated.
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