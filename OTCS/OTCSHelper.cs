using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using ACEAutomationProcesses.Database;
using ACEAutomationProcesses.Model;
using RestSharp;

namespace ACEAutomationProcesses.OTCS
{
    static class OtcsHelper
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string AuthenticateServiceUser()
        {
            string ticket = String.Empty;
            Log.Debug("Authenticate using Rest");
            try
            {
                //Authenticate
                RestClient client = CreateClient(Configuration.Url);
                RestRequest request = new RestRequest("auth", Method.POST);
                request.AddParameter("username", Configuration.Username);
                request.AddParameter("password", Configuration.Password);

                IRestResponse<GenericModel> response = client.Execute<GenericModel>(request);

                Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        Log.Debug("Ticket Received : " + response.Content);
                        ticket = response.Data.Ticket;
                    }
                    else
                    {
                        Log.Error(
                            $"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                        //throw new AuthenticateException($"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                    }
                }
                else
                {
                    Log.Error(
                        $"Rest Request to authenticate failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                        response.ErrorException);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return ticket;
        }

        public static byte[] DownloadCsv(long dataId)
        {
            Log.Debug("DownloadCSV : " + dataId);
            try
            {
                //Authenticate

                string ticket = AuthenticateServiceUser();
                if (string.IsNullOrEmpty(ticket))
                {
                    return null;
                }

                RestClient client = CreateClient(Configuration.Url);
                //RestClient client = new RestClient(Configuration.Url + "nodes/" + dataId + "/content");
                RestRequest request = new RestRequest($"nodes/{dataId}/content", Method.GET);
                request.AddHeader("OTCSTicket", ticket);

                IRestResponse response = client.Execute(request);
                Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        byte[] fileData = response.RawBytes;
                        Log.Debug("File Downloaded Successfully");
                        return fileData;
                    }
                    else
                    {
                        Log.Error(
                            $"Failed to download {dataId} failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                        //throw new AuthenticateException($"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                    }
                }
                else
                {
                    Log.Error(
                        $"Failed to download {dataId} failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                        response.ErrorException);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return null;
        }

        public static long CopyNode(long destId, long sourceId, string newName)
        {
            Log.Debug($"Copy Node {sourceId} to {destId} with name {newName}");
            try
            {
                //Authenticate

                string ticket = AuthenticateServiceUser();
                if (string.IsNullOrEmpty(ticket))
                {
                    return 0L;
                }

                RestClient client = CreateClient(Configuration.Url);
                //RestClient client = new RestClient(Configuration.Url + "nodes/");
                RestRequest request = new RestRequest("nodes/", Method.POST);
                request.AddHeader("OTCSTicket", ticket);
                request.AddParameter("original_id", sourceId);
                request.AddParameter("parent_id", destId);
                request.AddQueryParameter("name", newName);

                IRestResponse<RootObject> response = client.Execute<RootObject>(request);

                Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        long id = Convert.ToInt64(response.Data?.Data[0]?.Items[0]?.Id);
                        Log.Debug("Node successfully copied : " + id);
                        return id;
                    }
                    else
                    {
                        Log.Error(
                            $"Rest Request to copy node {sourceId} failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                        //throw new AuthenticateException($"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                    }
                }
                else
                {
                    Log.Error(
                        $"Rest Request to copy node {sourceId} failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                        response.ErrorException);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return 0L;
        }

        public static bool RemoveClassification(long nodeId, long classId, string updateCreationDate)
        {
            Log.Debug($"Remove Classification {classId} from {nodeId} using {Configuration.WrRemoveClass}");

            try
            {
                //Authenticate

                string ticket = AuthenticateServiceUser();
                if (string.IsNullOrEmpty(ticket))
                {
                    return false;
                }

                RestClient client = CreateClient(Configuration.Url);
                //RestClient client = new RestClient(Configuration.Url + "nodes/" + Configuration.WrRemoveClass + "/output");
                RestRequest request = new RestRequest($"nodes/{Configuration.WrRemoveClass}/output", Method.GET);
                request.AddHeader("OTCSTicket", ticket);
                request.AddParameter("nodeId", nodeId);
                request.AddParameter("classId", classId);
                request.AddQueryParameter("updateCreationDate", updateCreationDate);

                IRestResponse response = client.Execute(request);

                Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        Log.Debug("Classification removed successfully");
                        return true;
                    }
                    else
                    {
                        Log.Error(
                            $"Rest Request to remove classification failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                        //throw new AuthenticateException($"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                    }
                }
                else
                {
                    Log.Error(
                        $"Rest Request to remove classification failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                        response.ErrorException);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return false;
        }

        public static bool AddNricFolderCategories(long nodeId, AppointmentScenario scenario)
        {
            Log.Debug("Add Category NRIC Workspace categories to Node " + nodeId);
            Boolean success = false;
            try
            {
                //Authenticate
                string ticket = AuthenticateServiceUser();

                Log.Debug("Applying : " + Configuration.EPfileNricfinCategory);

                RestClient client = CreateClient(Configuration.Url);
                //RestClient client = new RestClient(Configuration.Url + "nodes/" + nodeId + "/categories");
                RestRequest request = new RestRequest($"nodes/{nodeId}/categories", Method.POST);
                request.AddHeader("OTCSTICKET", ticket);
                List<CategoryModel> categoryAttrs =
                    DbHelper.GetCategoryDetails(Convert.ToInt64(Configuration.EPfileNricfinCategory));

                //Dictionary<String, String> attributes = new Dictionary<string, string>();
                request.AddParameter("category_id", Configuration.EPfileNricfinCategory);
                foreach (CategoryModel categoryAttr in categoryAttrs)
                {
                    categoryAttr.AttrRegion = categoryAttr.AttrRegion.Replace("Attr_", "");
                    if (categoryAttr.AttrName.Equals(Configuration.EPfileWkspNricAttr))
                    {
                        request.AddParameter(categoryAttr.AttrRegion, scenario.Id);
                    }

                    if (categoryAttr.AttrName.Equals(Configuration.EPfileWkspNameAttr))
                    {
                        request.AddParameter(categoryAttr.AttrRegion, scenario.Name);
                    }
                }

                //request.AddJsonBody(attributes);

                IRestResponse<GenericModel> response = client.Execute<GenericModel>(request);

                Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        Log.Debug("Successfully applied : " + Configuration.EPfileNricfinCategory);
                        success = true;
                    }
                    else
                    {
                        Log.Error(
                            $"Rest Request to apply {Configuration.EPfileNricfinCategory} failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                        success = false;
                        //throw new AuthenticateException($"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                    }
                }
                else
                {
                    Log.Error(
                        $"Rest Request to apply {Configuration.EPfileNricfinCategory} failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                        response.ErrorException);
                    success = false;
                }

                if (success)
                {
                    Log.Debug("Successfully applied : " + Configuration.EPfileNricfinCategory);

                    Log.Debug("Applying : " + Configuration.EPfileWorkspaceCategory);

                    client = CreateClient(Configuration.Url);
                    //client = new RestClient(Configuration.Url + "nodes/" + nodeId + "/categories");
                    request = new RestRequest($"nodes/{nodeId}/categories", Method.POST);
                    request.AddHeader("OTCSTICKET", ticket);
                    categoryAttrs = DbHelper.GetCategoryDetails(Convert.ToInt64(Configuration.EPfileWorkspaceCategory));

                    //attributes = new Dictionary<string, string>();
                    request.AddParameter("category_id", Configuration.EPfileWorkspaceCategory);
                    foreach (CategoryModel categoryAttr in categoryAttrs)
                    {
                        categoryAttr.AttrRegion = categoryAttr.AttrRegion.Replace("Attr_", "");
                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceAgencyAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, scenario.Agency);
                        }

                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceDivStatusAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, scenario.DivisionText);
                        }

                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceSchemeOfServiceAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, scenario.SchemeOfServiceDescription);
                        }

                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceStatusAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, "Active");
                        }

                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspacePensionAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, "");
                        }
                    }

                    //request.AddJsonBody(attributes);

                    response = client.Execute<GenericModel>(request);

                    Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                    if (response.ResponseStatus == ResponseStatus.Completed)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                            response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            Log.Debug("Successfully applied : " + Configuration.EPfileWorkspaceCategory);
                            success = true;
                        }
                        else
                        {
                            Log.Error(
                                $"Rest Request to apply {Configuration.EPfileWorkspaceCategory} failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                            success = false;
                            //throw new AuthenticateException($"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                        }
                    }
                    else
                    {
                        Log.Error(
                            $"Rest Request to apply {Configuration.EPfileWorkspaceCategory} failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                            response.ErrorException);
                        success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                success = false;
            }

            return success;
        }

        public static bool UpdateNricFolderCategories(long nodeId, AppointmentScenario scenario)
        {
            Log.Debug("Update NRIC categories for Node " + nodeId);
            Boolean success = false;
            try
            {
                //Authenticate
                string ticket = AuthenticateServiceUser();

                Log.Debug("Updating " + Configuration.EPfileNricfinCategory);

                RestClient client = CreateClient(Configuration.Url);
                //RestClient client = new RestClient(Configuration.Url + "nodes/" + nodeId + "/categories/" + Configuration.EPfileNricfinCategory);
                RestRequest request =
                    new RestRequest($"nodes/{nodeId}/categories/{Configuration.EPfileNricfinCategory}", Method.PUT);
                request.AddHeader("OTCSTICKET", ticket);
                List<CategoryModel> categoryAttrs =
                    DbHelper.GetCategoryDetails(Convert.ToInt64(Configuration.EPfileNricfinCategory));

                //Dictionary<string, string> attributes = new Dictionary<string, string>();
                request.AddParameter("category_id", Configuration.EPfileNricfinCategory);
                request.AddParameter("id", nodeId);
                foreach (CategoryModel categoryAttr in categoryAttrs)
                {
                    categoryAttr.AttrRegion = categoryAttr.AttrRegion.Replace("Attr_", "");
                    if (categoryAttr.AttrName.Equals(Configuration.EPfileWkspNricAttr))
                    {
                        request.AddParameter(categoryAttr.AttrRegion, scenario.Id);
                    }

                    if (categoryAttr.AttrName.Equals(Configuration.EPfileWkspNameAttr))
                    {
                        request.AddParameter(categoryAttr.AttrRegion, scenario.Name);
                    }
                }

                //request.AddJsonBody(attributes);

                IRestResponse<GenericModel> response = client.Execute<GenericModel>(request);

                Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        Log.Debug("Successfully applied : " + Configuration.EPfileNricfinCategory);
                        success = true;
                    }
                    else
                    {
                        Log.Error(
                            $"Rest Request to update {Configuration.EPfileNricfinCategory} failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                        success = false;
                        //throw new AuthenticateException($"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                    }
                }
                else
                {
                    Log.Error(
                        $"Rest Request to update {Configuration.EPfileNricfinCategory} failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                        response.ErrorException);
                    success = false;
                }

                if (success)
                {
                    Log.Debug("Successfully Updated " + Configuration.EPfileNricfinCategory);
                    Log.Debug("Updating " + Configuration.EPfileWorkspaceCategory);

                    client = CreateClient(Configuration.Url);
                    //client = new RestClient(Configuration.Url + "nodes/" + nodeId + "/categories/" + Configuration.EPfileWorkspaceCategory);
                    request = new RestRequest($"nodes/{nodeId}/categories/{Configuration.EPfileWorkspaceCategory}",
                        Method.PUT);
                    request.AddHeader("OTCSTICKET", ticket);
                    categoryAttrs = DbHelper.GetCategoryDetails(Convert.ToInt64(Configuration.EPfileWorkspaceCategory));

                    //attributes = new Dictionary<string, string>();
                    request.AddParameter("category_id", Configuration.EPfileWorkspaceCategory);
                    request.AddParameter("id", nodeId);
                    foreach (CategoryModel categoryAttr in categoryAttrs)
                    {
                        categoryAttr.AttrRegion = categoryAttr.AttrRegion.Replace("Attr_", "");
                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceAgencyAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, scenario.Agency);
                        }

                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceDivStatusAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, scenario.DivisionText);
                        }

                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceSchemeOfServiceAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, scenario.SchemeOfServiceDescription);
                        }

                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceStatusAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, "Active");
                        }

                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspacePensionAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, "");
                        }

                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceExitDateAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, "");
                        }
                    }

                    //request.AddJsonBody(attributes);

                    response = client.Execute<GenericModel>(request);

                    Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                    if (response.ResponseStatus == ResponseStatus.Completed)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                            response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            Log.Debug("Successfully updated : " + Configuration.EPfileWorkspaceCategory);
                            success = true;
                        }
                        else
                        {
                            Log.Error(
                                $"Rest Request to update {Configuration.EPfileWorkspaceCategory} failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                            success = false;
                        }
                    }
                    else
                    {
                        Log.Error(
                            $"Rest Request to update {Configuration.EPfileWorkspaceCategory} failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                            response.ErrorException);
                        success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                success = false;
            }

            return success;
        }


        public static bool UpdateExitAttributes(long nodeId, ExitScenario scenario)
        {
            Log.Debug("Update NRIC categories for Exit Scenario " + nodeId);
            try
            {
                //Authenticate
                string ticket = AuthenticateServiceUser();


                Log.Debug("Updating " + Configuration.EPfileWorkspaceCategory);

                RestClient client = CreateClient(Configuration.Url);
                //RestClient client = new RestClient(Configuration.Url + "nodes/" + nodeId + "/categories/" + Configuration.EPfileWorkspaceCategory);
                RestRequest request =
                    new RestRequest($"nodes/{nodeId}/categories/{Configuration.EPfileWorkspaceCategory}", Method.PUT);
                request.AddHeader("OTCSTICKET", ticket);
                List<CategoryModel> categoryAttrs =
                    DbHelper.GetCategoryDetails(Convert.ToInt64(Configuration.EPfileWorkspaceCategory));

                //Dictionary<string, string> attributes = new Dictionary<string, string>();
                request.AddParameter("category_id", Configuration.EPfileWorkspaceCategory);
                request.AddParameter("id", nodeId);
                foreach (CategoryModel categoryAttr in categoryAttrs)
                {
                    categoryAttr.AttrRegion = categoryAttr.AttrRegion.Replace("Attr_", "");
                    if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceStatusAttr))
                    {
                        request.AddParameter(categoryAttr.AttrRegion, "Inactive");
                    }

                    if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceExitDateAttr))
                    {
                        string date = scenario.EffectiveDate?.ToString("yyyy-MM-ddTHH\\:mm\\:ss");
                        request.AddParameter(categoryAttr.AttrRegion, date);
                    }
                }

                //request.AddJsonBody(attributes);

                IRestResponse<GenericModel> response = client.Execute<GenericModel>(request);

                Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        Log.Debug(
                            $"Successfully Updated attributes of {Configuration.EPfileWorkspaceCategory} for exit scenario");
                        return true;
                    }
                    else
                    {
                        Log.Error(
                            $"Rest Request to update attributes failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                        //throw new AuthenticateException($"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                    }
                }
                else
                {
                    Log.Error(
                        $"Rest Request to update attributes failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                        response.ErrorException);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return false;
        }

        public static bool UpdateChangeInMdAttributes(long nodeId, ChangeMdScenario scenario)
        {
            Log.Debug("Update NRIC categories for ChangeInMD Scenario " + nodeId);
            try
            {
                //Authenticate
                string ticket = AuthenticateServiceUser();

                RestClient client = null;
                RestRequest request = null;

                if (scenario.ProcessIndicator.Equals("Change in Name") && !string.IsNullOrEmpty(scenario.Name))
                {
                    Log.Debug("Updating Name : " + Configuration.EPfileNricfinCategory);

                    client = CreateClient(Configuration.Url);
                    //client = new RestClient(Configuration.Url + "nodes/" + nodeId + "/categories/" + Configuration.EPfileNricfinCategory);
                    request = new RestRequest($"nodes/{nodeId}/categories/{Configuration.EPfileNricfinCategory}",
                        Method.PUT);
                    request.AddHeader("OTCSTICKET", ticket);

                    List<CategoryModel> categoryAttrs =
                        DbHelper.GetCategoryDetails(Convert.ToInt64(Configuration.EPfileNricfinCategory));

                    //Dictionary<string, string> attributes = new Dictionary<string, string>();
                    request.AddParameter("category_id", Configuration.EPfileNricfinCategory);
                    request.AddParameter("id", nodeId);
                    foreach (CategoryModel categoryAttr in categoryAttrs)
                    {
                        categoryAttr.AttrRegion = categoryAttr.AttrRegion.Replace("Attr_", "");
                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWkspNameAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, scenario.Name.Trim());
                            break;
                        }
                    }

                    //request.AddJsonBody(attributes);
                }

                if (scenario.ProcessIndicator.Equals("Change in Divisional Status") &&
                    !string.IsNullOrEmpty(scenario.DivisionText))
                {
                    Log.Debug("Updating Divisional Status : " + Configuration.EPfileWorkspaceCategory);

                    client = CreateClient(Configuration.Url);
                    //client = new RestClient(Configuration.Url + "nodes/" + nodeId + "/categories/" + Configuration.EPfileWorkspaceCategory);
                    request = new RestRequest($"nodes/{nodeId}/categories/{Configuration.EPfileWorkspaceCategory}",
                        Method.PUT);
                    request.AddHeader("OTCSTICKET", ticket);

                    List<CategoryModel> categoryAttrs =
                        DbHelper.GetCategoryDetails(Convert.ToInt64(Configuration.EPfileWorkspaceCategory));

                    //Dictionary<string, string> attributes = new Dictionary<string, string>();
                    request.AddParameter("category_id", Configuration.EPfileWorkspaceCategory);
                    request.AddParameter("id", nodeId);
                    foreach (CategoryModel categoryAttr in categoryAttrs)
                    {
                        categoryAttr.AttrRegion = categoryAttr.AttrRegion.Replace("Attr_", "");
                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceDivStatusAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, scenario.DivisionText.Trim());
                            break;
                        }
                    }

                    //request.AddJsonBody(attributes);
                }

                if (scenario.ProcessIndicator.Equals("Change in Scheme of Service") &&
                    !string.IsNullOrEmpty(scenario.SchemeOfServiceDescription))
                {
                    Log.Debug("Updating Scheme of Service : " + Configuration.EPfileWorkspaceCategory);

                    client = CreateClient(Configuration.Url);
                    //client = new RestClient(Configuration.Url + "nodes/" + nodeId + "/categories/" + Configuration.EPfileWorkspaceCategory);
                    request = new RestRequest($"nodes/{nodeId}/categories/{Configuration.EPfileWorkspaceCategory}",
                        Method.PUT);
                    request.AddHeader("OTCSTICKET", ticket);

                    List<CategoryModel> categoryAttrs =
                        DbHelper.GetCategoryDetails(Convert.ToInt64(Configuration.EPfileWorkspaceCategory));

                    //Dictionary<string, string> attributes = new Dictionary<string, string>();
                    request.AddParameter("category_id", Configuration.EPfileWorkspaceCategory);
                    request.AddParameter("id", nodeId);
                    foreach (CategoryModel categoryAttr in categoryAttrs)
                    {
                        categoryAttr.AttrRegion = categoryAttr.AttrRegion.Replace("Attr_", "");
                        if (categoryAttr.AttrName.Equals(Configuration.EPfileWorkspaceSchemeOfServiceAttr))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, scenario.SchemeOfServiceDescription.Trim());
                            break;
                        }
                    }

                    //request.AddJsonBody(attributes);
                }

                if (client == null)
                {
                    Log.Warn(
                        $"Not a valid process indicator :: {scenario.ProcessIndicator} or not valid values for process");
                    return false;
                }

                IRestResponse<GenericModel> response = client.Execute<GenericModel>(request);

                Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        Log.Debug(
                            $"Successfully Updated attributes of {Configuration.EPfileWorkspaceCategory} for change metadata scenario");
                        return true;
                    }
                    else
                    {
                        Log.Error(
                            $"Rest Request to update attributes failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                        //throw new AuthenticateException($"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                    }
                }
                else
                {
                    Log.Error(
                        $"Rest Request to update attributes failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                        response.ErrorException);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return false;
        }

        public static bool UpdateChangeIdForDocument(long nodeId, ChangeIdScenario scenario)
        {
            Log.Debug("Update NRIC category for Document for ChangeInMD Scenario " + nodeId);
            try
            {
                //Authenticate
                string ticket = AuthenticateServiceUser();

                Log.Debug("Updating Name : " + Configuration.EPfileNricfinCategory);

                RestClient client = CreateClient(Configuration.Url);
                //RestClient client = new RestClient(Configuration.Url + "nodes/" + nodeId + "/categories/" + Configuration.VitalGeneralCategory);
                RestRequest request = new RestRequest($"nodes/{nodeId}/categories/{Configuration.VitalGeneralCategory}",
                    Method.PUT);
                request.AddHeader("OTCSTICKET", ticket);

                List<CategoryModel> categoryAttrs =
                    DbHelper.GetCategoryDetails(Convert.ToInt64(Configuration.VitalGeneralCategory));

                // Dictionary<string, string> attributes = new Dictionary<string, string>();
                request.AddParameter("category_id", Configuration.VitalGeneralCategory);
                request.AddParameter("id", nodeId);
                foreach (CategoryModel categoryAttr in categoryAttrs)
                {
                    categoryAttr.AttrRegion = categoryAttr.AttrRegion.Replace("Attr_", "");
                    if (categoryAttr.AttrName.Equals(Configuration.VitalGeneralIdAttr))
                    {
                        string existingId = DbHelper.GetDocumentNric(nodeId);
                        if (string.IsNullOrEmpty(existingId))
                        {
                            request.AddParameter(categoryAttr.AttrRegion, scenario.NewId);
                        }
                        else
                        {
                            string newId = existingId.Replace(scenario.OldId, scenario.NewId);
                            request.AddParameter(categoryAttr.AttrRegion, newId);
                        }
                    }
                }

                //request.AddJsonBody(attributes);

                IRestResponse<GenericModel> response = client.Execute<GenericModel>(request);

                Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        Log.Debug(
                            $"Successfully Updated attributes of {Configuration.VitalGeneralCategory} for change id scenario");
                        return true;
                    }
                    else
                    {
                        Log.Error(
                            $"Rest Request to update attributes failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                        //throw new AuthenticateException($"Rest Request to authenticate failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                    }
                }
                else
                {
                    Log.Error(
                        $"Rest Request to update attributes failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}",
                        response.ErrorException);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return false;
        }

        private static RestClient CreateClient(string baseUrl)
        {
            Log.Info("***Create OTCS Rest Client***");
            RestClient client = new RestClient(baseUrl);

            Int32.TryParse(Configuration.RestReadTimeout, out int readTimeout);
            if (readTimeout != 0) client.ReadWriteTimeout = readTimeout * 1000;

            Int32.TryParse(Configuration.RestTimeout, out int timeout);
            if (readTimeout != 0) client.Timeout = timeout * 1000;

            return client;
        }


        /*
        * Part of code to handle renaming of the ChangeInID Shortcuts
        * Code changed on 19th October 2023 
        * Developer: Ruchir Dhiman
        */

        public static void renameshortcutNode( long parentID , String newNricID)
        {

            String ticket = null;

            long shortcutDataID = 0;
            RestRequest renameRequest = null;
            IRestResponse<GenericModel> renameResponse = null;

            Log.Info("Function renameNode START");
            Log.Info($"DataID: {parentID} -> New Name: {newNricID}");

            try
            {

                ticket = AuthenticateServiceUser();

                if (ticket == null)
                {
                    Log.Debug("Could not get OTCS Ticket, returning 'false' from here.");
                    return ; // Return null 
                }

                shortcutDataID = DbHelper.getShortcutDataID(parentID);

                if(shortcutDataID == 0)
                {
                    Log.Info("No shortcut found. Will return from here");
                    return;
                }

                renameRequest = new RestRequest($"nodes/{shortcutDataID}", Method.PUT);

                renameRequest.AddHeader("OTCSTicket", ticket);
                renameRequest.AddParameter("id", shortcutDataID);
                renameRequest.AddParameter("name", newNricID);


                renameResponse = CreateClient(Configuration.Url).Execute<GenericModel>(renameRequest);

                if (renameResponse != null && renameResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Log.Error("Unauthorized: Code will attempt to get OTCSTicket one more time.");
                    ticket = AuthenticateServiceUser();

                    Log.Debug($"Re-attempting rename the node {shortcutDataID}");

                    renameRequest.AddHeader("OTCSTicket", ticket);

                    renameResponse = CreateClient(Configuration.Url).Execute<GenericModel>(renameRequest);

                    if (renameResponse != null && renameResponse.StatusCode != HttpStatusCode.Unauthorized &&
                    renameResponse.StatusCode == HttpStatusCode.OK && renameResponse.ResponseStatus == ResponseStatus.Completed)
                    {

                        Log.Info("Node Renamed Successfully.");

                        return;
                    }
                    else
                    {
                        Log.Info("Node Renamed Failed.");
                        Log.Info($"Content: {renameResponse.Content}\n {renameResponse.StatusCode} -> {renameResponse.StatusDescription}");

                        return ;
                    }
                }

                if (renameResponse != null && renameResponse.StatusCode == HttpStatusCode.BadRequest)
                {

                    Log.Info("Node Renamed Failed.");
                    Log.Info($"Content: {renameResponse.Content}\n {renameResponse.StatusCode} -> {renameResponse.StatusDescription}");

                    return;
                }

                Log.Info("Node Renamed Successfully.");

            }
            catch(Exception e)
            {
                Log.Error(e.Message, e);
                Log.Error("Error occurred while attempting to rename the shortcut");
            }
        }

        public static void PostReportGeneration(string webReport)
        {
            try
            {
                string ticket = AuthenticateServiceUser();
                Log.Debug($"Calling WR : {webReport} to generate Post Report!");

                var client = new RestClient(Configuration.Url);
                var request = new RestRequest($"nodes/{webReport}/output", Method.GET);
                request.AddHeader("OTCSTICKET", ticket);
                IRestResponse<GenericModel> response = client.Execute<GenericModel>(request);
                

                Log.Debug($"ResponseStatus :: {response.ResponseStatus} -> HTTPStatus :: {response.StatusCode}");
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                    {
                        Log.Info("Post Report Generated in OTCS, Please check in Output folder!");
                    }
                    else
                    {
                        Log.Error($"Rest Request to generate post report failed with HTTPStatus :: {response.StatusCode} : {response.Content}");
                    }
                }
                else
                {
                    Log.Error($"Rest Request to generate post report failed with ResponseStatus :: {response.ResponseStatus} and Exception :: {response.ErrorMessage}");
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }
    }
}