using ACEAutomationProcesses.CWS;
using ACEAutomationProcesses.Database;
using ACEAutomationProcesses.DocManService;
using ACEAutomationProcesses.Model;
using ACEAutomationProcesses.OTCS;
using System;
using System.Linq;

namespace ACEAutomationProcesses.Processor
{
    static class ExitProcessor
    {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void ProcessExit(ExitScenario scenario)
        {
            Log.Debug(
                $"Process Exit Scenario {scenario.Agency}, {scenario.CurrentPrcCode}, {scenario.Id}, {scenario.Seq}, {scenario.FormId}");

            AgencyModel currentAgency = DbHelper.GetAgency(prcCode: scenario.CurrentPrcCode);
            if (currentAgency != null)
            {
                Log.Debug($"Current Agency details {currentAgency.VoicesAgencyCode} retrieved from AgencyDecoder Table");
                ChildrenModel agencyFolder = DbHelper.GetChildByName(Convert.ToInt64(Configuration.EpFileWorkspaceFolder), 0, currentAgency.VoicesAgencyCode);

                if (agencyFolder != null)
                {
                    Log.Debug($"Current Agency folder {agencyFolder.DataId} retrieved.");

                    //check if NRIC folder already exists

                    ChildrenModel nricFolder = DbHelper.GetChildByNameAndAncestor(agencyFolder.DataId, 0, scenario.Id);

                    if (nricFolder == null)
                    {
                        //update fail, NRIC folder doesn't exist
                        DbHelper.UpdateScenarioStatus("Exit", "Fail", scenario.FormId, scenario.Seq);
                    }
                    else
                    {
                        //nric folder exist, update attributes
                        Log.Debug($"NRIC Folder exist under {scenario.Agency} ({agencyFolder.Name})");
                        Node nricFolderNode = DocumentManagementUtils.GetNode(nricFolder.DataId, null);
                        AttributeGroup[] attributeGroups = nricFolderNode.Metadata.AttributeGroups;
                        string existingId = "";
                        string existingAgency = "";
                        Log.Warn($"Categories are missing on {nricFolderNode.Name}:{nricFolderNode.ID}");
                        foreach (AttributeGroup attributeGroup in attributeGroups ?? Enumerable.Empty<AttributeGroup>())
                        {
                            if (attributeGroup.DisplayName.Equals(Configuration.EPfileWorkspaceCategoryName))
                            {
                                DataValue[] values = attributeGroup.Values;
                                foreach (DataValue value in values ?? Enumerable.Empty<DataValue>())
                                {
                                    if (value.Description.Equals(Configuration.EPfileWorkspaceAgencyAttr))
                                    {
                                        StringValue agencyValue = (StringValue)value;
                                        if (agencyValue.Values?.Length > 0)
                                            existingAgency = agencyValue.Values[0];
                                    }
                                }
                            }
                            else if (attributeGroup.DisplayName.Equals(Configuration.EPfileNricfinCategoryName))
                            {
                                DataValue[] values = attributeGroup.Values;
                                foreach (DataValue value in values ?? Enumerable.Empty<DataValue>())
                                {
                                    if (value.Description.Equals(Configuration.EPfileWkspNricAttr))
                                    {
                                        StringValue nricValue = (StringValue)value;
                                        if (nricValue.Values?.Length > 0)
                                            existingId = nricValue.Values[0];
                                    }
                                }
                            }
                        }
                        Log.Debug($"Existing Folder Metadata:  Agency : {existingAgency}  ID : {existingId}");
                        if (nricFolderNode.Name.Equals(scenario.Id) && scenario.Id.Equals(existingId) && scenario.Agency.Equals(existingAgency))
                        {
                            Log.Debug(
                                $"NRIC Folder exist under {scenario.Agency} ({agencyFolder.Name}) and metadata matches");

                            bool success = OtcsHelper.UpdateExitAttributes(nricFolderNode.ID, scenario);
                            if (success)
                            {
                                Log.Debug(
                                    $"NRIC Folder exist under {scenario.Agency} ({agencyFolder.Name}) and metadata matches and successfully updated.");
                                DbHelper.UpdateScenarioStatus("Exit", "Success", scenario.FormId, scenario.Seq);
                            }
                            else
                            {
                                //update fail
                                Log.Error(
                                    $"NRIC Folder exists under {agencyFolder.Name} ({agencyFolder.DataId}) but failed to update metadata.Table will be updated with Failed status : {scenario.Seq}");
                                DbHelper.UpdateScenarioStatus("Exit", "Fail", scenario.FormId, scenario.Seq);
                            }
                        }
                        else
                        {
                            //update fail
                            Log.Error(
                                $"NRIC Folder exists under {agencyFolder.Name} ({agencyFolder.DataId} ) but metadata do not match. Table will be updated with Failed status : {scenario.Seq}");
                            DbHelper.UpdateScenarioStatus("Exit", "Fail", scenario.FormId, scenario.Seq);
                        }

                    }


                }
                else
                {
                    //update fail
                    Log.Error("Agency Folder is null in scenario, update fail status");
                    DbHelper.UpdateScenarioStatus("Exit", "Fail", scenario.FormId, scenario.Seq);
                }

            }
            else
            {
                //update fail
                Log.Error("Current Agency is null in scenario, update fail status");
                DbHelper.UpdateScenarioStatus("Exit", "Fail", scenario.FormId, scenario.Seq);
            }
        }

    }
}
