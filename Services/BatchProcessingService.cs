using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using log4net;
using ACEAutomationProcesses.Model.CSV;
using ACEAutomationProcesses.Exceptions;

namespace ACEAutomationProcesses.Services
{
    public class BatchProcessingService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BatchProcessingService));
        private const int BatchSize = 1000; // Adjust based on your needs

        public async Task<bool> ProcessCsvInBatches<T>(byte[] fileContent, string fileName, 
            Func<List<T>, Task<bool>> processBatch) where T : class
        {
            try
            {
                using (var byteStream = new MemoryStream(fileContent))
                using (var streamReader = new StreamReader(byteStream))
                using (var csvReader = new CsvReader(streamReader))
                {
                    csvReader.Configuration.HasHeaderRecord = true;
                    var records = csvReader.GetRecords<T>().ToList();
                    
                    Log.Info($"Processing {records.Count} records from {fileName} in batches of {BatchSize}");

                    for (int i = 0; i < records.Count; i += BatchSize)
                    {
                        var batch = records.Skip(i).Take(BatchSize).ToList();
                        Log.Debug($"Processing batch {i / BatchSize + 1} of {(records.Count + BatchSize - 1) / BatchSize}");
                        
                        var success = await processBatch(batch);
                        if (!success)
                        {
                            Log.Error($"Failed to process batch {i / BatchSize + 1}");
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, $"Processing CSV file in batches: {fileName}");
                return false;
            }
        }

        public async Task<bool> ProcessAppointmentCsvInBatches(byte[] fileContent, string fileName)
        {
            return await ProcessCsvInBatches<AppointmentCsv>(fileContent, fileName, async batch =>
            {
                Log.Info($"Processing batch of {batch.Count} appointment records");
                // Implement your batch processing logic here
                return true;
            });
        }

        public async Task<bool> ProcessExitCsvInBatches(byte[] fileContent, string fileName)
        {
            return await ProcessCsvInBatches<ExitCsv>(fileContent, fileName, async batch =>
            {
                Log.Info($"Processing batch of {batch.Count} exit records");
                // Implement your batch processing logic here
                return true;
            });
        }

        public async Task<bool> ProcessChangeMetadataCsvInBatches(byte[] fileContent, string fileName)
        {
            return await ProcessCsvInBatches<ChangeMetadataCsv>(fileContent, fileName, async batch =>
            {
                Log.Info($"Processing batch of {batch.Count} metadata change records");
                // Implement your batch processing logic here
                return true;
            });
        }

        public async Task<bool> ProcessChangeIdCsvInBatches(byte[] fileContent, string fileName)
        {
            return await ProcessCsvInBatches<ChangeIdcsv>(fileContent, fileName, async batch =>
            {
                Log.Info($"Processing batch of {batch.Count} ID change records");
                // Implement your batch processing logic here
                return true;
            });
        }

        public async Task<bool> ProcessSecondmentCsvInBatches(byte[] fileContent, string fileName)
        {
            return await ProcessCsvInBatches<SecondmentCsv>(fileContent, fileName, async batch =>
            {
                Log.Info($"Processing batch of {batch.Count} secondment records");
                // Implement your batch processing logic here
                return true;
            });
        }
    }
} 