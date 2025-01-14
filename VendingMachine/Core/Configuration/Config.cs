﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VendingMachine.Core.Misc;
using VendingMachine.Core.Products;
using VendingMachine.Properties;
using VendingMachine.VMDialogs;

namespace VendingMachine.Core.Configuration
{
    /// <summary>
    /// Main configuration handler
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Path to %appdata% folder
        /// </summary>
        public static String APP_DATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        /// <summary>
        /// Path to program config/log files
        /// </summary>
        public static String APP_PATH_MAIN = APP_DATA + @"\VMachine\";
        /// <summary>
        /// Name of log file
        /// </summary>
        public static String LOG_FILE_NAME = "VMachine.log";
        /// <summary>
        /// Name of config file
        /// </summary>
        public static String CONFIG_FILE_PATH = APP_PATH_MAIN + "VM-config.xml";
        /// <summary>
        /// local resource dictionary VMDictionary
        /// </summary>
        public static ResourceDictionary VMRD;

        /// <summary>
        /// Main configuration handler creator
        /// </summary>
        public Config()
        {
            ConfigProperties.instance.initDefaultProperties();
            VMRD = new ResourceDictionary();
            VMRD.Source =
                new Uri("pack://application:,,,/VendingMachine;component/XAML/VMDictionary.xaml",
                        UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Its invoking all methods of loading configs
        /// </summary>
        public void LoadAllConfigs()
        {
            readConfigFromFile();
            loadConfigToProgram();
            loadProductsToProgram(ConfigFileManager.getProductsFromFile());
        }

        /// <summary>
        /// Check if config file exists. (If not than create one).
        /// Loads configs from file
        /// </summary>
        public void readConfigFromFile()
        {
            try
            {
                if (!ConfigFileManager.configFileExists())
                {
                    Logger.Log("Not found config file.\nCreating one.");
                    ConfigFileManager.createNewConfigFile();
                }
                else
                {
                    ConfigProperties.LoadFromDictionary(ConfigFileManager.getOptionsFromFile());
                }
                printConfig();
            }
            catch (Exception e)
            {
                VMDialogManager.ShowExceptionMessage(e);
                Logger.ExceptionLog(e,"Read config error");
            }
        }

        /// <summary>
        /// Prints config on console to debug
        /// </summary>
        private static void printConfig()
        {
            string cs = "-------- Config Start --------";
            string confs="";
            foreach (KeyValuePair<int, ConfigProperty> entry in ConfigProperties.instance.Properties)
            {
                confs=confs+"\n"+entry.Key + ": " + entry.Value.Name + " = " + entry.Value.Value;
            }
            string ce = "\n-------- Config End ----------";
            Logger.Log(cs+confs+ce);
        }

        /// <summary>
        /// Implements read parameters from file to program
        /// </summary>
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
                switch (prop)
                {
                    case ConfigPropertyType.WINDOW_HEIGHT: VMachine.instance.MWindow.Height = Convert.ToInt32(property.Value); break;
                    case ConfigPropertyType.WINDOWS_WIDTH: VMachine.instance.MWindow.Width = Convert.ToInt32(property.Value); break;
                    case ConfigPropertyType.WINDOW_FULLSCREEN: break;
                    case ConfigPropertyType.MONEY_COLLECTED: break;
                    case ConfigPropertyType.WORKS: if (ConfigProperties.instance.getProperty(prop).Value == "True") VMDialogManager.ShowExceptionMessage(new Exception("Automat zepsuty")); break;
                    case ConfigPropertyType.SERVICE_PASSWD: break;
                    case ConfigPropertyType.SLOTS_COUNT: ProductsController.setupSlots(property.Value); break;
                    case ConfigPropertyType.SLOT_SIZE: break;
                    case ConfigPropertyType.CALL_FOR_REFILL: if (ConfigProperties.instance.getProperty(prop).Value == "True") VMDialogManager.ShowInfoMessage("Obsługa do uzupełnienia wezwana ... chyba"); break;
                    case ConfigPropertyType.ACCOUNT: CoinController.Init(ConfigProperties.instance.getProperty(prop).Value); break;
                    default: Logger.Log("There is no " + property.PropertyType.ToString() + " property in configuration loader switch statement!"); break;
                }
            }
        }

        /// <summary>
        /// Loads products to program product controller
        /// </summary>
        public void loadProductsToProgram(Dictionary<ProductE, ProductData> products)
        {
            foreach (KeyValuePair<ProductE, ProductData> node in products)
            {
                Product product = new Product(node.Value);
                ProductsController.Products.Add(node.Value.Product_ID, product);
            }
            ProductsController.ParseProductsOnView();
        }
    }
}
