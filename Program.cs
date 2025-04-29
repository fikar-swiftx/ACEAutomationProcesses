using ACEAutomationProcesses.CWS;
using ACEAutomationProcesses.Database;
using ACEAutomationProcesses.DocManService;
using ACEAutomationProcesses.Model;
using ACEAutomationProcesses.Model.CSV;
using ACEAutomationProcesses.OTCS;
using ACEAutomationProcesses.Processor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace ACEAutomationProcesses
{
    static class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            try
            {
                InitializeSecurity();
                ProcessPendingCsvs();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

        private static void InitializeSecurity()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        private static void ProcessPendingCsvs()
        {
            List<ChildrenModel> pendingCsVs = DbHelper.GetPendingCsv();
            foreach (ChildrenModel csv in pendingCsVs)
            {
                try
                {
                    ProcessSingleCsv(csv);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }
            }
        }

        private static void ProcessSingleCsv(ChildrenModel csv)
        {
            Log.Debug($"Processing {csv.DataId} with name {csv.Name}");
            byte[] fileContent = OtcsHelper.DownloadCsv(csv.DataId);
            if (fileContent == null) return;

            bool success = true;
            // if (csv.Name.StartsWith("FirstApptGreenHarvestingEntryNonCSReAppt"))
            // {
            //     success = _csvProcessingService.ProcessAppointmentCsv(fileContent, csv.Name);
            // }
            // else if (csv.Name.StartsWith("Exit"))
            // {
            //     success = _csvProcessingService.ProcessExitCsv(fileContent, csv.Name);
            // }
            // else if (csv.Name.StartsWith("ChangeNameDivStatSchemeOfSvc"))
            // {
            //     success = _csvProcessingService.ProcessChangeMetadataCsv(fileContent, csv.Name);
            // }
            // else if (csv.Name.StartsWith("ChangeIDfrom"))
            // {
            //     success = _csvProcessingService.ProcessChangeIdCsv(fileContent, csv.Name);
            // }
            // else if (csv.Name.StartsWith("SecondmentandEndSecondment"))
            // {
            //     success = _csvProcessingService.ProcessSecondmentCsv(fileContent, csv.Name);
            // }

            MoveCsvFile(csv.DataId, success);
        }

        private static void MoveCsvFile(long dataId, bool success)
        {
            var targetFolder = success ? Configuration.CsvArchiveFolder : Configuration.CsvErrorFolder;
            var options = new MoveOptions
            {
                AddVersion = false,
                AttrSourceType = AttributeSourceType.ORIGINAL,
                ForceInheritPermissions = false
            };

            try
            {
                DocumentManagementUtils.MoveNode(dataId, Convert.ToInt64(targetFolder), null, null, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }
    }
}
