using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using log4net;
using ACEAutomationProcesses.Model;
using ACEAutomationProcesses.Model.CSV;
using ACEAutomationProcesses.Exceptions;
using ACEAutomationProcesses.Database;

namespace ACEAutomationProcesses.Services
{
    public class CsvProcessingService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CsvProcessingService));

        public bool ProcessCsv<T>(byte[] fileContent, string fileName, Action<List<T>> processAction) where T : class
        {
            try
            {
                using (var byteStream = new MemoryStream(fileContent))
                using (var streamReader = new StreamReader(byteStream))
                using (var csvReader = new CsvReader(streamReader))
                {
                    csvReader.Configuration.HasHeaderRecord = true;
                    var records = csvReader.GetRecords<T>().ToList();
                    
                    Log.Debug($"Processing {records.Count} records from {fileName}");
                    processAction(records);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, $"Processing CSV file: {fileName}");
                return false;
            }
        }

        public bool ProcessAppointmentCsv(byte[] fileContent, string fileName)
        {
            return ProcessCsv<AppointmentCsv>(fileContent, fileName, records =>
            {
                Log.Info($"Processing {records.Count} appointment records");
                var success = DbHelper.InsertAppointment(records);
                if (!success)
                {
                    throw new Exception("Failed to insert appointment records into database");
                }
            });
        }

        public bool ProcessExitCsv(byte[] fileContent, string fileName)
        {
            return ProcessCsv<ExitCsv>(fileContent, fileName, records =>
            {
                Log.Info($"Processing {records.Count} exit records");
                var success = DbHelper.InsertExit(records);
                if (!success)
                {
                    throw new Exception("Failed to insert exit records into database");
                }
            });
        }

        public bool ProcessChangeMetadataCsv(byte[] fileContent, string fileName)
        {
            return ProcessCsv<ChangeMetadataCsv>(fileContent, fileName, records =>
            {
                Log.Info($"Processing {records.Count} metadata change records");
                var success = DbHelper.InsertChangeMetadata(records);
                if (!success)
                {
                    throw new Exception("Failed to insert metadata change records into database");
                }
            });
        }

        public bool ProcessChangeIdCsv(byte[] fileContent, string fileName)
        {
            return ProcessCsv<ChangeIdcsv>(fileContent, fileName, records =>
            {
                Log.Info($"Processing {records.Count} ID change records");
                var success = DbHelper.InsertChangeId(records);
                if (!success)
                {
                    throw new Exception("Failed to insert ID change records into database");
                }
            });
        }

        public bool ProcessSecondmentCsv(byte[] fileContent, string fileName)
        {
            return ProcessCsv<SecondmentCsv>(fileContent, fileName, records =>
            {
                Log.Info($"Processing {records.Count} secondment records");
                var success = DbHelper.InsertSecondment(records);
                if (!success)
                {
                    throw new Exception("Failed to insert secondment records into database");
                }
            });
        }
    }
} 