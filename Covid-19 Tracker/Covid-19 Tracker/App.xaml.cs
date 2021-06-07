using System;
using System.Resources;
using System.Windows;
using Covid_19_Tracker.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;
using NetFwTypeLib;

namespace Covid_19_Tracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10,
                    fileSizeLimitBytes: 52428800, rollOnFileSizeLimit: true)
                .WriteTo.Telegram(Covid_19_Tracker.Properties.Resources.TelegramApiKey, Covid_19_Tracker.Properties.Resources.TelegramChatId,
                    applicationName: IdentifyComputer.GetIdentification().Result)
                .CreateLogger();
            Log.Information("Application started.");

            try
            {
                var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2") ?? throw new InvalidOperationException());
                var firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule") ?? throw new InvalidOperationException());
                if (firewallRule != null)
                {
                    firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                    firewallRule.Description = "Allows the Covid-19 tracker to access the internet.";
                    firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // inbound
                    firewallRule.Enabled = true;
                    firewallRule.InterfaceTypes = "All";
                    firewallRule.Name = "Covid-19 Tracker";
                    firewallPolicy?.Rules.Add(firewallRule);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Application could not add firewall rule. " + ex.Message);
            }
            using var ctx = new TrackerDbContext();
            ctx.Database.Migrate();
            ctx.SaveChanges();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
