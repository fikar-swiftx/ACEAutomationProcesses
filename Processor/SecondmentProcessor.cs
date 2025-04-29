using ACEAutomationProcesses.CWS;
using ACEAutomationProcesses.Database;
using ACEAutomationProcesses.DocManService;
using ACEAutomationProcesses.Model;
using System;
using System.Linq;

namespace ACEAutomationProcesses.Processor
{
    static class SecondmentProcessor
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void ProcessSecondment(SecondmentScenario scenario)
        {
            Log.Debug(
                $"Process Exit Scenario {scenario.ParentAgency}, {scenario.CurrentPrcCode}, {scenario.Id}, {scenario.Seq}, {scenario.FormId} ");

            // AgencyModel parentAgency = DBHelper.GetAgency(scenario.ParentAgency);
            if (scenario.ParentAgency != null)
            {
                Log.Debug($"Parent Agency details {scenario.ParentAgency} retrieved from AgencyDecoder Table");
                ChildrenModel agencyFolder = DbHelper.GetChildByName(Convert.ToInt64(Configuration.EpFileWorkspaceFolder), 0, scenario.ParentAgency);

                if (agencyFolder != null)
                {
                    Log.Debug($"Current Agency folder {agencyFolder.DataId} retrieved.");

                    //check if NRIC folder already exists

                    ChildrenModel nricFolder = DbHelper.GetChildByNameAndAncestor(agencyFolder.DataId, 0, scenario.Id);

                    if (nricFolder == null)
                    {
                        //update fail, NRIC folder doesn't exist
                        DbHelper.UpdateScenarioStatus("Secondment", "Fail", scenario.FormId, scenario.Seq);
                    }
                    else
                    {
                        //nric folder exist, update attributes
                        Log.Debug($"NRIC Folder exist under {scenario.ParentAgency} ({agencyFolder.Name})");
                        Node nricFolderNode = DocumentManagementUtils.GetNode(nricFolder.DataId, null);
                        AttributeGroup[] attributeGroups = nricFolderNode.Metadata.AttributeGroups;
                        string existingId = "";
                        string existingAgency = "";
                        //string prevBorrowingAgency = "";
                        string currBorrowingAgency = "";
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
                                    else if (value.Description.Equals(Configuration.EPfileWorkspaceCurrBorrowingAgencyAttr))
                                    {
                                        StringValue currBorrowingAgencyValue = (StringValue)value;
                                        if (currBorrowingAgencyValue.Values?.Length > 0)
                                            currBorrowingAgency = currBorrowingAgencyValue.Values[0];
                                    }
                                    /*
                                    else if (value.Description.Equals(Configuration.EPfileWorkspacePrevBorrowingAgencyAttr))
                                    {
                                        StringValue prevBorrowingAgencyValue = (StringValue)value;
                                        if (prevBorrowingAgencyValue.Values?.Length > 0)
                                            prevBorrowingAgency = prevBorrowingAgencyValue.Values[0];
                                    }
                                    */
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
                        if (nricFolderNode.Name.Equals(scenario.Id) && scenario.Id.Equals(existingId) && scenario.ParentAgency.Equals(existingAgency))
                        {
                            Log.Debug(
                                $"NRIC Folder exist under {scenario.ParentAgency} ({agencyFolder.Name}) and metadata matches");
                            if (string.IsNullOrEmpty(currBorrowingAgency))
                            {
                                foreach (AttributeGroup attributeGroup in attributeGroups ?? Enumerable.Empty<AttributeGroup>())
                                {

                                    if (attributeGroup.DisplayName.Equals(Configuration.EPfileWorkspaceCategoryName))
                                    {
                                        DataValue[] values = attributeGroup.Values;
                                        foreach (DataValue value in values ?? Enumerable.Empty<DataValue>())
                                        {
                                            if (value.Description.Equals(Configuration.EPfileWorkspaceCurrBorrowingAgencyAttr))
                                            {
                                                StringValue currBorrowingValue = (StringValue)value;
                                                if (currBorrowingValue.Values?.Length > 0)
                                                    currBorrowingValue.Values[0] = scenario.NewBorrowingAgency;
                                                else if (currBorrowingValue.Values == null)
                                                    currBorrowingValue.Values = new[] { scenario.NewBorrowingAgency };
                                                else
                                                    currBorrowingValue.Values.SetValue(scenario.NewBorrowingAgency, 0);
                                                break;
                                            }
                                        }
                                    }

                                }
                            }
                            else if (!currBorrowingAgency.Equals(scenario.NewBorrowingAgency))
                            {
                                foreach (AttributeGroup attributeGroup in attributeGroups ?? Enumerable.Empty<AttributeGroup>())
                                {

                                    if (attributeGroup.DisplayName.Equals(Configuration.EPfileWorkspaceCategoryName))
                                    {
                                        DataValue[] values = attributeGroup.Values;
                                        foreach (DataValue value in values ?? Enumerable.Empty<DataValue>())
                                        {
                                            if (value.Description.Equals(Configuration.EPfileWorkspaceCurrBorrowingAgencyAttr))
                                            {
                                                StringValue currBorrowingValue = (StringValue)value;
                                                if (currBorrowingValue.Values?.Length > 0)
                                                    currBorrowingValue.Values[0] = scenario.NewBorrowingAgency;
                                                else if (currBorrowingValue.Values == null)
                                                    currBorrowingValue.Values = new[] { scenario.NewBorrowingAgency };
                                                else
                                                    currBorrowingValue.Values.SetValue(scenario.NewBorrowingAgency, 0);
                                            }

                                            if (value.Description.Equals(Configuration.EPfileWorkspacePrevBorrowingAgencyAttr))
                                            {
                                                StringValue prevBorrowingAgencyValue = (StringValue)value;
                                                if (prevBorrowingAgencyValue.Values?.Length > 0)
                                                    prevBorrowingAgencyValue.Values[0] = currBorrowingAgency;
                                                else if (prevBorrowingAgencyValue.Values == null)
                                                    prevBorrowingAgencyValue.Values = new[] { currBorrowingAgency };
                                                else
                                                    prevBorrowingAgencyValue.Values.SetValue(currBorrowingAgency, 0);
                                            }
                                        }
                                    }

                                }
                            }
                            else if (scenario.ProcessIndicator.Equals("End Secondment"))
                            {
                                foreach (AttributeGroup attributeGroup in attributeGroups ?? Enumerable.Empty<AttributeGroup>())
                                {

                                    if (attributeGroup.DisplayName.Equals(Configuration.EPfileWorkspaceCategoryName))
                                    {
                                        DataValue[] values = attributeGroup.Values;
                                        foreach (DataValue value in values ?? Enumerable.Empty<DataValue>())
                                        {
                                            if (value.Description.Equals(Configuration.EPfileWorkspaceCurrBorrowingAgencyAttr))
                                            {
                                                // StringValue currBorrowingValue = (StringValue)value;
                                                // currBorrowingValue.Values = null;
                                                /* if (currBorrowingValue.Values?.Count() > 0)
                                                     currBorrowingValue.Values[0] = "";
                                                 else if(currBorrowingValue.Values == null)
                                                 {
                                                     currBorrowingValue.Values 
                                                 }
                                                 else
                                                     currBorrowingValue.Values.SetValue("", 0);*/
                                            }

                                            if (value.Description.Equals(Configuration.EPfileWorkspacePrevBorrowingAgencyAttr))
                                            {
                                                StringValue prevBorrowingAgencyValue = (StringValue)value;
                                                if (prevBorrowingAgencyValue.Values?.Length > 0)
                                                    prevBorrowingAgencyValue.Values[0] = currBorrowingAgency;
                                                else if (prevBorrowingAgencyValue.Values == null)
                                                    prevBorrowingAgencyValue.Values = new[] { currBorrowingAgency };
                                                else
                                                    prevBorrowingAgencyValue.Values.SetValue(currBorrowingAgency, 0);
                                            }
                                        }
                                    }

                                }
                            }
                            nricFolderNode.Metadata.AttributeGroups = attributeGroups;
                            bool nodeSaved = DocumentManagementUtils.SaveNode(nricFolderNode, null);
                            DbHelper.UpdateScenarioStatus("Secondment", nodeSaved ? "Success" : "Fail", scenario.FormId,
                                scenario.Seq);
                        }
                        else
                        {
                            //update fail
                            Log.Error(
                                $"NRIC Folder exists under {agencyFolder.Name} ({agencyFolder.DataId} ) but metadata do not match. Table will be updated with Failed status : {scenario.Seq}");
                            DbHelper.UpdateScenarioStatus("Secondment", "Fail", scenario.FormId, scenario.Seq);
                        }

                    }


                }
                else
                {
                    //update fail
                    Log.Error("Agency Folder is null in scenario, update fail status");
                    DbHelper.UpdateScenarioStatus("Secondment", "Fail", scenario.FormId, scenario.Seq);
                }

            }
            else
            {
                //update fail
                Log.Error("Current Agency is null in scenario, update fail status");
                DbHelper.UpdateScenarioStatus("Secondment", "Fail", scenario.FormId, scenario.Seq);
            }
        }


    }
}
