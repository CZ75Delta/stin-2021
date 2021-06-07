using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var assembly = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (ResourceAssembly.GetName().Name ?? string.Empty) + ".exe");
            Log.Information(assembly);
#if !DEBUG
                var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2") ?? throw new InvalidOperationException());
                var fw = firewallPolicy?.Rules.OfType<INetFwRule>().Where(x => x.Name == "Covid-19 Tracker - OUT TCP" || x.Name == "Covid-19 Tracker - IN TCP" || x.Name == "Covid-19 Tracker - OUT UDP" || x.Name == "Covid-19 Tracker - IN UDP");
                if (fw != null)
                {
                    foreach (var netFwRule in fw)
                    {
                        firewallPolicy.Rules.Remove(netFwRule.Name);
                    }
                }
                try
                {
                    var firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule") ?? throw new InvalidOperationException());
                    if (firewallRule != null)
                    {
                        firewallRule.Name = "Covid-19 Tracker - IN TCP";
                        firewallRule.Description = "Allows the Covid-19 tracker to access the internet.";
                        firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                        firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                        firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                        firewallRule.ApplicationName = assembly;
                        firewallRule.LocalPorts = "80,443";
                        firewallRule.Enabled = true;
                    firewallPolicy?.Rules.Add(firewallRule);
                    }
                    firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule") ?? throw new InvalidOperationException());
                    if (firewallRule != null)
                    {
                        firewallRule.Name = "Covid-19 Tracker - OUT TCP";
                        firewallRule.Description = "Allows the Covid-19 tracker to access the internet.";
                        firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
                        firewallRule.Protocol = (int) NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                        firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                        firewallRule.ApplicationName = assembly;
                        firewallRule.LocalPorts = "80,443";
                        firewallRule.Enabled = true;
                        firewallPolicy?.Rules.Add(firewallRule);
                    }
                    firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule") ?? throw new InvalidOperationException());
                    if (firewallRule != null)
                    {
                        firewallRule.Name = "Covid-19 Tracker - IN UDP";
                        firewallRule.Description = "Allows the Covid-19 tracker to access the internet.";
                        firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                        firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP;
                        firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                        firewallRule.ApplicationName = assembly;
                        firewallRule.LocalPorts = "80,443";
                        firewallRule.Enabled = true;
                    firewallPolicy?.Rules.Add(firewallRule);
                    }
                    firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule") ?? throw new InvalidOperationException());
                    if (firewallRule != null)
                    {
                        firewallRule.Name = "Covid-19 Tracker - OUT UDP";
                        firewallRule.Description = "Allows the Covid-19 tracker to access the internet.";
                        firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
                        firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP;
                        firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                        firewallRule.ApplicationName = assembly;
                        firewallRule.LocalPorts = "80,443";
                        firewallRule.Enabled = true;
                    firewallPolicy?.Rules.Add(firewallRule);
                    }
                Log.Information("Firewall rules added.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Aplikaci se nepodařilo vytvořit vyjímku ve Firewallu. Pokud je aplikace zablokována může se stát, že nebude schopna stáhnout data.", "Covid-19 Tracker", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log.Error("Application could not add firewall rule. " + ex.Message);
                }
#endif

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
