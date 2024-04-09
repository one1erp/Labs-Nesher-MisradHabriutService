using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using LSSERVICEPROVIDERLib;
using System.Reflection;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Runtime.CompilerServices;
using Oracle.DataAccess.Client;
using DAL;
using Common;




namespace MisradHabriutService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>


        static void Main()
        {

            try
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = assemblyPath + ".config";
                Configuration cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                var appSettings = cfg.AppSettings;

                var oraCon = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);

                if (oraCon.State != ConnectionState.Open)
                {
                    oraCon.Open();
                }

                            
                List<string> urlsService = new List<string>();
 
                string sql = "select PE.PHRASE_DESCRIPTION from lims_sys.PHRASE_ENTRY PE inner join lims_sys.PHRASE_HEADER PH on PH.PHRASE_ID = PE.PHRASE_ID where PH.name = 'UrlService_FCS'";
                var cmd = new OracleCommand(sql, oraCon);
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string url = reader["PHRASE_DESCRIPTION"].ToString();
                    urlsService.Add(url);
                }
                ListenerRequests listnRequest = new ListenerRequests(urlsService);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.WriteLogFile("MisradHabriutService:  " + e.Message);

            }


        }
    }
}
