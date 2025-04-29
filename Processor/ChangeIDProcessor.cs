using ACEAutomationProcesses.CWS;
using ACEAutomationProcesses.Database;
using ACEAutomationProcesses.DocManService;
using ACEAutomationProcesses.Model;
using ACEAutomationProcesses.OTCS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACEAutomationProcesses.Processor
{
    static class ChangeIdProcessor
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void ProcessChangeId(ChangeIdScenario scenario)
        {
            Log.Debug(
                $"Process ChangeID Scenario {scenario.Seq}, {scenario.NewId}, {scenario.OldId}, {scenario.FormId}, {scenario.Agency} ");
            //bool nricFolderFound = false;
            //List<AgencyModel> agencies = DbHelper.GetAgency(null, "ACE");
            //String[] agencies = Configuration.AceAgencies.Split(',');
            AgencyModel agency = DbHelper.GetAgency(scenario.Agency);
            //foreach (AgencyModel agency in agencies)
            {
                //AgencyModel currentAgency = DBHelper.GetAgency(agency);
                if (agency != null)
                {
                    Log.Debug($"Current Agency details {agency.VoicesAgencyCode} retrieved from AgencyDecoder Table");
                    ChildrenModel agencyFolder = DbHelper.GetChildByName(Convert.ToInt64(Configuration.EpFileWorkspaceFolder), 0, agency.VoicesAgencyCode);

                    if (agencyFolder != null)
                    {
                        Log.Debug($"Current Agency folder {agencyFolder.DataId} retrieved.");

                        //check if NRIC folder already exists

                        ChildrenModel nricFolder = DbHelper.GetChildByNameAndAncestor(agencyFolder.DataId, 0, scenario.OldId);

                        if (nricFolder != null)
                        {
                            //nric folder exist, update attributes
                            Log.Debug($"NRIC Folder exist under {agency.VoicesAgencyCode} ({agencyFolder.Name})");
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
                            if (nricFolderNode.Name.Equals(scenario.OldId) && scenario.OldId.Equals(existingId) && existingAgency.Equals(agency.VoicesAgencyCode))
                            {
                                Log.Debug(
                                    $"NRIC Folder exist under {agency.VoicesAgencyCode} ({agencyFolder.Name}) and metadata matches");

                                foreach (AttributeGroup attributeGroup in attributeGroups ?? Enumerable.Empty<AttributeGroup>())
                                {
                                    if (attributeGroup.DisplayName.Equals(Configuration.EPfileWorkspaceCategoryName))
                                    {
                                        DataValue[] values = attributeGroup.Values;
                                        foreach (DataValue value in values ?? Enumerable.Empty<DataValue>())
                                        {
                                            if (value.Description.Equals(Configuration.EPfileWorkspaceNewIdEffecDateAttr))
                                            {
                                                DateValue effectiveDateValue = (DateValue)value;
                                                if (effectiveDateValue.Values?.Length > 0)
                                                    effectiveDateValue.Values[0] = scenario.EffectiveDate;
                                                else if (effectiveDateValue.Values == null)
                                                    effectiveDateValue.Values = new[] { scenario.EffectiveDate };
                                                else
                                                    effectiveDateValue.Values.SetValue(scenario.EffectiveDate, 0);

                                            }
                                            else if (value.Description.Equals(Configuration.EPfileWorkspaceOldIdAttr))
                                            {
                                                StringValue oldIdValue = (StringValue)value;
                                                if (oldIdValue.Values?.Length > 0)
                                                    oldIdValue.Values[0] = scenario.OldId;
                                                else if (oldIdValue.Values == null)
                                                    oldIdValue.Values = new[] { scenario.OldId };
                                                else
                                                    oldIdValue.Values.SetValue(scenario.OldId, 0);

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
                                                    nricValue.Values[0] = scenario.NewId;
                                                else if (nricValue.Values == null)
                                                    nricValue.Values = new[] { scenario.NewId };
                                                else
                                                    nricValue.Values.SetValue(scenario.NewId, 0);
                                            }

                                        }
                                    }
                                }
                                nricFolderNode.Metadata.AttributeGroups = attributeGroups;
                                bool success = DocumentManagementUtils.SaveNode(nricFolderNode, null);
                                if (success)
                                {
                                    Log.Debug(
                                        $"NRIC Folder exist under {agency.VoicesAgencyCode} ({agencyFolder.Name}) and metadata matches and successfully updated.");
                                    success = DocumentManagementUtils.RenameNode(nricFolderNode.ID, scenario.NewId, null);
                                    if (success)
                                    {
                                        List<ChildrenModel> documents = DbHelper.GetAllChildrenByTypeAndAncestor(nricFolderNode.ID, 144);

                                        foreach (ChildrenModel document in documents)
                                        {
                                            OtcsHelper.UpdateChangeIdForDocument(document.DataId, scenario);
                                        }
                                        //all documents updated, we will not check for failed status becoz we cant roll back from this stage. If any issue reported, logs to be checked

                                        /*
                                         * Part of code to handle renaming of the ChangeInID Shortcuts
                                         * Code changed on 19th October 2023 
                                         * Developer: Ruchir Dhiman
                                         */

                                        OtcsHelper.renameshortcutNode(nricFolderNode.ID, scenario.NewId);
                                        /*
                                         * Code changes end here
                                         * */

                                        DbHelper.UpdateScenarioStatus("ChangeID", "Success", scenario.FormId, scenario.Seq);
                                    }
                                    else
                                    {
                                        //update fail
                                        DbHelper.UpdateScenarioStatus("ChangeID", "Fail", scenario.FormId, scenario.Seq);
                                    }
                                }
                                else
                                {
                                    //update fail
                                    Log.Error(
                                        $"NRIC Folder exists under {agencyFolder.Name} ({agencyFolder.DataId} ) but failed to update metadata.Table will be updated with Failed status : {scenario.Seq}");
                                    DbHelper.UpdateScenarioStatus("ChangeID", "Fail", scenario.FormId, scenario.Seq);
                                }
                            }
                            else
                            {
                                //update fail
                                Log.Error(
                                    $"NRIC Folder exists under {agencyFolder.Name} ({agencyFolder.DataId} ) but metadata do not match. Table will be updated with Failed status : {scenario.Seq}");
                                DbHelper.UpdateScenarioStatus("ChangeID", "Fail", scenario.FormId, scenario.Seq);
                            }
                        }
                        else
                        {
                            //update fail
                            Log.Error(
                                $"NRIC Folder does not exists under {agencyFolder.Name} ({agencyFolder.DataId} ) ");
                            DbHelper.UpdateScenarioStatus("ChangeID", "Fail", scenario.FormId, scenario.Seq);
                        }

                    }
                    else
                    {
                        //update fail
                        Log.Error("Agency Folder is null in scenario, update fail status");
                        DbHelper.UpdateScenarioStatus("ChangeID", "Fail", scenario.FormId, scenario.Seq);
                    }
                }
                else
                {
                    //update fail
                    Log.Error("Agency is null in scenario, update fail status");
                    DbHelper.UpdateScenarioStatus("ChangeID", "Fail", scenario.FormId, scenario.Seq);
                }
            }
            
        }

    }
}
