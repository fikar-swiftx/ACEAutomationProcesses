using ACEAutomationProcesses.Model;
using ACEAutomationProcesses.Model.CSV;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace ACEAutomationProcesses.Database
{
    static class DbHelper
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<ChildrenModel> GetPendingCsv()
        {
            Log.Info(
                $"Get Pending Scenario CSV DATAID using SP {Configuration.GetChildrenSp} under folder {Configuration.SourceCsvFolder}:: ");
            List<ChildrenModel> list = new List<ChildrenModel>();

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetChildrenSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@parentID", SqlDbType.VarChar).Value = Configuration.SourceCsvFolder;
                        sqlCommand.Parameters.Add("@type", SqlDbType.Int).Value = 144;
                        sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ChildrenModel model = new ChildrenModel();
                                long dataId = DBNull.Value.Equals(rdr["DATAID"]) ? 0L : Convert.ToInt64(rdr["DATAID"]);
                                string name = DBNull.Value.Equals(rdr["NAME"]) ? "" : rdr["NAME"].ToString().Trim();
                                Log.Debug("Scenario ID : " + dataId + " Name : " + name);
                                model.DataId = dataId;
                                model.Name = name;
                                model.Subtype = 144;
                                list.Add(model);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving pending CSVs.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            Log.Debug("return from getPendingScenarios : " + list.Count);
            return list;
        }


        public static List<ChildrenModel> GetChildren(long parentId, int type)
        {
            Log.Info($"Get All children under {Configuration.GetChildrenSp} using SP {parentId} of type {type}");
            List<ChildrenModel> list = new List<ChildrenModel>();

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetChildrenSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@parentID", SqlDbType.VarChar).Value = parentId;
                        if (type != -1)
                        {
                            sqlCommand.Parameters.Add("@type", SqlDbType.Int).Value = type;
                        }
                        //sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ChildrenModel model = new ChildrenModel
                                {
                                    DataId =
                                        DBNull.Value.Equals(rdr["DATAID"]) ? 0L : Convert.ToInt64(rdr["DATAID"]),
                                    Name = DBNull.Value.Equals(rdr["NAME"]) ? "" : rdr["NAME"].ToString().Trim(),
                                    Subtype = DBNull.Value.Equals(rdr["SUBTYPE"])
                                        ? -1
                                        : Convert.ToInt16(rdr["SUBTYPE"])
                                };
                                // model.dataId = dataID;
                                //model.name = name;                       
                                list.Add(model);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving children.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            Log.Debug("return from getChildren : " + list.Count);
            return list;
        }

        public static ChildrenModel GetChildByName(long parentId, int type, string name)
        {
            Log.Info(
                $"getChild by name {name} of type {type} under parent {parentId} using {Configuration.GetChildrenSp} ");
            ChildrenModel model = null;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetChildrenSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@parentID", SqlDbType.VarChar).Value = parentId;
                        if (type != -1)
                        {
                            sqlCommand.Parameters.Add("@type", SqlDbType.Int).Value = type;
                        }
                        sqlCommand.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
                        // sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                model = new ChildrenModel
                                {
                                    DataId =
                                        DBNull.Value.Equals(rdr["DATAID"]) ? 0L : Convert.ToInt64(rdr["DATAID"]),
                                    Name = DBNull.Value.Equals(rdr["NAME"]) ? "" : rdr["NAME"].ToString().Trim(),
                                    Subtype = DBNull.Value.Equals(rdr["SUBTYPE"])
                                        ? -1
                                        : Convert.ToInt16(rdr["SUBTYPE"])
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving child by name.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            if (model == null)
            {
                Log.Debug("Return from getChildByName : false");
            }
            else
            {
                Log.Debug("Return from getChildByName : " + model.DataId);
            }
            return model;
        }

        public static List<ChildrenModel> GetAllChildrenByTypeAndAncestor(long parentId, int type)
        {
            Log.Info($"getAllChildren by type {type} under parent {parentId} using {Configuration.GetChildrenSp} ");
            List<ChildrenModel> list = new List<ChildrenModel>();
            //ChildrenModel model = null;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetChildrenSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@parentID", SqlDbType.VarChar).Value = parentId;
                        if (type != -1)
                        {
                            sqlCommand.Parameters.Add("@type", SqlDbType.Int).Value = type;
                        }
                        sqlCommand.Parameters.Add("@subfolder", SqlDbType.VarChar).Value = "1";
                        sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ChildrenModel model = new ChildrenModel
                                {
                                    DataId =
                                        DBNull.Value.Equals(rdr["DATAID"]) ? 0L : Convert.ToInt64(rdr["DATAID"]),
                                    Name = DBNull.Value.Equals(rdr["NAME"]) ? "" : rdr["NAME"].ToString().Trim(),
                                    Subtype = DBNull.Value.Equals(rdr["SUBTYPE"])
                                        ? -1
                                        : Convert.ToInt16(rdr["SUBTYPE"])
                                };
                                list.Add(model);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving all children by type and ancestor.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            Log.Debug("Return from getAllChildren : " + list.Count);

            return list;
        }


        public static ChildrenModel GetChildByNameAndAncestor(long ancestorId, int type, string name)
        {
            Log.Info(
                $"getChild by NameAndAncestor {name} of type {type} under parent {ancestorId} using {Configuration.GetChildrenSp} ");
            ChildrenModel model = null;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetChildrenSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@parentID", SqlDbType.VarChar).Value = ancestorId;
                        sqlCommand.Parameters.Add("@subfolder", SqlDbType.VarChar).Value = "1";
                        if (type != -1)
                        {
                            sqlCommand.Parameters.Add("@type", SqlDbType.Int).Value = type;
                        }
                        sqlCommand.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
                        sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                model = new ChildrenModel
                                {
                                    DataId =
                                        DBNull.Value.Equals(rdr["DATAID"]) ? 0L : Convert.ToInt64(rdr["DATAID"]),
                                    Name = DBNull.Value.Equals(rdr["NAME"]) ? "" : rdr["NAME"].ToString().Trim(),
                                    Subtype = DBNull.Value.Equals(rdr["SUBTYPE"])
                                        ? -1
                                        : Convert.ToInt16(rdr["SUBTYPE"])
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving child by type, name and ancestor.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            if (model == null)
            {
                Log.Debug("Return from GetChildByNameAndAncestor : false");
            }
            else
            {
                Log.Debug("Return from GetChildByNameAndAncestor : " + model.DataId);
            }

            return model;
        }

        public static MemberModel GetMemberByName(string name)
        {
            Log.Info("getMemberFromSP :: using query :: " + name);
            MemberModel model = null;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetMemberQuery, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                model = new MemberModel
                                {
                                    Id = DBNull.Value.Equals(rdr["ID"]) ? 0L : Convert.ToInt64(rdr["ID"]),
                                    Name = DBNull.Value.Equals(rdr["NAME"]) ? "" : rdr["NAME"].ToString().Trim(),
                                    Type = DBNull.Value.Equals(rdr["TYPE"]) ? "" : rdr["TYPE"].ToString().Trim()
                                };

                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving member by name.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            if (model == null)
            {
                Log.Debug("return from GetMemberByName : false");
            }
            else
            {
                Log.Debug("return from GetMemberByName : " + model.Id + ":" + model.Name);
            }

            return model;
        }

        public static List<CategoryModel> GetCategoryDetails(long catId)
        {
            Log.Info("GetCategoryDetails :: using query :: " + catId);
            List<CategoryModel> list = new List<CategoryModel>();
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetCatQuery, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.Add("@catId", SqlDbType.VarChar).Value = catId;
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                CategoryModel model = new CategoryModel
                                {
                                    CatId = DBNull.Value.Equals(rdr["CATID"]) ? 0L : Convert.ToInt64(rdr["CATID"]),
                                    CatName = DBNull.Value.Equals(rdr["CATNAME"])
                                        ? ""
                                        : rdr["CATNAME"].ToString().Trim(),
                                    AttrName = DBNull.Value.Equals(rdr["ATTRNAME"])
                                        ? ""
                                        : rdr["ATTRNAME"].ToString().Trim(),
                                    AttrRegion = DBNull.Value.Equals(rdr["REGIONNAME"])
                                        ? ""
                                        : rdr["REGIONNAME"].ToString().Trim()
                                };
                                list.Add(model);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving category details.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            Log.Debug($"Return from GetCategoryDetails : {list.Count}");
            return list;
        }

        public static List<ClassificationModel> GetClassification(long dataId)
        {
            Log.Info("GetClassification :: " + Configuration.GetClassificationSp + " :: " + dataId);
            List<ClassificationModel> list = new List<ClassificationModel>();

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetClassificationSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@dataID", SqlDbType.BigInt).Value = dataId;
                        //sqlCommand.CommandTimeout = Convert.ToInt16(SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ClassificationModel model = new ClassificationModel
                                {
                                    ClassId =
                                        DBNull.Value.Equals(rdr["CLASSID"]) ? 0L : Convert.ToInt64(rdr["CLASSID"]),
                                    ClassName = DBNull.Value.Equals(rdr["CLASSNAME"])
                                        ? ""
                                        : rdr["CLASSNAME"].ToString().Trim()
                                };
                                list.Add(model);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving classification.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            Log.Debug("Return from GetClassification : " + list.Count);
            return list;
        }

        public static List<FolderAliasModel> GetFolderAlias(string level)
        {
            Log.Info("getFolderAlias using query :: " + level);
            List<FolderAliasModel> list = new List<FolderAliasModel>();

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    //using (SqlCommand sqlCommand = new SqlCommand("select folderName,folderAlias,Folder_Level,ExtraPermission from ePfile_permFoldersAliases", sqlConnection))
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetFolderAliasQuery, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        //sqlCommand.CommandTimeout = Convert.ToInt16(SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                FolderAliasModel model = new FolderAliasModel
                                {
                                    FolderName = DBNull.Value.Equals(rdr["FOLDERNAME"])
                                        ? ""
                                        : rdr["FOLDERNAME"].ToString().Trim(),
                                    Alias = DBNull.Value.Equals(rdr["FOLDERALIAS"])
                                        ? ""
                                        : rdr["FOLDERALIAS"].ToString().Trim(),
                                    Level = DBNull.Value.Equals(rdr["FOLDER_LEVEL"])
                                        ? 0
                                        : Convert.ToInt16(rdr["FOLDER_LEVEL"]),
                                    ExtraPermission = DBNull.Value.Equals(rdr["ExtraPermission"])
                                        ? ""
                                        : rdr["ExtraPermission"].ToString().Trim()
                                };
                                // model.dataId = dataID;
                                //model.name = name;
                                if (string.IsNullOrEmpty(level) || (level.Equals(model.Level.ToString())))
                                {
                                    list.Add(model);
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving folder alias.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            Log.Debug("Return from getFolderAlias : " + list.Count);
            return list;
        }

        public static long GetAgencyTemplate(long classId)
        {
            Log.Info("GetAgencyTemplate :: " + Configuration.GetAgencyTemplateSp + " :: " + classId);
            long templateId = 0L;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetAgencyTemplateSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@CID", SqlDbType.BigInt).Value = classId;
                        sqlCommand.Parameters.Add("@TemplateVolume", SqlDbType.BigInt).Value = Convert.ToInt64(Configuration.TemplateVolId);
                        sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                templateId = DBNull.Value.Equals(rdr["DATAID"]) ? 0L : Convert.ToInt64(rdr["DATAID"]);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving agency template.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            Log.Debug("Return from getAgencyTemplate : " + templateId);
            return templateId;
        }

        public static List<AgencyModel> GetAgency(string prcCode, string source)
        {
            List<AgencyModel> models = new List<AgencyModel>();
            Log.Info("getAgencyFromSP :: " + Configuration.GetAgencySp + " :: " + source + " :: " + prcCode);
            if (String.IsNullOrEmpty(prcCode) && String.IsNullOrEmpty(source))
            {
                Log.Warn("getAgencyFromSP :: " + Configuration.GetAgencySp + " :: Return empty list as both source and prc code are blank");
                return models;
            }
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetAgencySp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        if (prcCode != null)
                        {
                            sqlCommand.Parameters.Add("@prcCode", SqlDbType.VarChar, 50).Value = prcCode;
                        }
                        if (source != null)
                        {
                            sqlCommand.Parameters.Add("@agencySource", SqlDbType.VarChar, 50).Value = source;
                        }
                        sqlConnection.Open();
                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                AgencyModel model = new AgencyModel
                                {
                                    HrmsPrcCode = DBNull.Value.Equals(rdr["HRMS_PRC"])
                                        ? 0L
                                        : Convert.ToInt64(rdr["HRMS_PRC"]),
                                    HrmsDescription = DBNull.Value.Equals(rdr["HRMS_DESCRIPTION"])
                                        ? ""
                                        : rdr["HRMS_DESCRIPTION"].ToString().Trim(),
                                    HrVital = DBNull.Value.Equals(rdr["HR_VITAL"]) ? "" : rdr["HR_VITAL"].ToString(),
                                    VoicesAgencyCode = DBNull.Value.Equals(rdr["Voices_Agency_Code"])
                                        ? ""
                                        : rdr["Voices_Agency_Code"].ToString().Trim(),
                                    VoicesAgencyName = DBNull.Value.Equals(rdr["Voices_Agency_Name"])
                                        ? ""
                                        : rdr["Voices_Agency_Name"].ToString().Trim(),
                                    DbthEpFile = DBNull.Value.Equals(rdr["DBTH_ePfile"])
                                        ? ""
                                        : rdr["DBTH_ePfile"].ToString().Trim(),
                                    AgencySource = DBNull.Value.Equals(rdr["AgencySource"])
                                        ? ""
                                        : rdr["AgencySource"].ToString().Trim()
                                };

                                models.Add(model);

                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving agencies.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            Log.Debug("Return from getAgency : " + models.Count);

            return models;
        }
        public static AgencyModel GetAgency(string prcCode)
        {
            Log.Info("getAgencyFromSP :: " + Configuration.GetChildrenSp + " :: " + prcCode);
            List<AgencyModel> models = GetAgency(prcCode, null);
            return models.Count > 0 ? models[0] : null;
        }

        //public static int InsertAppointment(List<AppointmentScenario> scenarios, String processIndicator, String nric, String name, String currentPRCCode, String agencyName, String servingAgency, String schemeOfService, String schemeOfServiceDescription, String divisionalStatus, String divisionText, String previousPRCCode, String prevAgency, String servingPrevAgency, String effectiveDate, String status)
        public static bool InsertAppointment(List<AppointmentCsv> scenarios)
        {
            Log.Info("insertAppointment :: using query");
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                    {
                        int rowsInserted = 0;
                        foreach (AppointmentCsv scenario in scenarios)
                        {

                            AgencyModel currentAgency = GetAgency(prcCode: scenario.CurrentPRCCode);
                            string status = currentAgency == null ? "Fail" : "Pending";
                            AgencyModel previousAgency = GetAgency(prcCode: scenario.PreviousPRCCode);

                            using (SqlCommand sqlCommand = new SqlCommand(Configuration.InsertAppointmentQuery, sqlConnection, transaction))
                            {
                                try
                                {
                                    sqlCommand.CommandType = CommandType.Text;
                                    sqlCommand.Parameters.Add("@ProcessIndicator", SqlDbType.VarChar).Value = scenario.ProcessIndicator;
                                    sqlCommand.Parameters.Add("@NRIC", SqlDbType.VarChar).Value = scenario.NRIC;
                                    sqlCommand.Parameters.Add("@Name", SqlDbType.VarChar).Value = scenario.Name;
                                    sqlCommand.Parameters.Add("@AgencyName", SqlDbType.VarChar).Value = currentAgency == null ? "Incorrect PRC Code" : currentAgency.VoicesAgencyCode;
                                    sqlCommand.Parameters.Add("@ServingAgency", SqlDbType.VarChar).Value = currentAgency == null ? "Incorrect PRC Code" : currentAgency.DbthEpFile;
                                    sqlCommand.Parameters.Add("@CurrentPRCCode", SqlDbType.VarChar).Value = scenario.CurrentPRCCode;
                                    sqlCommand.Parameters.Add("@SchemeOfService", SqlDbType.VarChar).Value = scenario.SchemeofService;
                                    sqlCommand.Parameters.Add("@SchemeofServiceDesc", SqlDbType.VarChar).Value = scenario.SchemeofServiceDescription;
                                    sqlCommand.Parameters.Add("@DivisionalStatus", SqlDbType.VarChar).Value = scenario.DivisionalStatus;
                                    sqlCommand.Parameters.Add("@DivisionText", SqlDbType.VarChar).Value = scenario.DivisonText;
                                    sqlCommand.Parameters.Add("@PreviousPRCCode", SqlDbType.VarChar).Value = scenario.PreviousPRCCode;

                                    if (previousAgency == null || string.IsNullOrEmpty(previousAgency.VoicesAgencyCode))
                                    {
                                        sqlCommand.Parameters.Add("@PrevAgency", SqlDbType.VarChar).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        sqlCommand.Parameters.Add("@PrevAgency", SqlDbType.VarChar).Value = previousAgency.VoicesAgencyCode;
                                    }
                                    if (previousAgency == null || string.IsNullOrEmpty(previousAgency.DbthEpFile))
                                    {
                                        sqlCommand.Parameters.Add("@ServingPrevAgency", SqlDbType.VarChar).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        sqlCommand.Parameters.Add("@ServingPrevAgency", SqlDbType.VarChar).Value = previousAgency.DbthEpFile;
                                    }
                                    sqlCommand.Parameters.Add("@Status", SqlDbType.VarChar).Value = status;

                                    if (DateTime.TryParseExact(scenario.EffectiveDate, Configuration.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime effectiveDate))
                                    {

                                        sqlCommand.Parameters.Add("@EffectiveDate", SqlDbType.DateTime).Value = effectiveDate;
                                    }
                                    else
                                    {
                                        sqlCommand.Parameters.Add("@EffectiveDate", SqlDbType.DateTime).Value = DBNull.Value;
                                    }

                                    String[] appointmentFormDetails = Configuration.AppointmentForm?.Split('|');
                                    if (appointmentFormDetails != null && appointmentFormDetails.Length == 4)
                                    {
                                        long dataID = Int64.Parse(appointmentFormDetails[1]);
                                        long volumeID = Int64.Parse(appointmentFormDetails[0]);
                                        long versionNum = Int64.Parse(appointmentFormDetails[2]);
                                        String tableName = appointmentFormDetails[3];
                                        long seq = GetNextSequenceNumber(volumeID, dataID, versionNum, tableName);

                                        sqlCommand.Parameters.Add("@Seq", SqlDbType.BigInt).Value = seq;
                                        sqlCommand.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                                        sqlCommand.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                                        sqlCommand.Parameters.Add("@VersionNumber", SqlDbType.BigInt).Value = versionNum;


                                        int result = sqlCommand.ExecuteNonQuery();
                                        Log.Debug("Number of rows inserted : " + result);
                                        rowsInserted += result;
                                    }
                                    else
                                    {
                                        throw new Exception("Appointment Form Details not configured");
                                    }
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                            Boolean personnelNumberAdded = AddPersonnelNumber(scenario.PERNR, currentAgency.VoicesAgencyCode, scenario.NRIC, scenario.Name);
                            if (!personnelNumberAdded)
                            {
                                transaction.Rollback();
                                throw new Exception("Failed to Add Personal Number");
                            }

                        }
                        // rows inserted = inserts in appointment table + inserts in pernnr table
                        if (rowsInserted == scenarios.Count)
                        {
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            Log.Warn("Not all rows got inserted in DB, rollback and move the CSV in error folder");
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while inserting appointments.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            return false;
        }

        public static bool InsertExit(List<ExitCsv> scenarios)
        {
            Log.Info("insertExit :: using query");
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                    {
                        int rowsInserted = 0;
                        foreach (ExitCsv scenario in scenarios)
                        {
                            AgencyModel currentAgency = GetAgency(prcCode: scenario.CurrentPRCCode);
                            string processStatus = currentAgency == null ? "Fail" : "Pending";
                            using (SqlCommand sqlCommand = new SqlCommand(Configuration.InsertExitQuery, sqlConnection, transaction))
                            {
                                try
                                {
                                    sqlCommand.CommandType = CommandType.Text;

                                    sqlCommand.Parameters.Add("@ProcessIndicator", SqlDbType.VarChar).Value = scenario.ProcessIndicator;
                                    sqlCommand.Parameters.Add("@NRIC", SqlDbType.VarChar).Value = scenario.NRIC;
                                    sqlCommand.Parameters.Add("@Name", SqlDbType.VarChar).Value = scenario.Name;
                                    sqlCommand.Parameters.Add("@AgencyName", SqlDbType.VarChar).Value = currentAgency == null ? "Incorrect PRC Code" : currentAgency.VoicesAgencyCode;
                                    sqlCommand.Parameters.Add("@ServingAgency", SqlDbType.VarChar).Value = currentAgency == null ? "Incorrect PRC Code" : currentAgency.DbthEpFile;
                                    sqlCommand.Parameters.Add("@CurrentPRCCode", SqlDbType.VarChar).Value = scenario.CurrentPRCCode;
                                    sqlCommand.Parameters.Add("@DivisionalStatus", SqlDbType.VarChar).Value = scenario.DivisionalStatus;
                                    sqlCommand.Parameters.Add("@Status", SqlDbType.VarChar).Value = "0";
                                    sqlCommand.Parameters.Add("@ProcessStatus", SqlDbType.VarChar).Value = processStatus;
                                    if (DateTime.TryParseExact(scenario.EffectiveDate, Configuration.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime effectiveDate))
                                    {
                                        sqlCommand.Parameters.Add("@EffectiveDate", SqlDbType.DateTime).Value = effectiveDate;
                                    }
                                    else
                                    {
                                        sqlCommand.Parameters.Add("@EffectiveDate", SqlDbType.DateTime).Value = DBNull.Value;
                                    }

                                    String[] exitFormDetails = Configuration.ExitForm?.Split('|');
                                    if (exitFormDetails != null && exitFormDetails.Length == 4)
                                    {
                                        long dataID = Int64.Parse(exitFormDetails[1]);
                                        long volumeID = Int64.Parse(exitFormDetails[0]);
                                        long versionNum = Int64.Parse(exitFormDetails[2]);
                                        String tableName = exitFormDetails[3];
                                        long seq = GetNextSequenceNumber(volumeID, dataID, versionNum, tableName);

                                        sqlCommand.Parameters.Add("@Seq", SqlDbType.BigInt).Value = seq;
                                        sqlCommand.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                                        sqlCommand.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                                        sqlCommand.Parameters.Add("@VersionNumber", SqlDbType.BigInt).Value = versionNum;

                                        int result = sqlCommand.ExecuteNonQuery();
                                        Log.Debug("Number of rows inserted : " + result);
                                        rowsInserted += result;
                                    }
                                    else
                                    {
                                        throw new Exception("Exit Form Details not configured");
                                    }
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }
                        if (rowsInserted == scenarios.Count)
                        {
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            Log.Warn("Not all rows got inserted in DB, rollback and move the CSV in error folder");
                            transaction.Rollback();
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while inserting exits.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return false;
        }

        public static bool InsertChangeMetadata(List<ChangeMetadataCsv> scenarios)
        {

            Log.Info("InsertChangeMetadata :: using query");

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                    {
                        int rowsInserted = 0;
                        foreach (ChangeMetadataCsv scenario in scenarios)
                        {

                            AgencyModel currentAgency = GetAgency(prcCode: scenario.CurrentPRCCode);
                            string processStatus = currentAgency == null ? "Fail" : "Pending";

                            using (SqlCommand sqlCommand = new SqlCommand(Configuration.InsertChangeMetadataQuery, sqlConnection, transaction))
                            {
                                try
                                {
                                    sqlCommand.CommandType = CommandType.Text;

                                    sqlCommand.Parameters.Add("@ProcessIndicator", SqlDbType.VarChar).Value = scenario.ProcessIndicator;
                                    sqlCommand.Parameters.Add("@NRIC", SqlDbType.VarChar).Value = scenario.NRIC;
                                    sqlCommand.Parameters.Add("@FIN", SqlDbType.VarChar).Value = DBNull.Value;
                                    sqlCommand.Parameters.Add("@Name", SqlDbType.VarChar).Value = scenario.Name;
                                    sqlCommand.Parameters.Add("@Agency", SqlDbType.VarChar).Value = currentAgency == null ? "Incorrect PRC Code" : currentAgency.VoicesAgencyCode;
                                    sqlCommand.Parameters.Add("@ServingAgency", SqlDbType.VarChar).Value = currentAgency == null ? "Incorrect PRC Code" : currentAgency.DbthEpFile;
                                    sqlCommand.Parameters.Add("@PRCCode", SqlDbType.VarChar).Value = scenario.CurrentPRCCode;
                                    sqlCommand.Parameters.Add("@ProcessStatus", SqlDbType.VarChar).Value = processStatus;
                                    sqlCommand.Parameters.Add("@SchemeOfService", SqlDbType.VarChar).Value = scenario.SchemeofService;
                                    sqlCommand.Parameters.Add("@SchemeofServiceDesc", SqlDbType.VarChar).Value = scenario.SchemeofServiceDescription;
                                    sqlCommand.Parameters.Add("@DivisionalStatus", SqlDbType.VarChar).Value = scenario.DivisionalStatus;
                                    sqlCommand.Parameters.Add("@DivisionText", SqlDbType.VarChar).Value = scenario.DivisonText;
                                    if (DateTime.TryParseExact(scenario.EffectiveDate, Configuration.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime effectiveDate))
                                    {

                                        sqlCommand.Parameters.Add("@EffectiveDate", SqlDbType.DateTime).Value = effectiveDate;
                                    }
                                    else
                                    {
                                        sqlCommand.Parameters.Add("@EffectiveDate", SqlDbType.DateTime).Value = DBNull.Value;
                                    }

                                    String[] changeMdFormDetails = Configuration.ChangeMdForm?.Split('|');
                                    if (changeMdFormDetails != null && changeMdFormDetails.Length == 4)
                                    {
                                        long dataID = Int64.Parse(changeMdFormDetails[1]);
                                        long volumeID = Int64.Parse(changeMdFormDetails[0]);
                                        long versionNum = Int64.Parse(changeMdFormDetails[2]);
                                        String tableName = changeMdFormDetails[3];
                                        long seq = GetNextSequenceNumber(volumeID, dataID, versionNum, tableName);

                                        sqlCommand.Parameters.Add("@Seq", SqlDbType.BigInt).Value = seq;
                                        sqlCommand.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                                        sqlCommand.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                                        sqlCommand.Parameters.Add("@VersionNumber", SqlDbType.BigInt).Value = versionNum;


                                        int result = sqlCommand.ExecuteNonQuery();
                                        Log.Debug("Number of rows inserted : " + result);
                                        rowsInserted += result;
                                    }
                                    else
                                    {
                                        throw new Exception("ChangeMD Form Details not configured");
                                    }
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                            if (scenario.ProcessIndicator.Equals("Change in Name"))
                            {
                                using (SqlCommand sqlCommand = new SqlCommand(Configuration.UpdateNamePernrQuery, sqlConnection, transaction))
                                {
                                    try
                                    {
                                        sqlCommand.CommandType = CommandType.Text;

                                        sqlCommand.Parameters.Add("@NRIC", SqlDbType.VarChar).Value = scenario.NRIC;
                                        sqlCommand.Parameters.Add("@Name", SqlDbType.VarChar).Value = scenario.Name;
                                        sqlCommand.Parameters.Add("@Agency", SqlDbType.VarChar).Value = currentAgency.VoicesAgencyCode;

                                        int result = sqlCommand.ExecuteNonQuery();
                                        Log.Debug("Number of rows inserted : " + result);
                                        if (result == 0)
                                        {
                                            // entry does not exist in table for NRIC.
                                            Log.WarnFormat("While updating name {0} for {1} in PERNR table, rows returned is {2}", scenario.Name, scenario.NRIC, result);
                                        }
                                        //return result;
                                    }
                                    catch (Exception)
                                    {
                                        transaction.Rollback();
                                        throw;
                                    }
                                }
                            }
                        }
                        if (rowsInserted == scenarios.Count)
                        {
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            Log.Warn("Not all rows got inserted in DB, rollback and move the CSV in error folder");
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while inserting changeMds.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return false;
        }

        public static bool InsertChangeId(List<ChangeIdcsv> scenarios)
        {

            Log.Info("InsertChangeID :: using query");

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                    {
                        int rowsInserted = 0;
                        foreach (ChangeIdcsv scenario in scenarios)
                        {

                            AgencyModel currentAgency = GetAgency(prcCode: scenario.Agency);

                            using (SqlCommand sqlCommand = new SqlCommand(Configuration.InsertChangeIdQuery, sqlConnection, transaction))
                            {
                                try
                                {
                                    sqlCommand.CommandType = CommandType.Text;
                                    //[VolumeID],[DataID],[VersionNum],[Seq],[Process_Indicator],[New_NRIC],[Old_NRIC],[Effective_Date],[Process_Status],[Creation_Date]
                                    sqlCommand.Parameters.Add("@ProcessIndicator", SqlDbType.VarChar).Value = scenario.ProcessIndicator;
                                    sqlCommand.Parameters.Add("@NewNRIC", SqlDbType.VarChar).Value = scenario.NRIC;
                                    sqlCommand.Parameters.Add("@OldNRIC", SqlDbType.VarChar).Value = scenario.FIN;
                                    sqlCommand.Parameters.Add("@ProcessStatus", SqlDbType.VarChar).Value = DBNull.Value;
                                    sqlCommand.Parameters.Add("@Agency", SqlDbType.VarChar).Value = scenario.Agency;
                                    if (DateTime.TryParseExact(scenario.EffectiveDate, Configuration.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime effectiveDate))
                                    {

                                        sqlCommand.Parameters.Add("@EffectiveDate", SqlDbType.DateTime).Value = effectiveDate;
                                    }
                                    else
                                    {
                                        sqlCommand.Parameters.Add("@EffectiveDate", SqlDbType.DateTime).Value = DBNull.Value;
                                    }

                                    String[] changeIdFormDetails = Configuration.ChangeIdForm?.Split('|');
                                    if (changeIdFormDetails != null && changeIdFormDetails.Length == 4)
                                    {
                                        long dataID = Int64.Parse(changeIdFormDetails[1]);
                                        long volumeID = Int64.Parse(changeIdFormDetails[0]);
                                        long versionNum = Int64.Parse(changeIdFormDetails[2]);
                                        String tableName = changeIdFormDetails[3];
                                        long seq = GetNextSequenceNumber(volumeID, dataID, versionNum, tableName);

                                        sqlCommand.Parameters.Add("@Seq", SqlDbType.BigInt).Value = seq;
                                        sqlCommand.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                                        sqlCommand.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                                        sqlCommand.Parameters.Add("@VersionNumber", SqlDbType.BigInt).Value = versionNum;


                                        int result = sqlCommand.ExecuteNonQuery();
                                        Log.Debug("Number of rows inserted : " + result);
                                        rowsInserted += result;
                                    }
                                    else
                                    {
                                        throw new Exception("ChangeID Form Details not configured");
                                    }
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }

                            using (SqlCommand sqlCommand = new SqlCommand(Configuration.UpdateIdPernrQuery, sqlConnection, transaction))
                            {
                                try
                                {
                                    sqlCommand.CommandType = CommandType.Text;

                                    sqlCommand.Parameters.Add("@NewNRIC", SqlDbType.VarChar).Value = scenario.NRIC;
                                    sqlCommand.Parameters.Add("@OldNRIC", SqlDbType.VarChar).Value = scenario.FIN;
                                    sqlCommand.Parameters.Add("@Agency", SqlDbType.VarChar).Value = currentAgency.VoicesAgencyCode;

                                    int result = sqlCommand.ExecuteNonQuery();
                                    Log.Debug("Number of rows updated : " + result);
                                    if (result == 0)
                                    {
                                        // entry does not exist in table for NRIC.
                                        Log.WarnFormat("While updating ID {0} for {1} in PERNR table, rows returned is {2}", scenario.NRIC, scenario.FIN, result);
                                    }
                                    //return result;
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }
                        if (rowsInserted == scenarios.Count)
                        {
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            Log.Warn("Not all rows got inserted in DB, rollback and move the CSV in error folder");
                            transaction.Rollback();
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while inserting changeIds.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            return false;
        }

        public static bool InsertSecondment(List<SecondmentCsv> scenarios)
        {
            Log.Info("InsertSecondment :: using query");
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                    {
                        int rowsInserted = 0;
                        foreach (SecondmentCsv scenario in scenarios)
                        {
                            using (SqlCommand sqlCommand = new SqlCommand(Configuration.InsertSecondmentQuery, sqlConnection, transaction))
                            {
                                try
                                {
                                    AgencyModel parentAgencyModel = GetAgency(prcCode: scenario.PreviousPRCCode);
                                    string parentAgency = parentAgencyModel == null ? "Incorrect PRC Code" : parentAgencyModel.VoicesAgencyCode;
                                    string servingParentAgency = parentAgencyModel == null ? "Incorrect PRC Code" : parentAgencyModel.DbthEpFile;

                                    AgencyModel newBorrowingAgencyModel = GetAgency(prcCode: scenario.CurrentPRCCode);
                                    string newBorrowingAgency = newBorrowingAgencyModel == null ? "Incorrect PRC Code" : newBorrowingAgencyModel.VoicesAgencyCode;
                                    string servingNewBorrowingAgency = newBorrowingAgencyModel == null ? "Incorrect PRC Code" : newBorrowingAgencyModel.DbthEpFile;

                                    string status = (parentAgencyModel == null || newBorrowingAgencyModel == null) ? "Fail" : "Pending";

                                    sqlCommand.CommandType = CommandType.Text;

                                    sqlCommand.Parameters.Add("@ProcessIndicator", SqlDbType.VarChar).Value = scenario.ProcessIndicator;
                                    sqlCommand.Parameters.Add("@NRIC", SqlDbType.VarChar).Value = scenario.NRIC;
                                    sqlCommand.Parameters.Add("@Name", SqlDbType.VarChar).Value = scenario.Name;
                                    sqlCommand.Parameters.Add("@ParentPRCCode", SqlDbType.VarChar).Value = scenario.PreviousPRCCode;
                                    sqlCommand.Parameters.Add("@ParentAgencyName", SqlDbType.VarChar).Value = parentAgency;
                                    sqlCommand.Parameters.Add("@ServingParentAgency", SqlDbType.VarChar).Value = servingParentAgency;
                                    sqlCommand.Parameters.Add("@BorrowingAgency", SqlDbType.VarChar).Value = scenario.CurrentPRCCode;
                                    sqlCommand.Parameters.Add("@NewBorrowingAgency1", SqlDbType.VarChar).Value = newBorrowingAgency;
                                    sqlCommand.Parameters.Add("@NewBorrowingAgency2", SqlDbType.VarChar).Value = newBorrowingAgency;
                                    sqlCommand.Parameters.Add("@ServingNewBorrowingAgency", SqlDbType.VarChar).Value = servingNewBorrowingAgency;
                                    sqlCommand.Parameters.Add("@OldBorrowingPRCCode", SqlDbType.VarChar).Value = parentAgency;
                                    sqlCommand.Parameters.Add("@OldBorrowingAgency", SqlDbType.VarChar).Value = parentAgency;
                                    sqlCommand.Parameters.Add("@ServingOldBorrowingAgency", SqlDbType.VarChar).Value = servingParentAgency;
                                    sqlCommand.Parameters.Add("@SchemeOfService", SqlDbType.VarChar).Value = scenario.SchemeofService;
                                    sqlCommand.Parameters.Add("@SchemeOfServiceDesc", SqlDbType.VarChar).Value = scenario.SchemeofServiceDescription;
                                    sqlCommand.Parameters.Add("@DivisionalStatus", SqlDbType.VarChar).Value = scenario.DivisionalStatus;
                                    sqlCommand.Parameters.Add("@DivisionText", SqlDbType.VarChar).Value = scenario.DivisonText;
                                    sqlCommand.Parameters.Add("@Status", SqlDbType.VarChar).Value = status;


                                    String[] changeSecondmentFormDetails = Configuration.SecondmentForm?.Split('|');
                                    if (changeSecondmentFormDetails != null && changeSecondmentFormDetails.Length == 4)
                                    {
                                        long dataID = Int64.Parse(changeSecondmentFormDetails[1]);
                                        long volumeID = Int64.Parse(changeSecondmentFormDetails[0]);
                                        long versionNum = Int64.Parse(changeSecondmentFormDetails[2]);
                                        String tableName = changeSecondmentFormDetails[3];
                                        long seq = GetNextSequenceNumber(volumeID, dataID, versionNum, tableName);

                                        sqlCommand.Parameters.Add("@Seq", SqlDbType.BigInt).Value = seq;
                                        sqlCommand.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                                        sqlCommand.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                                        sqlCommand.Parameters.Add("@VersionNumber", SqlDbType.BigInt).Value = versionNum;


                                        int result = sqlCommand.ExecuteNonQuery();
                                        Log.Debug("Number of rows inserted : " + result);
                                        rowsInserted += result;
                                    }
                                    else
                                    {
                                        throw new Exception("Secondment Form Details not configured");
                                    }
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }
                        if (rowsInserted == scenarios.Count)
                        {
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            Log.Warn("Not all rows got inserted in DB, rollback and move the CSV in error folder");
                            transaction.Rollback();
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while inserting secondments.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            return false;
        }

        public static List<AppointmentScenario> GetPendingAppointmentScenarios()
        {
            List<AppointmentScenario> list = new List<AppointmentScenario>();
            Log.Info("getAppointmentScenarios :: " + Configuration.GetPendingScenariosSp);

            try
            {

                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {

                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetPendingScenariosSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@scenario", SqlDbType.VarChar).Value = "Appointment";
                        sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                AppointmentScenario model = new AppointmentScenario
                                {
                                    Agency = DBNull.Value.Equals(rdr["iGEMS_Agency__Mapping_logic_"])
                                        ? ""
                                        : rdr["iGEMS_Agency__Mapping_logic_"].ToString().Trim(),
                                    CurrentPrcCode = DBNull.Value.Equals(rdr["HRMS_Agency"])
                                        ? ""
                                        : rdr["HRMS_Agency"].ToString().Trim(),
                                    DivisionalStatus = DBNull.Value.Equals(rdr["Divisional_Status"])
                                        ? ""
                                        : rdr["Divisional_Status"].ToString().Trim(),
                                    DivisionText = DBNull.Value.Equals(rdr["Division_Text"])
                                        ? ""
                                        : rdr["Division_Text"].ToString().Trim(),
                                    EffectiveDate = DBNull.Value.Equals(rdr["Effective_Date"])
                                        ? (DateTime?)null
                                        : Convert.ToDateTime(rdr["Effective_Date"]),
                                    Id = DBNull.Value.Equals(rdr["NRIC"]) ? "" : rdr["NRIC"].ToString().Trim(),
                                    Name = DBNull.Value.Equals(rdr["Name"]) ? "" : rdr["Name"].ToString().Trim(),
                                    PreviousPrcCode = DBNull.Value.Equals(rdr["Previous_Agency"])
                                        ? ""
                                        : rdr["Previous_Agency"].ToString().Trim(),
                                    ProcessIndicator = DBNull.Value.Equals(rdr["Process_Indicator"])
                                        ? ""
                                        : rdr["Process_Indicator"].ToString().Trim(),
                                    SchemeOfService = DBNull.Value.Equals(rdr["Scheme_of_Service"])
                                        ? ""
                                        : rdr["Scheme_of_Service"].ToString().Trim(),
                                    SchemeOfServiceDescription =
                                        DBNull.Value.Equals(rdr["Scheme_of_Service_Description"])
                                            ? ""
                                            : rdr["Scheme_of_Service_Description"].ToString().Trim(),
                                    Seq = DBNull.Value.Equals(rdr["Seq"]) ? 0L : Convert.ToInt64(rdr["Seq"]),
                                    FormId = DBNull.Value.Equals(rdr["DataID"]) ? 0L : Convert.ToInt64(rdr["DataID"]),
                                    ServingAgency = DBNull.Value.Equals(rdr["Serving_Agency"])
                                        ? ""
                                        : rdr["Serving_Agency"].ToString().Trim(),
                                    ProcessStatus = DBNull.Value.Equals(rdr["Process_Status"])
                                        ? ""
                                        : rdr["Process_Status"].ToString().Trim()
                                };
                                list.Add(model);
                            }
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving appointments.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            Log.Debug("return from getPendingAppointment : " + list.Count);
            return list;

        }

        public static List<SecondmentScenario> GetPendingSecondmentScenarios()
        {
            List<SecondmentScenario> list = new List<SecondmentScenario>();
            Log.Info("GetPendingSecondmentScenarios :: " + Configuration.GetPendingScenariosSp);

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {

                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetPendingScenariosSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@scenario", SqlDbType.VarChar).Value = "Secondment";
                        sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                SecondmentScenario model = new SecondmentScenario
                                {
                                    CurrentPrcCode = DBNull.Value.Equals(rdr["Borrowing_Agency"])
                                        ? ""
                                        : rdr["Borrowing_Agency"].ToString().Trim(),
                                    Id = DBNull.Value.Equals(rdr["NRIC"]) ? "" : rdr["NRIC"].ToString().Trim(),
                                    NewBorrowingAgency = DBNull.Value.Equals(rdr["iGEMS_New_Borrowing_Agency"])
                                        ? ""
                                        : rdr["iGEMS_New_Borrowing_Agency"].ToString().Trim(),
                                    ProcessIndicator = DBNull.Value.Equals(rdr["Process_Indicator"])
                                        ? ""
                                        : rdr["Process_Indicator"].ToString().Trim(),
                                    Seq = DBNull.Value.Equals(rdr["Seq"]) ? 0L : Convert.ToInt64(rdr["Seq"]),
                                    FormId = DBNull.Value.Equals(rdr["DataID"]) ? 0L : Convert.ToInt64(rdr["DataID"]),
                                    ParentAgency = DBNull.Value.Equals(rdr["iGEMS_Agency__Mapping_logic_"])
                                        ? ""
                                        : rdr["iGEMS_Agency__Mapping_logic_"].ToString().Trim(),
                                    ProcessStatus = DBNull.Value.Equals(rdr["Process_Status"])
                                        ? ""
                                        : rdr["Process_Status"].ToString().Trim()
                                };
                                list.Add(model);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving secondments.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            Log.Debug("return from GetPendingSecondmentScenarios : " + list.Count);
            return list;

        }

        public static List<ExitScenario> GetPendingExitScenarios()
        {
            List<ExitScenario> list = new List<ExitScenario>();
            Log.Info("GetPendingExitScenarios :: " + Configuration.GetPendingScenariosSp);

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetPendingScenariosSp, sqlConnection))
                    {

                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@scenario", SqlDbType.VarChar).Value = "Exit";
                        sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ExitScenario model = new ExitScenario
                                {
                                    Agency = DBNull.Value.Equals(rdr["iGEMS_Agency__Mapping_logic_"])
                                        ? ""
                                        : rdr["iGEMS_Agency__Mapping_logic_"].ToString().Trim(),
                                    CurrentPrcCode = DBNull.Value.Equals(rdr["Agency"])
                                        ? ""
                                        : rdr["Agency"].ToString().Trim(),
                                    EffectiveDate = DBNull.Value.Equals(rdr["Effective_Date"])
                                        ? (DateTime?)null
                                        : Convert.ToDateTime(rdr["Effective_Date"]),
                                    Id = DBNull.Value.Equals(rdr["NRIC"]) ? "" : rdr["NRIC"].ToString().Trim(),
                                    Name = DBNull.Value.Equals(rdr["Name"]) ? "" : rdr["Name"].ToString().Trim(),
                                    ProcessIndicator = DBNull.Value.Equals(rdr["Process_Indicator"])
                                        ? ""
                                        : rdr["Process_Indicator"].ToString().Trim(),
                                    Seq = DBNull.Value.Equals(rdr["Seq"]) ? 0L : Convert.ToInt64(rdr["Seq"]),
                                    FormId = DBNull.Value.Equals(rdr["DataID"]) ? 0L : Convert.ToInt64(rdr["DataID"]),
                                    ServingAgency = DBNull.Value.Equals(rdr["Serving_Agency"])
                                        ? ""
                                        : rdr["Serving_Agency"].ToString().Trim(),
                                    ProcessStatus = DBNull.Value.Equals(rdr["Process_Status"])
                                        ? ""
                                        : rdr["Process_Status"].ToString().Trim()
                                };

                                list.Add(model);
                            }
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving exits.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            Log.Debug("return from GetPendingExitScenarios : " + list.Count);
            return list;

        }

        public static List<ChangeMdScenario> GetPendingChangeinMdScenarios()
        {
            List<ChangeMdScenario> list = new List<ChangeMdScenario>();
            Log.Info("GetPendingChangeinMdScenarios :: " + Configuration.GetPendingScenariosSp);

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetPendingScenariosSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@scenario", SqlDbType.VarChar).Value = "Metadata";
                        sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ChangeMdScenario model = new ChangeMdScenario
                                {
                                    Agency = DBNull.Value.Equals(rdr["iGEMS_Agency__Mapping_logic_"])
                                        ? ""
                                        : rdr["iGEMS_Agency__Mapping_logic_"].ToString().Trim(),
                                    CurrentPrcCode = DBNull.Value.Equals(rdr["Agency"])
                                        ? ""
                                        : rdr["Agency"].ToString().Trim(),
                                    EffectiveDate = DBNull.Value.Equals(rdr["Effective_Date"])
                                        ? (DateTime?)null
                                        : Convert.ToDateTime(rdr["Effective_Date"]),
                                    Id = DBNull.Value.Equals(rdr["NRIC"]) ? "" : rdr["NRIC"].ToString().Trim(),
                                    Name = DBNull.Value.Equals(rdr["Name"]) ? "" : rdr["Name"].ToString().Trim(),
                                    ProcessIndicator = DBNull.Value.Equals(rdr["Process_Indicator"])
                                        ? ""
                                        : rdr["Process_Indicator"].ToString().Trim(),
                                    SchemeOfService = DBNull.Value.Equals(rdr["Scheme_of_Service"])
                                        ? ""
                                        : rdr["Scheme_of_Service"].ToString().Trim(),
                                    SchemeOfServiceDescription =
                                        DBNull.Value.Equals(rdr["Scheme_of_Service_Full_Description"])
                                            ? ""
                                            : rdr["Scheme_of_Service_Full_Description"].ToString().Trim(),
                                    DivisionText = DBNull.Value.Equals(rdr["Divison_Text"])
                                        ? ""
                                        : rdr["Divison_Text"].ToString().Trim(),
                                    DivisionalStatus = DBNull.Value.Equals(rdr["Divisional_Status"])
                                        ? ""
                                        : rdr["Divisional_Status"].ToString().Trim(),
                                    Seq = DBNull.Value.Equals(rdr["Seq"]) ? 0L : Convert.ToInt64(rdr["Seq"]),
                                    FormId = DBNull.Value.Equals(rdr["DataID"]) ? 0L : Convert.ToInt64(rdr["DataID"]),
                                    ServingAgency = DBNull.Value.Equals(rdr["Serving_Agency"])
                                        ? ""
                                        : rdr["Serving_Agency"].ToString().Trim(),
                                    ProcessStatus = DBNull.Value.Equals(rdr["Process_Status"])
                                        ? ""
                                        : rdr["Process_Status"].ToString().Trim()
                                };

                                list.Add(model);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving changeMds.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            Log.Debug("return from GetPendingChangeinMdScenarios : " + list.Count);
            return list;
        }

        public static List<ChangeIdScenario> GetPendingChangeIdScenarios()
        {
            List<ChangeIdScenario> list = new List<ChangeIdScenario>();
            Log.Info("getChangeIDScenarios :: " + Configuration.GetPendingScenariosSp);

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetPendingScenariosSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@scenario", SqlDbType.VarChar).Value = "ID";
                        sqlCommand.CommandTimeout = Convert.ToInt16(Configuration.SqlTimeout);
                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ChangeIdScenario model = new ChangeIdScenario
                                {
                                    EffectiveDate = DBNull.Value.Equals(rdr["Effective_Date"])
                                        ? (DateTime?)null
                                        : Convert.ToDateTime(rdr["Effective_Date"]),
                                    NewId = DBNull.Value.Equals(rdr["New_NRIC"])
                                        ? ""
                                        : rdr["New_NRIC"].ToString().Trim(),
                                    OldId = DBNull.Value.Equals(rdr["Old_NRIC"])
                                        ? ""
                                        : rdr["Old_NRIC"].ToString().Trim(),
                                    ProcessIndicator = DBNull.Value.Equals(rdr["Process_Indicator"])
                                        ? ""
                                        : rdr["Process_Indicator"].ToString().Trim(),
                                    Seq = DBNull.Value.Equals(rdr["Seq"]) ? 0L : Convert.ToInt64(rdr["Seq"]),
                                    FormId = DBNull.Value.Equals(rdr["DataID"]) ? 0L : Convert.ToInt64(rdr["DataID"]),
                                    ProcessStatus = DBNull.Value.Equals(rdr["Process_Status"])
                                        ? ""
                                        : rdr["Process_Status"].ToString().Trim(),
                                    Agency = DBNull.Value.Equals(rdr["Agency"])
                                        ? ""
                                        : rdr["Agency"].ToString().Trim()
                                };
                                list.Add(model);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving changeIds.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            Log.Debug("return from getPendingChangeID : " + list.Count);
            return list;

        }

        public static bool UpdateScenarioStatus(string scenario, string status, long formId, long seq)
        {
            //List<AppointmentScenario> list = new List<AppointmentScenario>();
            Log.Info("updateScenario Status :: " + Configuration.UpdateScenariosSp + " : " + scenario + " : " + seq + " : " + scenario);

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {

                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.UpdateScenariosSp, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add("@scenario", SqlDbType.VarChar).Value = scenario;
                        sqlCommand.Parameters.Add("@Status", SqlDbType.VarChar).Value = status;
                        sqlCommand.Parameters.Add("@Seq", SqlDbType.BigInt).Value = seq;
                        sqlCommand.Parameters.Add("@FormID", SqlDbType.BigInt).Value = formId;

                        sqlConnection.Open();

                        int rows = sqlCommand.ExecuteNonQuery();
                        Log.Debug("Number of rows updated : " + rows);

                        if (rows > 0)
                        {
                            return true;
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while updating scenario status.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            return false;

        }

        public static string GetDocumentNric(long dataId)
        {
            string nric = "";
            Log.Info("getDocumentNRIC using query :: " + dataId);

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetDocumentNricQuery, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.Text;

                        sqlCommand.Parameters.Add("@DataID", SqlDbType.VarChar).Value = dataId;

                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                nric = DBNull.Value.Equals(rdr["nric"]) ? "" : rdr["nric"].ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving document NRIC.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            Log.Debug("Returning NRIC : " + nric);
            return nric;
        }

        /*
        private static int UpdateFormSequence(SqlConnection sqlConnection, SqlTransaction transaction, long volumeID, long dataID, long versionNum, long seq, long newSeq)
        {
            using (SqlCommand sqlCommand = new SqlCommand(Configuration.UpdateSeqQuery1, sqlConnection, transaction))
            {
                try
                {
                    //Get the original document file name

                    sqlCommand.CommandType = CommandType.Text;

                    sqlCommand.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                    sqlCommand.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                    sqlCommand.Parameters.Add("@VersionNum", SqlDbType.BigInt).Value = versionNum;
                    sqlCommand.Parameters.Add("@Seq", SqlDbType.BigInt).Value = seq;
                    sqlCommand.Parameters.Add("@NewSeq", SqlDbType.BigInt).Value = newSeq;
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }


                    int result = sqlCommand.ExecuteNonQuery();

                    if (result == 0)
                    {
                        Log.Debug("failed to update formsequence, will try using less than query");
                        //try updating the value wihout sequence check
                        using (SqlCommand sqlCommand2 = new SqlCommand(Configuration.UpdateSeqQuery2, sqlConnection, transaction))
                        {

                            //Get the original document file name

                            sqlCommand2.CommandType = CommandType.Text;

                            sqlCommand2.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                            sqlCommand2.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                            sqlCommand2.Parameters.Add("@VersionNum", SqlDbType.BigInt).Value = versionNum;
                            sqlCommand2.Parameters.Add("@Seq", SqlDbType.BigInt).Value = newSeq;
                            sqlCommand2.Parameters.Add("@NewSeq", SqlDbType.BigInt).Value = newSeq;

                            result = sqlCommand2.ExecuteNonQuery();

                        }
                    }

                    return result;

                }
                catch (SqlException ex)
                {
                    StringBuilder errorMessages = new StringBuilder();
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Log.Error(String.Format("SQL Exception while update formsequence.\n {0}", errorMessages));
                }
                catch (InvalidOperationException ex)
                {
                    Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                }
                return 0;
            }
        }
        */
        public static Boolean AddPersonnelNumber(string personnelNumber, string agency, string nric, string name)
        {
            Log.Info($"AddPersonnelNumber :: using query with inputs personnelNumber:{personnelNumber}, ageny:{agency}, " +
                $"nric:{nric}, name:{name}");
            long rowsChanged = 0L;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                Boolean hasEntry = false;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    //set all to old
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.UpdateStatusPernrQuery2, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.Add("@Status", SqlDbType.VarChar).Value = "OLD";
                        sqlCommand.Parameters.Add("@Agency", SqlDbType.VarChar).Value = agency;
                        sqlCommand.Parameters.Add("@NRIC", SqlDbType.VarChar).Value = nric;
                        sqlCommand.ExecuteNonQuery();
                    }

                    //check if entries exist
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetPernrQuery, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.Add("@PERNR", SqlDbType.VarChar).Value = personnelNumber;
                        sqlCommand.Parameters.Add("@Agency", SqlDbType.VarChar).Value = agency;
                        sqlCommand.Parameters.Add("@NRIC", SqlDbType.VarChar).Value = nric;
                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                hasEntry = true;
                            }
                        }
                    }
                    //update if exist otherwise insert
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        if (hasEntry)
                        {
                            sqlCommand.CommandText = Configuration.UpdateStatusPernrQuery1;
                        }
                        else
                        {
                            sqlCommand.CommandText = Configuration.InsertPernrQuery;

                            String[] pernrFormDetails = Configuration.PersonnelNumberForm?.Split('|');
                            if (pernrFormDetails != null && pernrFormDetails.Length == 4)
                            {
                                long dataID = Int64.Parse(pernrFormDetails[1]);
                                long volumeID = Int64.Parse(pernrFormDetails[0]);
                                long versionNum = Int64.Parse(pernrFormDetails[2]);
                                String tableName = pernrFormDetails[3];
                                long seq = GetNextSequenceNumber(volumeID, dataID, versionNum, tableName);

                                sqlCommand.Parameters.Add("@Seq", SqlDbType.BigInt).Value = seq;
                                sqlCommand.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                                sqlCommand.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                                sqlCommand.Parameters.Add("@VersionNumber", SqlDbType.BigInt).Value = versionNum;
                                sqlCommand.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
                                sqlCommand.Parameters.Add("@InterfaceDate", SqlDbType.DateTime).Value = DateTime.Now;
                                sqlCommand.Parameters.Add("@EffectiveDate", SqlDbType.DateTime).Value = DateTime.Now;
                            }
                            else
                            {
                                throw new Exception("Personnel Number Form Details not configured");
                            }
                        }

                        sqlCommand.Parameters.Add("@PERNR", SqlDbType.VarChar).Value = personnelNumber;
                        sqlCommand.Parameters.Add("@Agency", SqlDbType.VarChar).Value = agency;
                        sqlCommand.Parameters.Add("@NRIC", SqlDbType.VarChar).Value = nric;
                        sqlCommand.Parameters.Add("@Status", SqlDbType.VarChar).Value = "NEW";

                        //Get the original document file name

                        sqlCommand.CommandType = CommandType.Text;
                        rowsChanged = sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Log.Error(String.Format("SQL Exception while retrieving adding personnel number.\n {0}", errorMessages));
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(String.Format("Invalid Opertion Exception {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            Log.Debug("return from AddPersonnelNumber : " + rowsChanged);
            return rowsChanged > 0;
        }

        private static long GetNextSequenceNumber(long volumeID, long dataID, long versionNum, String tableName)
        {
            Log.Debug($"GetNextSequenceNumber: {volumeID}:{dataID}:{versionNum}:{tableName}");
            long seq = 0;
            int rowsUpdated = 0;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    Boolean sequenceExist = true;
                    using (SqlCommand sqlCommand = new SqlCommand(Configuration.SeqQuery, sqlConnection))
                    {
                        //Get the original document file name
                        Log.Debug($"get sequence from formssequence: {volumeID}:{dataID}:{versionNum}");
                        sqlCommand.CommandType = CommandType.Text;

                        sqlCommand.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                        sqlCommand.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                        sqlCommand.Parameters.Add("@VersionNum", SqlDbType.BigInt).Value = versionNum;
                        if (sqlConnection.State != ConnectionState.Open)
                            sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                seq = rdr.GetValue(rdr.GetOrdinal("Seq")) as long? ?? 0L;
                                seq++;
                            }
                            else
                            {
                                sequenceExist = false;
                            }
                        }
                    }

                    using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                    {
                        try
                        {
                            String seqQuery = Configuration.UpdateSeqQuery;
                            if (!sequenceExist)
                            {
                                // No sequence exist, default is 1
                                seq = 1;
                                seqQuery = Configuration.InsertSeqQuery;
                                Log.Debug($"sequence doesnt exist in formssequence table, will insert an entry");
                                // check if there is already a sequence number in the template table for 
                                // the form (e.g. possible in case of upgrade)
                                String query = Configuration.TableSeqQuery.Replace("@Table", tableName);
                                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection, transaction))
                                {
                                    Log.Debug($"get max sequence from table: {volumeID}:{dataID}:{versionNum}:{tableName}");
                                    sqlCommand.CommandType = CommandType.Text;

                                    sqlCommand.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                                    sqlCommand.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                                    sqlCommand.Parameters.Add("@VersionNum", SqlDbType.BigInt).Value = versionNum;
                                    if (sqlConnection.State != ConnectionState.Open)
                                        sqlConnection.Open();

                                    using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                                    {
                                        if (rdr.Read())
                                        {
                                            seq = rdr.GetValue(rdr.GetOrdinal("Seq")) as long? ?? 0L;
                                            seq += 1;
                                        }
                                    }
                                }

                            }

                            //update the formsequence table
                            using (SqlCommand sqlCommand = new SqlCommand(seqQuery, sqlConnection, transaction))
                            {
                                sqlCommand.CommandType = CommandType.Text;

                                sqlCommand.Parameters.Add("@DataID", SqlDbType.BigInt).Value = dataID;
                                sqlCommand.Parameters.Add("@VolumeID", SqlDbType.BigInt).Value = volumeID;
                                sqlCommand.Parameters.Add("@VersionNum", SqlDbType.BigInt).Value = versionNum;
                                sqlCommand.Parameters.Add("@Seq", SqlDbType.BigInt).Value = seq - 1;
                                sqlCommand.Parameters.Add("@NewSeq", SqlDbType.BigInt).Value = seq;
                                if (sqlConnection.State != ConnectionState.Open)
                                    sqlConnection.Open();

                                rowsUpdated = sqlCommand.ExecuteNonQuery();
                            }
                        }
                        finally
                        {
                            if (rowsUpdated > 0)
                            {
                                transaction.Commit();
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw ex;
            }
            if (rowsUpdated > 0)
            {
                return seq;
            }
            else
            {
                throw new ApplicationException($"failed to get nextsequence number for {tableName}");
            }
        }

        /*
        * Part of code to handle renaming of the ChangeInID Shortcuts
        * Code changed on 19th October 2023 
        * Developer: Ruchir Dhiman
        */
        public static long getShortcutDataID(long parentID)
        {
            long shortcutDataID = 0;

            SqlDataReader reader = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand($"SELECT DataID FROM DTree WHERE OriginDataID = {parentID}", connection))
                    {

                        //command.Parameters.AddWithValue("@EFFECTIVE_DATE", dataToInsert["EFFECTIVE_DATE"]);
                        
                        connection.Open(); // Open SQL Connection

                        reader = command.ExecuteReader();

                        while( reader.Read())
                        {

                            Log.Info($"Shortcut for the DataID: {parentID} found.");
                            shortcutDataID = reader.GetInt64( reader.GetOrdinal("DataID") );
                            Log.Info($"Shortcut DataID: {shortcutDataID} found.");
                            break;
                        }

                        connection.Close(); // Close connection

                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(e.Message,e);
                Log.Error("Error occurred while trying to find the shortcut");
            }
            return shortcutDataID;
        }

        public static bool CheckIfAppointmentCumulusBatchExists()
        {
            Log.Info($"Check If New Appointment Cumulus Batch Exists from Z_ePfile_Appointment!");

            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConn"].ToString()))
            {
                using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetAppointmentCumulusBatch, sqlConnection))
                {
                    try
                    {
                        sqlCommand.CommandType = CommandType.Text;

                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                return true;
                            }
                        }
                        sqlConnection.Close();
                    }

                    catch (SqlException ex)
                    {
                        Log.Error(String.Format("SQL Exception while retrieving Appointment Cumulus Batch in Z_ePfile_Appointment.\n {0}", ex));
                    }
                    catch (InvalidOperationException ex)
                    {
                        Log.Error(String.Format("Invalid Operation Exception {0}", ex.Message), ex);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
            Log.Info("Cumulus Batch doesn't exist in Appointment Scenario, skipped post report generation!");
            return false;
        }

        public static bool CheckIfIDChangeCumulusBatchExists()
        {
            Log.Info($"Check If New Appointment Cumulus Batch Exists from Z_ePfile_IDChange!");

            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConn"].ToString()))
            {
                using (SqlCommand sqlCommand = new SqlCommand(Configuration.GetIDChangeCumulusBatch, sqlConnection))
                {
                    try
                    {
                        sqlCommand.CommandType = CommandType.Text;

                        sqlConnection.Open();

                        using (SqlDataReader rdr = sqlCommand.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                return true;
                            }
                        }
                        sqlConnection.Close();
                    }

                    catch (SqlException ex)
                    {
                        Log.Error(String.Format("SQL Exception while retrieving ID Change Cumulus Batch in Z_ePfile_IDChange.\n {0}", ex));
                    }
                    catch (InvalidOperationException ex)
                    {
                        Log.Error(String.Format("Invalid Operation Exception {0}", ex.Message), ex);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
            Log.Info("Cumulus Batch doesn't exist in ID change Scenario, skipped post report generation!");
            return false;
        }

    }
}
