using ACEAutomationProcesses.DocManService;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ACEAutomationProcesses.CWS
{
    static class DocumentManagementUtils
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void AddPermissionToNode(long nodeId, NodeRight right, string authToken)
        {
            // Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;


            try
            {
                docManClient.AddNodeRight(ref otAuth, nodeId, right);
            }
            catch (FaultException e)
            {
                Log.Error("Failed!");
                Log.Error(e.Message, e);
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }


        }

        public static Boolean UpdateRightsToNode(long nodeId, NodeRights rights, string authToken)
        {
            // Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;

            try
            {
                docManClient.SetNodeRights(ref otAuth, nodeId, rights);
            }
            catch (FaultException e)
            {
                Log.Error("Failed!");
                Log.Error(e.Message, e);
                return false;
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }
            return true;
        }

        public static NodeRights GetPermissions(long nodeId, string authToken)
        {
            // Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;

            NodeRights rights = null;
            try
            {
                rights = docManClient.GetNodeRights(ref otAuth, nodeId);
            }
            catch (FaultException e)
            {
                Log.Error("Failed!");
                Log.Error(e.Message, e);
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }

            return rights;
        }


        public static Node GetNode(long nodeId, string authToken)
        {
            Log.Debug("Get Node : " + nodeId);

            // Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;

            Node node;

            try
            {
                node = docManClient.GetNode(ref otAuth, nodeId);
            }
            catch (FaultException e)
            {
                Log.Error("Failed!");
                Log.Error(e.Message, e);
                return null;
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }

            return node;
        }

        public static Node CopyNode(long sourceId, long destinationId, string newName, string authToken, CopyOptions copyOptions)
        {
            Log.Debug("Copy Node from : " + sourceId + " to " + destinationId + " with name : " + newName);

            // Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;

            Node node;

            try
            {
                node = docManClient.CopyNode(ref otAuth, sourceId, destinationId, newName, copyOptions);
            }
            catch (FaultException e)
            {
                Log.Error("Failed!");
                Log.Error(e.Message, e);
                return null;
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }

            return node;
        }

        public static List<Node> GetChildNodes(long nodeID, string authToken, GetNodesInContainerOptions options)
        {
            Log.Debug($"Get Children for Node : {nodeID}");

            // Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;

            List<Node> nodes = new List<Node>();

            try
            {

                nodes = docManClient.GetNodesInContainer(ref otAuth, nodeID, options).ToList();
            }
            catch (FaultException e)
            {
                Log.Error("Failed!");
                Log.Error(e.Message, e);
                return null;
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }

            return nodes;
        }

        public static bool DeleteNode(long nodeId, string authToken)
        {
            Log.Debug("Delete Node : " + nodeId);

            // Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;

            //Node node = null;

            try
            {

                docManClient.DeleteNode(ref otAuth, nodeId);
                return true;
            }
            catch (FaultException e)
            {
                Log.Error("Failed!");
                Log.Error(e.Message, e);
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }

            return false;
        }

        public static bool MoveNode(long nodeId, long parentId, string newName, string authToken, MoveOptions options)
        {
            Log.Debug("Move Node : " + nodeId);

            // Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;

            //Node node = null;

            try
            {

                docManClient.MoveNode(ref otAuth, nodeId, parentId, newName, options);
                return true;
            }
            catch (FaultException e)
            {
                Log.Error("Failed!");
                Log.Error(e.Message, e);
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }
            return false;
        }

        public static bool RenameNode(long nodeId, string newName, string authToken)
        {
            Log.Debug("Rename Node : " + nodeId + " : " + newName);

            // Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;

            //Node node = null;

            try
            {

                docManClient.RenameNode(ref otAuth, nodeId, newName);
                return true;
            }
            catch (FaultException e)
            {
                Log.Error("Failed!");
                Log.Error(e.Message, e);
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }
            return false;
        }

        public static bool SaveNode(Node node, string authToken)
        {
            Log.Debug("Save Node : " + node.ID);

            // Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;

            //Node node = null;

            try
            {

                docManClient.UpdateNode(ref otAuth, node);
                return true;
            }
            catch (FaultException e)
            {
                Log.Error("Failed!");
                Log.Error(e.Message, e);
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }
            return false;
        }

    }
}
