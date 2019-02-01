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
        public static void ProtectConfiguration(Configuration config)
        {
            // Get the section to protect.
            ConfigurationSection connStrings = config.ConnectionStrings;
            var msAccessSettings = config.GetSection("StudentElection.MSAccess.Properties.Settings");

            ProtectConfigurationSection(connStrings);
            ProtectConfigurationSection(msAccessSettings);

            config.Save(ConfigurationSaveMode.Full);
        }

        private static void ProtectConfigurationSection(ConfigurationSection section)
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
                    }
                    else
                    {
                        Logger.LogInfo($"Can't protect, section { section.SectionInformation.Name } is locked");
                    }
                }
            }
        }
    }
}
