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

namespace ACEAutomationProcesses
{
    static class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // ReSharper disable once UnusedParameter.Local
        
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            List<ChildrenModel> pendingCsVs = DbHelper.GetPendingCsv();
            foreach (ChildrenModel csv in pendingCsVs)
            {
                try
                {
                    Log.Debug($"Processing {csv.DataId} with name {csv.Name} ");
                    byte[] fileContent = OtcsHelper.DownloadCsv(csv.DataId);
                    if (fileContent == null) continue;


                    Stream byteStream = new MemoryStream(fileContent);
                    StreamReader streamReader = new StreamReader(byteStream);
                    CsvHelper.CsvReader csvReader = new CsvHelper.CsvReader(streamReader);
                    if (csv.Name.StartsWith("FirstApptGreenHarvestingEntryNonCSReAppt"))
                    {
                        Log.Info($"Process Appointment CSV {csv.DataId} with name {csv.Name} ");
                        csvReader.Configuration.HasHeaderRecord = true;
                        List<AppointmentCsv> appointments = csvReader.GetRecords<AppointmentCsv>().ToList();
                        Log.Debug("Total Records in CSV " + appointments.Count);
                        Boolean success = DbHelper.InsertAppointment(appointments);
                        if (success)
                        {
                            //move file to archive Folder
                            MoveOptions options = new MoveOptions
                            {
                                AddVersion = false,
                                AttrSourceType = AttributeSourceType.ORIGINAL,
                                ForceInheritPermissions = false
                            };
                            DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvArchiveFolder), null, null, options);

                        }
                        else
                        {
                            //move file to error folder
                            MoveOptions options = new MoveOptions
                            {
                                AddVersion = false,
                                AttrSourceType = AttributeSourceType.ORIGINAL,
                                ForceInheritPermissions = false
                            };
                            DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvErrorFolder), null, null, options);

                        }

                    }

                    //Exit Scenario
                    if (csv.Name.StartsWith("Exit"))
                    {
                        Log.Info($"Process Exit CSV {csv.DataId} with name {csv.Name} ");
                        csvReader.Configuration.HasHeaderRecord = true;
                        List<ExitCsv> exits = csvReader.GetRecords<ExitCsv>().ToList();
                        Log.Debug("Total Records in CSV " + exits.Count);
                        Boolean success = DbHelper.InsertExit(exits);
                        if (success)
                        {
                            //move file to archive Folder
                            MoveOptions options = new MoveOptions
                            {
                                AddVersion = false,
                                AttrSourceType = AttributeSourceType.ORIGINAL,
                                ForceInheritPermissions = false
                            };
                            DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvArchiveFolder), null, null, options);

                        }
                        else
                        {
                            //move file to error folder
                            MoveOptions options = new MoveOptions
                            {
                                AddVersion = false,
                                AttrSourceType = AttributeSourceType.ORIGINAL,
                                ForceInheritPermissions = false
                            };
                            DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvErrorFolder), null, null, options);

                        }
                    }


                    //Change of metadata
                    if (csv.Name.StartsWith("ChangeNameDivStatSchemeOfSvc"))
                    {
                        Log.Info($"Process ChangeNameDivStatSchemeOfSvc CSV {csv.DataId} with name {csv.Name} ");
                        csvReader.Configuration.HasHeaderRecord = true;
                        List<ChangeMetadataCsv> changeMetadataCsVs = csvReader.GetRecords<ChangeMetadataCsv>().ToList();
                        Log.Debug("Total Records in CSV " + changeMetadataCsVs.Count);
                        Boolean success = DbHelper.InsertChangeMetadata(changeMetadataCsVs);
                        if (success)
                        {
                            //move file to archive Folder
                            MoveOptions options = new MoveOptions
                            {
                                AddVersion = false,
                                AttrSourceType = AttributeSourceType.ORIGINAL,
                                ForceInheritPermissions = false
                            };
                            DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvArchiveFolder), null, null, options);

                        }
                        else
                        {
                            //move file to error folder
                            MoveOptions options = new MoveOptions
                            {
                                AddVersion = false,
                                AttrSourceType = AttributeSourceType.ORIGINAL,
                                ForceInheritPermissions = false
                            };
                            DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvErrorFolder), null, null, options);

                        }

                    }

                    //Change of ID
                    if (csv.Name.StartsWith("ChangeIDfrom"))
                    {
                        Log.Info($"Process ChangeID CSV {csv.DataId} with name {csv.Name} ");
                        csvReader.Configuration.HasHeaderRecord = true;
                        List<ChangeIdcsv> changeIdCsVs = csvReader.GetRecords<ChangeIdcsv>().ToList();
                        Log.Debug("Total Records in CSV " + changeIdCsVs.Count);
                        Boolean success = DbHelper.InsertChangeId(changeIdCsVs);
                        if (success)
                        {
                            //move file to archive Folder
                            MoveOptions options = new MoveOptions
                            {
                                AddVersion = false,
                                AttrSourceType = AttributeSourceType.ORIGINAL,
                                ForceInheritPermissions = false
                            };
                            DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvArchiveFolder), null, null, options);

                        }
                        else
                        {
                            //move file to error folder
                            MoveOptions options = new MoveOptions
                            {
                                AddVersion = false,
                                AttrSourceType = AttributeSourceType.ORIGINAL,
                                ForceInheritPermissions = false
                            };
                            DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvErrorFolder), null, null, options);

                        }

                    }


                    //Secondment
                    if (csv.Name.StartsWith("SecondmentandEndSecondment"))
                    {
                        Log.Info($"Process Secondment CSV {csv.DataId} with name {csv.Name} ");
                        csvReader.Configuration.HasHeaderRecord = true;
                        List<SecondmentCsv> secondmentCsVs = csvReader.GetRecords<SecondmentCsv>().ToList();
                        Log.Debug("Total Records in CSV " + secondmentCsVs.Count);
                        Boolean success = DbHelper.InsertSecondment(secondmentCsVs);
                        if (success)
                        {
                            //move file to archive Folder
                            MoveOptions options = new MoveOptions
                            {
                                AddVersion = false,
                                AttrSourceType = AttributeSourceType.ORIGINAL,
                                ForceInheritPermissions = false
                            };
                            DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvArchiveFolder), null, null, options);

                        }
                        else
                        {
                            //move file to error folder
                            MoveOptions options = new MoveOptions
                            {
                                AddVersion = false,
                                AttrSourceType = AttributeSourceType.ORIGINAL,
                                ForceInheritPermissions = false
                            };
                            DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvErrorFolder), null, null, options);

                        }

                    }

                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                    //move file to error folder
                    MoveOptions options = new MoveOptions
                    {
                        AddVersion = false,
                        AttrSourceType = AttributeSourceType.ORIGINAL,
                        ForceInheritPermissions = false
                    };
                    DocumentManagementUtils.MoveNode(csv.DataId, Convert.ToInt64(Configuration.CsvErrorFolder), null, null, options);
                }
            }

            ProcessScenarios();
            //Generate Post Report by calling WRs after automation job done
            if (DbHelper.CheckIfAppointmentCumulusBatchExists())
            {
                Log.Info("Generating Post Report for Appointment Cumulus Batch!");
                OtcsHelper.PostReportGeneration(Configuration.WR_Main_Appointment_Epfile_Creation_Status);
            }
            if (DbHelper.CheckIfIDChangeCumulusBatchExists())
            {
                Log.Info("Generating Post Report for ID Change Cumulus Batch!");
                OtcsHelper.PostReportGeneration(Configuration.WR_Main_IDChange_Change_Performed_Status);
            }
        }
    

        private static void ProcessScenarios()
        {
            try
            {
                Log.Debug("Fetch Appointment records from table");
                List<AppointmentScenario> scenarios = DbHelper.GetPendingAppointmentScenarios();
                Log.Debug($"Total Appointment scenarios to be processed : {scenarios.Count}");
                foreach (AppointmentScenario scenario in scenarios)
                {
                    try
                    {
                        AppointmentProcessor.ProcessAppointment(scenario);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            //change in ID
            try
            {
                Log.Debug("Fetch ChangeInID records from table");
                List<ChangeIdScenario> scenarios = DbHelper.GetPendingChangeIdScenarios();
                Log.Debug($"Total Change ID scenarios to be processed : {scenarios.Count}");
                foreach (ChangeIdScenario scenario in scenarios)
                {
                    try
                    {
                        ChangeIdProcessor.ProcessChangeId(scenario);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            //secondment
            try
            {
                Log.Debug("Fetch Secondment records from table");
                List<SecondmentScenario> scenarios = DbHelper.GetPendingSecondmentScenarios();
                Log.Debug($"Total Secondment scenarios to be processed : {scenarios.Count}");
                foreach (SecondmentScenario scenario in scenarios)
                {
                    try
                    {
                        SecondmentProcessor.ProcessSecondment(scenario);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            
            //change in MD
            try
            {
                Log.Debug("Fetch ChangeInMD records from table");
                List<ChangeMdScenario> scenarios = DbHelper.GetPendingChangeinMdScenarios();
                Log.Debug($"Total Change MD scenarios to be processed : {scenarios.Count}");
                foreach (ChangeMdScenario scenario in scenarios)
                {
                    try
                    {
                        ChangeMdProcessor.ProcessChangeMd(scenario);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            //exit
            try
            {
                Log.Debug("Fetch Exit records from table");
                List<ExitScenario> scenarios = DbHelper.GetPendingExitScenarios();
                Log.Debug($"Total Exit scenarios to be processed : {scenarios.Count}");
                foreach (ExitScenario scenario in scenarios)
                {
                    try
                    {
                        ExitProcessor.ProcessExit(scenario);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }




        }


    }
}
