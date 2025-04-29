using System.Configuration;

namespace ACEAutomationProcesses
{
    static class Configuration
    {
        //subtypes
        public static readonly int TypeFolder = 0;
        public static readonly int TypeDocument = 144;

        public static readonly string Username = ConfigurationManager.AppSettings["otcsUser"];
        public static readonly string Password = ConfigurationManager.AppSettings["otcsPassword"];
        public static readonly string Url = ConfigurationManager.AppSettings["otcsURL"];
        public static readonly string RestReadTimeout = ConfigurationManager.AppSettings["REST_READ_TIMEOUT"];
        public static readonly string RestTimeout = ConfigurationManager.AppSettings["REST_TIMEOUT"];

        public static readonly string DateFormat = ConfigurationManager.AppSettings["CSV_DATE_FORMAT"];

        public static readonly string WrRemoveClass = ConfigurationManager.AppSettings["WR_REMOVE_CLASS"];

        public static readonly string EpFileWorkspaceFolder = ConfigurationManager.AppSettings["EPFILE_WKSP_FOLDER_ID"];

        public static readonly string EPfileNricfinCategory =
            ConfigurationManager.AppSettings["ePfile_NRIC_FIN_CAT_ID"];

        public static readonly string EPfileWorkspaceCategory = ConfigurationManager.AppSettings["ePfile_WKSP_CAT_ID"];

        public static readonly string EPfileNricfinCategoryName =
            ConfigurationManager.AppSettings["ePfile_NRIC_FIN_CAT_Name"];

        public static readonly string EPfileWorkspaceCategoryName =
            ConfigurationManager.AppSettings["ePfile_WKSP_CAT_Name"];

        public static readonly string EPfileWkspNricAttr = ConfigurationManager.AppSettings["ePfile_WKSP_NRIC_ATTR"];

        public static readonly string EPfileWorkspaceAgencyAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_Agency_ATTR"];

        public static readonly string EPfileWkspNameAttr = ConfigurationManager.AppSettings["ePfile_WKSP_Name_ATTR"];

        public static readonly string EPfileWorkspaceDivStatusAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_DIV_STATUS_ATTR"];

        public static readonly string EPfileWorkspaceSchemeOfServiceAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_SOS_ATTR"];

        public static readonly string EPfileWorkspaceStatusAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_STATUS_ATTR"];

        public static readonly string EPfileWorkspacePensionAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_PENSION_ATTR"];

        public static readonly string EPfileWorkspacePernrAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_PERNNR_ATTR"];

        public static readonly string EPfileWorkspaceExitDateAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_EXIT_DATE_ATTR"];

        public static readonly string EPfileWorkspaceNewIdEffecDateAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_NEW_ID_EFFEC_DATE_ATTR"];

        public static readonly string EPfileWorkspaceOldIdAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_OLD_ID_ATTR"];

        public static readonly string EPfileWorkspaceCurrBorrowingAgencyAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_CURR_AGENCY_ATTR"];

        public static readonly string EPfileWorkspacePrevBorrowingAgencyAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_PREV_AGENCY_ATTR"];

        public static readonly string EPfileWorkspacePrevOwnerAgencyAttr =
            ConfigurationManager.AppSettings["ePfile_WKSP_PREV_OWNER_AGENCY_ATTR"];

        public static readonly string NonHrOfficerFolderName =
            ConfigurationManager.AppSettings["NON_HR_OFFICER_FOLDER_NAME"];

        public static string TeFolderName = ConfigurationManager.AppSettings["TE_FOLDER_NAME"];
        public static string NteFolderName = ConfigurationManager.AppSettings["NTE_FOLDER_NAME"];
        public static string PermPrefix1 = ConfigurationManager.AppSettings["PERM_PREFIX_1"];
        public static string PermSuffix1 = ConfigurationManager.AppSettings["PERM_SUFFIX_CONTRIBUTOR"];
        public static string PermSuffix2 = ConfigurationManager.AppSettings["PERM_SUFFIX_READ"];
        public static string PermNte = ConfigurationManager.AppSettings["PERM_NTE_ALIAS"];
        public static string PermFunc = ConfigurationManager.AppSettings["PERM_FUNC"];

        // Value changed by Ruchir in app.config
        public static string ConfigPermName = ConfigurationManager.AppSettings["CONFIG_PERM_NAME"];

        //scenario forms
        public static string AppointmentForm = ConfigurationManager.AppSettings["APPOINTMENT_FORM_DETAILS"];
        public static string ExitForm = ConfigurationManager.AppSettings["EXIT_FORM_DETAILS"];
        public static string ChangeMdForm = ConfigurationManager.AppSettings["CHANGE_METADATA_FORM_DETAILS"];
        public static string ChangeIdForm = ConfigurationManager.AppSettings["CHANGE_ID_FORM_DETAILS"];
        public static string SecondmentForm = ConfigurationManager.AppSettings["SECONDMENT_FORM_DETAILS"];
        public static string PersonnelNumberForm = ConfigurationManager.AppSettings["PERSONNEL_NUMBER_FORM_DETAILS"];

        public static string CsvArchiveFolder = ConfigurationManager.AppSettings["CSV_ARCHIVE_FOLDER"];
        public static string CsvErrorFolder = ConfigurationManager.AppSettings["CSV_ERROR_FOLDER"];


        public static readonly string VitalGeneralCategory = ConfigurationManager.AppSettings["VITAL_GENERAL_CAT_ID"];
        public static readonly string VitalGeneralIdAttr = ConfigurationManager.AppSettings["VITAL_GENERAL_NRIC_ATTR"];

        public static readonly string VitalGeneralPernNrAttr =
            ConfigurationManager.AppSettings["VITAL_GENERAL_PERNNR_ATTR"];

        public static readonly string SqlTimeout = ConfigurationManager.AppSettings["SQL_TIMEOUT"];
        public static readonly string GetChildrenSp = ConfigurationManager.AppSettings["SP_GET_CHILDREN"];
        public static readonly string GetAgencySp = ConfigurationManager.AppSettings["SP_GET_AGENCY"];
        public static string SourceCsvFolder = ConfigurationManager.AppSettings["CSV_SOURCE_FOLDER"];
        public static string GetClassificationSp = ConfigurationManager.AppSettings["SP_GET_CLASSIFICATION"];
        public static string GetAgencyTemplateSp = ConfigurationManager.AppSettings["SP_GET_AGENCY_TEMPLATE"];
        public static string TemplateVolId = ConfigurationManager.AppSettings["TEMPLATE_VOL_ID"];
        public static string GetPendingScenariosSp = ConfigurationManager.AppSettings["GET_PENDING_SCENARIOS_SP"];

        public static string
            UpdateScenariosSp =
                ConfigurationManager.AppSettings["UPDATE_SCENARIOS_SP"]; //sp_updateDGSAutomationScenario

        public static string InsertAppointmentQuery = "INSERT INTO  [Z_ePfile_Appointment] " +
                                                      "([VolumeID],[DataID],[VersionNum],[Seq],[Process_Indicator],[NRIC],[Name],[HRMS_Agency],[iGEMS_Agency__Mapping_logic_],[Serving_Agency]," +
                                                      "[Scheme_of_Service],[Scheme_of_Service_Description],[Divisional_Status],[Division_Text],[Previous_Agency],[iGEMS_Previous_Agency__Mapping_logic_]," +
                                                      "[Serving_Previous_Agency],[Effective_Date],[Process_Status],[Creation_Date]) VALUES (@VolumeID,@DataID,@VersionNumber,@seq,@ProcessIndicator,@NRIC," +
                                                      "@Name,@CurrentPRCCode,@AgencyName,@ServingAgency,@SchemeOfService,@SchemeOfServiceDesc,@DivisionalStatus,@DivisionText,@PreviousPRCCode,@PrevAgency," +
                                                      "@ServingPrevAgency,@EffectiveDate,@Status,GETDATE())";

        public static string InsertExitQuery = "INSERT INTO  [Z_ePfile_Exit] " +
                                               "([VolumeID],[DataID],[VersionNum],[Seq],[Process_Indicator],[NRIC],[Name],[Agency],[iGEMS_Agency__Mapping_logic_],[Serving_Agency]," +
                                               "[Status],[Effective_Date],[Process_Status],[Creation_Date]) VALUES (@VolumeID,@DataID,@VersionNumber,@seq,@ProcessIndicator,@NRIC," +
                                               "@Name,@CurrentPRCCode,@AgencyName,@ServingAgency,@Status,@EffectiveDate,@ProcessStatus,GETDATE())";

        public static string InsertChangeMetadataQuery = "INSERT INTO  [Z_ePfile_MDChanges] " +
                                                         "([VolumeID],[DataID],[VersionNum],[Seq],[Process_Indicator],[NRIC],[FIN],[Name],[Agency],[iGEMS_Agency__Mapping_logic_],[Serving_Agency]," +
                                                         "[Scheme_of_Service],[Scheme_of_Service_Full_Description],[Divisional_Status],[Divison_Text],[Effective_Date],[Process_Status],[Creation_Date])" +
                                                         " VALUES (@VolumeID,@DataID,@VersionNumber,@seq,@ProcessIndicator,@NRIC,@FIN," +
                                                         "@Name,@PRCCode,@Agency,@ServingAgency,@SchemeOfService,@SchemeOfServiceDesc,@DivisionalStatus,@DivisionText," +
                                                         "@EffectiveDate,@ProcessStatus,GETDATE())";

        public static string InsertChangeIdQuery = "INSERT INTO  [Z_ePfile_IDChange] " +
                                                   "([VolumeID],[DataID],[VersionNum],[Seq],[Process_Indicator],[New_NRIC],[Old_NRIC],[Effective_Date],[Process_Status],[Creation_Date],[Agency])" +
                                                   " VALUES (@VolumeID,@DataID,@VersionNumber,@seq,@ProcessIndicator,@NewNRIC,@OldNRIC," +
                                                   "@EffectiveDate,@ProcessStatus,GETDATE(),@Agency)";

        public static string InsertSecondmentQuery = "INSERT INTO  [Z_ePfile_Secondment] " +
                                                     "([VolumeID],[DataID],[VersionNum],[Seq],[Process_Indicator],[NRIC],[Name],[Agency__Parent_],[iGEMS_Agency__Mapping_logic_],[Serving_Agency]," +
                                                     "[Borrowing_Agency],[New_Borrowing_Agency],[iGEMS_New_Borrowing_Agency],[Serving_New_Borrowing_Agency],[Old_Borrowing_Agency]," +
                                                     "[iGEMS_Old_Borrowing_Agency],[Serving_Old_Borrowing_Agency],[Scheme_of_Service],[Scheme_of_Service_Full_Description],[Divisional_Status],[Division_Text]," +
                                                     "[Secondment_Date],[Process_Status],[Creation_Date]) VALUES " +
                                                     "(@VolumeID,@DataID,@VersionNumber,@seq,@ProcessIndicator,@NRIC,@Name," +
                                                     "@ParentPRCCode,@ParentAgencyName,@ServingParentAgency,@BorrowingAgency,@NewBorrowingAgency1,@NewBorrowingAgency2,@ServingNewBorrowingAgency," +
                                                     "@OldBorrowingPRCCode,@OldBorrowingAgency,@ServingOldBorrowingAgency,@SchemeOfService,@SchemeOfServiceDesc,@DivisionalStatus,@DivisionText," +
                                                     "GETDATE(),@Status,GETDATE())";

        /*public static string InsertPernrQuery = "MERGE INTO [Z_NRIC_PERNR] AS Target " +
            "USING (SELECT @Agency as Agency, @PERNR as PERNR, @NRIC as NRIC) AS Source " +
            "ON Target.PERNR = Source.PERNR and Target.Agency = Source.Agency and Target.NRIC = Source.NRIC " +
            "WHEN MATCHED THEN " +
            "UPDATE SET Status = @Status " +
            "WHEN NOT MATCHED THEN " +
            "INSERT ([VolumeID],[DataID],[VersionNum],[Seq],[Agency],[PERNR],[NRIC],[Name],[Status]) " +
            "VALUES (@VolumeID,@DataID,@VersionNumber,@Seq,@Agency,@PERNR,@NRIC,@Name,@Status);";*/
        public static string GetPernrQuery =
            "SELECT * FROM [Z_NRIC_PERNR] WHERE PERNR = @PERNR and Agency = @Agency and NRIC = @NRIC";

        public static string InsertPernrQuery = "INSERT INTO [Z_NRIC_PERNR]" +
                                                "([VolumeID],[DataID],[VersionNum],[Seq],[Agency],[PERNR],[NRIC],[Name],[Status],[Interface_Date],[Effective_Date]) " +
                                                "VALUES " +
                                                "(@VolumeID,@DataID,@VersionNumber,@Seq,@Agency,@PERNR,@NRIC,@Name,@Status,@InterfaceDate,@EffectiveDate)";

        public static string UpdateStatusPernrQuery1 =
            "UPDATE [Z_NRIC_PERNR] SET [Status] = @Status WHERE [NRIC] = @NRIC and [Agency] = @Agency and [PERNR] = @PERNR";

        public static string UpdateStatusPernrQuery2 =
            "UPDATE [Z_NRIC_PERNR] SET [Status] = @Status WHERE [NRIC] = @NRIC and [Agency] = @Agency";

        public static string UpdateNamePernrQuery =
            "UPDATE [Z_NRIC_PERNR] SET [Name] = @Name WHERE [NRIC] = @NRIC and [Agency] = @Agency";

        public static string UpdateIdPernrQuery =
            "UPDATE [Z_NRIC_PERNR] SET [NRIC] = @NewNRIC WHERE [NRIC] = @OldNRIC and [Agency] = @Agency";
        //public static string SeqQuery = "select DataID, VolumeID, Seq+1 as \"Seq\", VersionNum  from @Table where seq in (select max(seq) from @Table)";

        /*
         public static string SeqQuery = "select A.DataID, A.VolumeID, A.Seq, A.VersionNum, B.Seq as \"Seq2\" from " +
            "(select * from @Table where seq in (select max(seq) from @Table)) A " +
            "inner join FormsSequence B " +
            "on A.DataID  = B.DataID and A.VolumeID = B.VolumeID";
        */
        //public static string UpdateSeqQuery1 = "UPDATE FormsSequence SET Seq = @NewSeq WHERE VolumeID = @VolumeID AND DataID = @DataID AND VersionNum = @VersionNum AND Seq = @Seq";
        //public static string UpdateSeqQuery2 = "UPDATE FormsSequence SET Seq = @NewSeq WHERE VolumeID = @VolumeID AND DataID = @DataID AND VersionNum = @VersionNum AND Seq < @Seq";
        public static string TableSeqQuery =
            "select max(seq) as\"Seq\" from @Table where DataID = @DataID and VolumeID = @VolumeID and VersionNum = @VersionNum";

        public static string InsertSeqQuery =
            "INSERT INTO FormsSequence ( VolumeID, DataID, VersionNum, Seq ) VALUES (@VolumeID, @DataID, @VersionNum, @NewSeq)";

        public static string SeqQuery =
            "select Seq from FormsSequence where DataID = @DataID and VolumeID = @VolumeID and VersionNum = @VersionNum";

        public static string UpdateSeqQuery =
            "UPDATE FormsSequence SET Seq = @NewSeq WHERE VolumeID = @VolumeID AND DataID = @DataID AND VersionNum = @VersionNum AND Seq = @Seq";

        public static string GetMemberQuery = "select ID, Name, Type from KUAF where Deleted = 0 and Name = @name";

        public static string GetCatQuery =
            "select CatID,CatName,AttrName,RegionName from CatRegionMap where CatID = @catId";

        public static string GetFolderAliasQuery =
            "select folderName,folderAlias,Folder_Level,ExtraPermission from ePfile_permFoldersAliases";

        public static string GetDocumentNricQuery =
            "select dbo.getCatAttrValueMulti('VITAL-General','NRIC/ FIN',@DataID,2) nric";

        public static readonly string WR_Main_Appointment_Epfile_Creation_Status = ConfigurationManager.AppSettings["WR_Main_Appointment_Epfile_Creation_Status"];
        public static readonly string WR_Main_IDChange_Change_Performed_Status = ConfigurationManager.AppSettings["WR_Main_IDChange_Change_Performed_Status"];

        public static string GetAppointmentCumulusBatch = "SELECT * FROM Z_ePfile_Appointment WHERE iGEMS_Agency__Mapping_logic_ IN (SELECT Voices_Agency_Code FROM AgencyDecoder WHERE AgencySource = 'ACE' AND Voices_Agency_Code <> 'PUB') AND Cumulus_Batch_Status = 'Processing'";

        public static string GetIDChangeCumulusBatch = "SELECT * FROM Z_ePfile_IDChange WHERE Agency <> 807 AND Cumulus_Batch_Status = 'Processing'";
    }
}