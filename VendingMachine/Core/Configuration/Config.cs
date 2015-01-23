﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingMachine.Core.Misc;
using VendingMachine.VMDialogs;

namespace VendingMachine.Core.Configuration
{
    public class Config
    {
        public static String APP_DATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static String APP_PATH_MAIN = APP_DATA + @"\VMachine\";
        public static String LOG_FILE_NAME = "VMachine.log";
        public static String CONFIG_FILE_PATH = APP_PATH_MAIN + "VM-config.xml";

        public Config(VMachine vMachine)
        {
            Machine = vMachine;
            ConfigProperties.instance.initDefaultProperties();
        }

        public void readConfigFromFile()
        {
            try
            {
                if (!ConfigManager.configExists())
                {
                    Logger.Log("Not found config file.\nCreating one.");
                    ConfigManager.createNewConfig();
                }
                else
                {
                    ConfigProperties.LoadFromFile(ConfigManager.loadConfigFileOptions());
                }
                Console.WriteLine("-------- Config Start --------");
                foreach (KeyValuePair<int, ConfigProperty> entry in ConfigProperties.instance.Properties)
                {
                    Console.WriteLine(entry.Key + ": " + entry.Value.Name + " = " + entry.Value.Value);
                }
                Console.WriteLine("-------- Config End ----------");
            }
            catch (Exception e)
            {
                 VMDialogManager.ShowExceptionMessage(e);
            }
        }

        public void loadConfigToProgram()
        {
            foreach (ConfigPropertyType prop in Enum.GetValues(typeof(ConfigPropertyType)))
            {
                ConfigProperty property = ConfigProperties.instance.getProperty(prop);
                if (property == null)
                {
                    Logger.Log("There is no proprty " + prop.ToString() + " in properties.");
                    continue;
                }
                switch (property.PropertyType)
                {
                    case ConfigPropertyType.WINDOW_HEIGHT: VMachine.MWindow.Height = Convert.ToInt32(property.Value); break;
                    case ConfigPropertyType.WINDOWS_WIDTH: VMachine.MWindow.Width = Convert.ToInt32(property.Value); break;
                    default: Logger.Log("There is no " + property.PropertyType.ToString() + " property in configuration loader."); break;
                }
            }
        }

        public static VMachine Machine { get; private set; }
    }
}