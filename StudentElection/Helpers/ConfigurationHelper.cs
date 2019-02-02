using Project.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Helpers
{
    public static class ConfigurationHelper
    {
        // Protect the connectionStrings section.
        public static void ProtectConfiguration()
        {
            // Get the section to protect.
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ConfigurationSection connStrings = configFile.ConnectionStrings;
            ConfigurationSection appSettings = configFile.AppSettings;
            var entityFrameworkSection = configFile.Sections["entityFramework"];

            ProtectConfigurationSection(connStrings);
            ProtectConfigurationSection(appSettings);
            ProtectConfigurationSection(entityFrameworkSection);

            configFile.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection(connStrings.SectionInformation.Name);
            ConfigurationManager.RefreshSection(entityFrameworkSection.SectionInformation.Name);

            var roamingLocalConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

            var msAccessUserSettings = roamingLocalConfig.GetSection("userSettings/StudentElection.MSAccess.Properties.Settings");
            var networkUserSettings = roamingLocalConfig.GetSection("userSettings/StudentElection.Properties.Network");

            ProtectConfigurationSection(msAccessUserSettings);
            ProtectConfigurationSection(networkUserSettings);
            
            roamingLocalConfig.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection($"userSettings/{ msAccessUserSettings.SectionInformation.Name }");
            ConfigurationManager.RefreshSection($"userSettings/{ networkUserSettings.SectionInformation.Name }");
        }

        private static bool ProtectConfigurationSection(ConfigurationSection section)
        {
            // Define the Rsa provider name.
            string provider = "RsaProtectedConfigurationProvider";

            if (section != null)
            {
                if (!section.SectionInformation.IsProtected)
                {
                    if (!section.ElementInformation.IsLocked)
                    {
                        // Protect the section.
                        section.SectionInformation.ProtectSection(provider);

                        section.SectionInformation.ForceSave = true;

                        return true;
                    }
                    else
                    {
                        Logger.LogInfo($"Can't protect, section { section.SectionInformation.Name } is locked");
                    }
                }
            }

            return false;
        }
    }
}
