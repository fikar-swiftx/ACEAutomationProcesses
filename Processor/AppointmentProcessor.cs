using System;
using System.Collections.Generic;
using System.Linq;
using ACEAutomationProcesses.CWS;
using ACEAutomationProcesses.Database;
using ACEAutomationProcesses.DocManService;
using ACEAutomationProcesses.Model;
using ACEAutomationProcesses.OTCS;

namespace ACEAutomationProcesses.Processor
{
    static class AppointmentProcessor
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void ProcessAppointment(AppointmentScenario scenario)
        {
            Log.Debug(
                $"Process Appointment Scenario {scenario.Agency}, {scenario.CurrentPrcCode}, {scenario.Id}, {scenario.Seq}, {scenario.FormId} ");

            AgencyModel currentAgency = DbHelper.GetAgency(prcCode: scenario.CurrentPrcCode);
            if (currentAgency != null)
            {
                Log.Debug(
                    $"Current Agency details {currentAgency.VoicesAgencyCode} retrieved from AgencyDecoder Table");
                ChildrenModel agencyFolder =
                    DbHelper.GetChildByName(Convert.ToInt64(Configuration.EpFileWorkspaceFolder), 0,
                        currentAgency.VoicesAgencyCode);

                if (agencyFolder != null)
                {
                    Log.Debug($"Current Agency folder {agencyFolder.DataId} retrieved.");
                    ChildrenModel designationFolder =
                        DbHelper.GetChildByName(agencyFolder.DataId, 0, Configuration.NonHrOfficerFolderName);

                    if (designationFolder != null)
                    {
                        Log.Debug($"Designation folder {designationFolder.Name} {designationFolder.DataId} retrieved.");
                        //check if NRIC folder already exists

                        ChildrenModel nricFolder = DbHelper.GetChildByName(designationFolder.DataId, 0, scenario.Id);

                        if (nricFolder == null)
                        {
                            Log.Debug("NRIC Folder doesn't exist, will create new");
                            ClassificationModel templateClass = null;
                            string agencyTemplateClass = currentAgency.VoicesAgencyCode + "_Template";
                            List<ClassificationModel> classifications =
                                DbHelper.GetClassification(designationFolder.DataId);
                            foreach (ClassificationModel classification in classifications ??
                                                                           Enumerable.Empty<ClassificationModel>())
                            {
                                if (classification.ClassName.Equals("Default Template"))
                                {
                                    Log.Debug("Designation Folder have Default Template classification");
                                    templateClass = classification;
                                    break;
                                }
                                else if (classification.ClassName.Equals(agencyTemplateClass))
                                {
                                    Log.Debug(
                                        $"Designation Folder have Agency Template {agencyTemplateClass} classification");
                                    templateClass = classification;
                                    break;
                                }
                            }

                            if (templateClass != null)
                            {
                                long templateId = DbHelper.GetAgencyTemplate(templateClass.ClassId);
                                Log.Debug($"NRIC Template dataid for classification {templateClass} is {templateId}");
                                Node newNricFolder = DocumentManagementUtils.CopyNode(templateId,
                                    designationFolder.DataId, scenario.Id, null, null);
                                if (newNricFolder != null)
                                {
                                    Log.Debug(
                                        $"NRIC folder created successfully with name {newNricFolder.Name} and id {newNricFolder.ID}");
                                    bool success = PermissionProcessor.RemovedAssignedAccess(newNricFolder);
                                    if (success)
                                    {
                                        success = OtcsHelper.RemoveClassification(newNricFolder.ID,
                                            templateClass.ClassId, "Yes");
                                        if (success)
                                        {
                                            Log.Debug("Classification removed from new NRIC node");
                                            success = OtcsHelper.AddNricFolderCategories(newNricFolder.ID, scenario);
                                            if (success)
                                            {
                                                Log.Debug("Categories added to new NRIC node");
                                                try
                                                {
                                                    PermissionProcessor.ProcessNhrPermissionForAppointment(
                                                        newNricFolder.ID, currentAgency.VoicesAgencyCode);

                                                    PermissionProcessor.ProcessDgsPermission(newNricFolder.ID , scenario.Agency);
 

                                                    Log.Debug(
                                                        "NRIC Folder creation and processing completed successfully");
                                                    DbHelper.UpdateScenarioStatus("Appointment", "Success",
                                                        scenario.FormId, scenario.Seq);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Log.Error(
                                                        $"Exception occurred in permission processing ({ex.Message})",
                                                        ex);
                                                    //update fail.
                                                    DbHelper.UpdateScenarioStatus("Appointment", "Fail",
                                                        scenario.FormId, scenario.Seq);
                                                    DocumentManagementUtils.DeleteNode(newNricFolder.ID, null);
                                                }
                                            }
                                            else
                                            {
                                                //update fail
                                                //Adding workspace categories failed.
                                                Log.Error("Category Update on NRIC Folder failed, update fail status");
                                                DbHelper.UpdateScenarioStatus("Appointment", "Fail", scenario.FormId,
                                                    scenario.Seq);
                                                DocumentManagementUtils.DeleteNode(newNricFolder.ID, null);
                                            }
                                        }
                                        else
                                        {
                                            //update fail
                                            //Removal of template classification from new NRIc Folder failed.
                                            Log.Error(
                                                "Removal of template classification from new NRIC Folder failed, update fail status");
                                            DbHelper.UpdateScenarioStatus("Appointment", "Fail", scenario.FormId,
                                                scenario.Seq);
                                            DocumentManagementUtils.DeleteNode(newNricFolder.ID, null);
                                        }
                                    }
                                    else
                                    {
                                        //update fail
                                        //Removal of assigned access failed.
                                        Log.Error(
                                            "Removal of assigned access for new NRIC Folder failed, update fail status");
                                        DbHelper.UpdateScenarioStatus("Appointment", "Fail", scenario.FormId,
                                            scenario.Seq);
                                        DocumentManagementUtils.DeleteNode(newNricFolder.ID, null);
                                    }
                                }
                                else
                                {
                                    //update fail
                                    //Failed To create NRIC Folder
                                    Log.Error("NRIC Folder creation failed, update fail status");
                                    DbHelper.UpdateScenarioStatus("Appointment", "Fail", scenario.FormId, scenario.Seq);
                                    //DocumentManagementUtils.DeleteNode(newNRICFolder.ID, null);
                                }
                            }
                            else
                            {
                                //template classification not found
                                Log.Error("Template Classification not found, updating fail status");
                                DbHelper.UpdateScenarioStatus("Appointment", "Fail", scenario.FormId, scenario.Seq);
                            }
                        }
                        else
                        {
                            //nric folder exist, update attributes
                            Log.Debug($"NRIC Folder exist under {scenario.Agency} ({designationFolder.Name})");
                            Node nricFolderNode = DocumentManagementUtils.GetNode(nricFolder.DataId, null);
                            AttributeGroup[] attributeGroups = nricFolderNode.Metadata.AttributeGroups;
                            string existingId = "";
                            string existingAgency = "";
                            if (attributeGroups == null)
                                Log.Warn($"Categories are missing on {nricFolderNode.Name}:{nricFolderNode.ID}");
                            foreach (AttributeGroup attributeGroup in attributeGroups ??
                                                                      Enumerable.Empty<AttributeGroup>())
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
                            if (nricFolderNode.Name.Equals(scenario.Id) && scenario.Id.Equals(existingId) &&
                                scenario.Agency.Equals(existingAgency))
                            {
                                Log.Debug(
                                    $"NRIC Folder exist under {scenario.Agency} ({designationFolder.Name}) and metadata matches");

                                bool success = OtcsHelper.UpdateNricFolderCategories(nricFolderNode.ID, scenario);
                                if (success)
                                {
                                    Log.Debug(
                                        $"NRIC Folder exist under {scenario.Agency} ({designationFolder.Name}) and metadata matches and successfully updated.");
                                    DbHelper.UpdateScenarioStatus("Appointment", "Success", scenario.FormId,
                                        scenario.Seq);
                                }
                                else
                                {
                                    //update fail
                                    Log.Error(
                                        $"NRIC Folder exists under {agencyFolder.Name} ({designationFolder.Name} ({designationFolder.DataId})) but failed to update metadata.Table will be updated with Failed status : {scenario.Seq}");
                                    DbHelper.UpdateScenarioStatus("Appointment", "Fail", scenario.FormId, scenario.Seq);
                                }
                            }
                            else
                            {
                                //update fail
                                Log.Error(
                                    $"NRIC Folder exists under {agencyFolder.Name} ({designationFolder.Name} ({designationFolder.DataId})) but metadata do not match. Table will be updated with Failed status : {scenario.Seq}");
                                DbHelper.UpdateScenarioStatus("Appointment", "Fail", scenario.FormId, scenario.Seq);
                            }
                        }
                    }
                    else
                    {
                        //update Fail
                        Log.Error(
                            $"Designation Folder is not found under {agencyFolder.Name} ({agencyFolder.DataId}) ");
                        DbHelper.UpdateScenarioStatus("Appointment", "Fail", scenario.FormId, scenario.Seq);
                    }
                }
                else
                {
                    //update fail
                    Log.Error("Agency Folder is null in scenario, update fail status");
                    DbHelper.UpdateScenarioStatus("Appointment", "Fail", scenario.FormId, scenario.Seq);
                }
            }
            else
            {
                //update fail
                Log.Error("Current Agency is null in scenario, update fail status");
                DbHelper.UpdateScenarioStatus("Appointment", "Fail", scenario.FormId, scenario.Seq);
            }
        }
    }
}