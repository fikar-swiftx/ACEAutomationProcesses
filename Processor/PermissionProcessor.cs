using System;
using System.Collections.Generic;
using ACEAutomationProcesses.CWS;
using ACEAutomationProcesses.Database;
using ACEAutomationProcesses.DocManService;

namespace ACEAutomationProcesses.Processor
{
    static class PermissionProcessor
    {
        private static readonly string RightTypeAcl = "ACL";

        // private static string _rightTypeOwner = "Owner";
        // private static string _rightTypeOwnerGroup = "OwnerGroup";
        // private static string _rightTypePublic = "Public";
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private static readonly NodePermissions
            ReadPerms = new() { SeePermission = true, SeeContentsPermission = true };

        private static readonly NodePermissions ModifyPerms = new()
            { SeePermission = true, SeeContentsPermission = true, ModifyPermission = true };

        private static readonly NodePermissions EditAttrPermissions = new()
        {
            SeePermission = true,
            SeeContentsPermission = true,
            ModifyPermission = true,
            EditAttributesPermission = true
        };

        private static readonly NodePermissions DELETE_PERMISSIONS = new()
        {

            DeletePermission = true,
            EditAttributesPermission = true,
            AddItemsPermission = true

        };

        public static void ProcessNhrPermissionForAppointment(long nricFolderId, string agency)
        {
            Log.Info($"Process appointment permissions for {nricFolderId} with agency : {agency}");

            ChildrenModel teFolder = DbHelper.GetChildByName(nricFolderId, 0, Configuration.TeFolderName);
            ChildrenModel nteFolder = DbHelper.GetChildByName(nricFolderId, 0, Configuration.NteFolderName);
            List<FolderAliasModel> folderAliases = DbHelper.GetFolderAlias(null);
            if (teFolder != null)
            {
                Log.Debug($"TE Folder ; {teFolder.DataId}");
                ProcessTePermission(teFolder.DataId, nricFolderId, agency, folderAliases);
            }

            if (nteFolder != null)
            {
                Log.Debug($"TE Folder ; {nteFolder.DataId}");
                ProcessNtePermission(nteFolder.DataId, nricFolderId, agency);
            }

            if (teFolder != null && nteFolder != null)
            {
                Log.Debug("Process L3 permission");
                ProcessLevel3Permission(nricFolderId, agency, folderAliases, teFolder.DataId, nteFolder.DataId);

                string hrGroupAlias = agency.Equals("MOE") ? "HPO" : agency.Equals("Vital") ? "" : "Vital";

                Log.Debug($"Process extra permission for Alias : {hrGroupAlias}");
                ProcessExtraPermission(nricFolderId, agency, folderAliases, teFolder.DataId, nteFolder.DataId,
                    hrGroupAlias);
            }
        }

        //Parameter agencyName added by Ruchir on 13 July 2023
        // To get the group per agency


        public static void ProcessDgsPermission(long nricFolderId,String agencyName)
        {

            Log.Info($"Process appointment permissions for {nricFolderId}");
            List<ChildrenModel> nricFolderChildren = DbHelper.GetAllChildrenByTypeAndAncestor(nricFolderId, -1);

            Log.Info($"Group to be found -> {String.Format(Configuration.ConfigPermName,agencyName)}"); // Added by Ruchir 13 July 2023

            MemberModel memberConfigPermName = DbHelper.GetMemberByName(String.Format(Configuration.ConfigPermName, agencyName));
            string authToken = AuthenticationUtils.AuthenticateUser();

            if (memberConfigPermName != null)
            {
                NodeRight right = new()
                {
                    Permissions = DELETE_PERMISSIONS,
                    RightID = memberConfigPermName.Id,
                    Type = RightTypeAcl
                };

                DocumentManagementUtils.AddPermissionToNode(nricFolderId, right, authToken);



                foreach (ChildrenModel child in nricFolderChildren)
                {
                    Log.Debug($"Process DGS permissions for {child.DataId}, Name {child.Name}");
                    DocumentManagementUtils.AddPermissionToNode(child.DataId, right, authToken);

                }
            }
            // apply dgs permission

        }


        private static void ProcessTePermission(long teFolderId, long nricFolderId, string agency,
            List<FolderAliasModel> folderAliases)
        {
            Log.Info($"Process TE permissions for {nricFolderId} with agency : {agency}");
            foreach (FolderAliasModel folderAlias in folderAliases)
            {
                Log.Debug($"FolderAlias : {folderAlias.Alias} with level : {folderAlias.Level}");
                string authToken = AuthenticationUtils.AuthenticateUser();
                if (folderAlias.Level != 4 /*|| !folderAlias.ExtraPermission.Equals("ALL")*/) continue;

                string groupContributor = Configuration.PermPrefix1 + agency + "_" + folderAlias.Alias +
                                          Configuration.PermSuffix1;
                string groupRead = Configuration.PermPrefix1 + agency + "_" + folderAlias.Alias +
                                   Configuration.PermSuffix2;

                MemberModel memberContributor = DbHelper.GetMemberByName(groupContributor);
                MemberModel memberRead = DbHelper.GetMemberByName(groupRead);

                if (memberContributor == null || memberRead == null) continue;

                ChildrenModel inclusionListFolder = DbHelper.GetChildByName(teFolderId, 0, folderAlias.FolderName);

                //NodePermissions perms = new NodePermissions { SeePermission = true, SeeContentsPermission = true };
                NodeRight right = new()
                {
                    Permissions = ReadPerms,
                    RightID = memberContributor.Id,
                    Type = RightTypeAcl
                };
                if (inclusionListFolder != null)
                {
                    DocumentManagementUtils.AddPermissionToNode(inclusionListFolder.DataId, right, authToken);
                    Log.Debug($"Groups {groupContributor} added to {inclusionListFolder.DataId}");
                }

                DocumentManagementUtils.AddPermissionToNode(teFolderId, right, authToken);
                DocumentManagementUtils.AddPermissionToNode(nricFolderId, right, authToken);
                Log.Debug($"Groups {groupContributor} added to {teFolderId} and {nricFolderId}");

                right = new NodeRight { Permissions = ReadPerms, RightID = memberRead.Id, Type = RightTypeAcl };
                if (inclusionListFolder != null)
                {
                    DocumentManagementUtils.AddPermissionToNode(inclusionListFolder.DataId, right, authToken);
                    Log.Debug($"Groups {groupRead} added to {inclusionListFolder.DataId}");
                }

                DocumentManagementUtils.AddPermissionToNode(teFolderId, right, authToken);
                DocumentManagementUtils.AddPermissionToNode(nricFolderId, right, authToken);
                Log.Debug($"Groups {groupRead} added to {teFolderId} and {nricFolderId}");

                if (inclusionListFolder != null)
                {
                    Log.Debug(
                        $"Groups {groupContributor} and  {groupRead} will be added to children of {inclusionListFolder.DataId}");

                    List<ChildrenModel> inclusionListFolderChildren =
                        DbHelper.GetAllChildrenByTypeAndAncestor(inclusionListFolder.DataId, -1);
                    foreach (ChildrenModel inclusionListFolderChild in inclusionListFolderChildren)
                    {
                        //perms = new NodePermissions();
                        //read group
                        //perms = new NodePermissions { SeePermission = true, SeeContentsPermission = true };
                        right = new NodeRight { RightID = memberRead.Id, Type = RightTypeAcl, Permissions = ReadPerms };
                        DocumentManagementUtils.AddPermissionToNode(inclusionListFolderChild.DataId, right,
                            authToken);

                        //contributor group
                        right = new NodeRight
                        {
                            RightID = memberContributor.Id,
                            Type = RightTypeAcl,
                            Permissions = inclusionListFolder.Subtype == Configuration.TypeDocument
                                ? ModifyPerms
                                : ReadPerms,
                        };
                        DocumentManagementUtils.AddPermissionToNode(inclusionListFolderChild.DataId, right,
                            authToken);
                    }
                }
            }
        }

        private static void ProcessNtePermission(long nteFolderId, long nricFolderId, string agency)
        {
            Log.Info($"Process NTE permissions for {nricFolderId} with agency : {agency}");
            string authToken = AuthenticationUtils.AuthenticateUser();

            string groupContributor =
                Configuration.PermPrefix1 + agency + Configuration.PermNte + Configuration.PermSuffix1;
            string groupRead = Configuration.PermPrefix1 + agency + Configuration.PermNte + Configuration.PermSuffix2;

            MemberModel memberContributor = DbHelper.GetMemberByName(groupContributor);
            MemberModel memberRead = DbHelper.GetMemberByName(groupRead);

            //NodePermissions perms = new NodePermissions { SeePermission = true, SeeContentsPermission = true };
            NodeRight right;
            if (memberContributor != null)
            {
                right = new NodeRight { Permissions = ReadPerms, RightID = memberContributor.Id, Type = RightTypeAcl };

                DocumentManagementUtils.AddPermissionToNode(nteFolderId, right, authToken);

                DocumentManagementUtils.AddPermissionToNode(nricFolderId, right, authToken);

                Log.Debug($"Groups {groupContributor} added to {nteFolderId} and {nricFolderId}");
            }

            if (memberRead != null)
            {
                right = new NodeRight { Permissions = ReadPerms, RightID = memberRead.Id, Type = RightTypeAcl };

                DocumentManagementUtils.AddPermissionToNode(nteFolderId, right, authToken);

                DocumentManagementUtils.AddPermissionToNode(nricFolderId, right, authToken);

                Log.Debug($"Groups {groupRead} added to {nteFolderId} and {nricFolderId}");
            }


            Log.Debug($"Groups {groupContributor} and  {groupRead} will be added to children of {nteFolderId}");

            List<ChildrenModel> nteFolderChildren = DbHelper.GetAllChildrenByTypeAndAncestor(nteFolderId, -1);
            foreach (ChildrenModel nteFolderChild in nteFolderChildren)
            {
                //read group
                //perms = new NodePermissions { SeePermission = true, SeeContentsPermission = true };
                if (memberRead != null)
                {
                    right = new NodeRight { Permissions = ReadPerms, RightID = memberRead.Id, Type = RightTypeAcl };

                    DocumentManagementUtils.AddPermissionToNode(nteFolderChild.DataId, right, authToken);
                }

                //contributor group
                if (memberContributor != null)
                {
                    right = new NodeRight
                    {
                        RightID = memberContributor.Id, Type = RightTypeAcl,
                        Permissions = nteFolderChild.Subtype == Configuration.TypeDocument ? ModifyPerms : ReadPerms
                    };

                    DocumentManagementUtils.AddPermissionToNode(nteFolderChild.DataId, right, authToken);
                }
            }
        }

        private static void ProcessLevel3Permission(long nricFolderId, string agency,
            List<FolderAliasModel> folderAliases, long teFolderId, long nteFolderId)
        {
            Log.Info($"Process L3 permissions for {nricFolderId} with agency : {agency}");
            string authToken = AuthenticationUtils.AuthenticateUser();

            foreach (FolderAliasModel folderAlias in folderAliases)
            {
                if (folderAlias.Level != 3) continue;

                string groupFunc = Configuration.PermPrefix1 + Configuration.PermFunc + agency + "_" +
                                   folderAlias.Alias;

                MemberModel memberFunction = DbHelper.GetMemberByName(groupFunc);

                ChildrenModel functionFolder = DbHelper.GetChildByName(nricFolderId, 0, folderAlias.FolderName);

                //NodePermissions perms = new NodePermissions { SeePermission = true, SeeContentsPermission = true };


                if (functionFolder != null && memberFunction != null)
                {
                    NodeRight right = new NodeRight
                    {
                        Permissions = ReadPerms,
                        RightID = memberFunction.Id,
                        Type = RightTypeAcl
                    };
                    DocumentManagementUtils.AddPermissionToNode(nricFolderId, right, authToken);
                    Log.Debug($"Group {groupFunc} added to {nricFolderId}");
                    DocumentManagementUtils.AddPermissionToNode(functionFolder.DataId, right, authToken);
                    Log.Debug($"Group {groupFunc} added to {functionFolder.DataId}");
                    List<ChildrenModel> functionFolderChildren =
                        DbHelper.GetAllChildrenByTypeAndAncestor(functionFolder.DataId, -1);
                    foreach (ChildrenModel functionFolderChild in functionFolderChildren)
                    {
                        //read group
                        right = new NodeRight
                            { RightID = memberFunction.Id, Type = RightTypeAcl, Permissions = EditAttrPermissions };

                        DocumentManagementUtils.AddPermissionToNode(functionFolderChild.DataId, right, authToken);
                    }

                    Log.Debug($"Group {groupFunc} added to children of {functionFolder.DataId}");


                    if (folderAlias.ExtraPermission.Equals("Y"))
                    {
                        Log.Debug($"Extra L3L4 permissions will be applied for {folderAlias.Alias}");
                        //perms = new NodePermissions { SeePermission = true, SeeContentsPermission = true };
                        //read group
                        right = new NodeRight
                            { RightID = memberFunction.Id, Type = RightTypeAcl, Permissions = ReadPerms };
                        DocumentManagementUtils.AddPermissionToNode(teFolderId, right, authToken);
                        ApplyL3L4ExtraPermisssion(teFolderId, memberFunction.Id, 4, "Y", folderAliases);

                        if (agency.Equals("MOE"))
                        {
                            DocumentManagementUtils.AddPermissionToNode(nteFolderId, right, authToken);
                            ApplyL3L4ExtraPermisssion(nteFolderId, memberFunction.Id, 4, "Y", folderAliases);
                        }
                    }
                }
            }
        }

        private static void ProcessExtraPermission(long nricFolderId, string agency,
            List<FolderAliasModel> folderAliases, long teFolderId, long nteFolderId, String alias)
        {
            Log.Info($"Process Extra permissions for {nricFolderId} with agency : {agency} for alias {alias}");

            string authToken = AuthenticationUtils.AuthenticateUser();

            foreach (FolderAliasModel folderAlias in folderAliases)
            {
                if (folderAlias.Level != 4) continue;

                string groupFunc = Configuration.PermPrefix1 + agency + "_" + folderAlias.Alias + "_" + alias;

                MemberModel memberAlias = DbHelper.GetMemberByName(groupFunc);

                if (memberAlias == null) continue;

                ChildrenModel inclusionListFolder = DbHelper.GetChildByName(teFolderId, 0, folderAlias.FolderName);

                //NodePermissions perms = new NodePermissions { SeePermission = true, SeeContentsPermission = true };

                NodeRight right = new()
                {
                    Permissions = ReadPerms,
                    RightID = memberAlias.Id,
                    Type = RightTypeAcl
                };

                DocumentManagementUtils.AddPermissionToNode(teFolderId, right, authToken);
                DocumentManagementUtils.AddPermissionToNode(nricFolderId, right, authToken);
                if (inclusionListFolder != null)
                {
                    DocumentManagementUtils.AddPermissionToNode(inclusionListFolder.DataId, right, authToken);

                    List<ChildrenModel> inclusionListFolderChildren =
                        DbHelper.GetAllChildrenByTypeAndAncestor(inclusionListFolder.DataId, -1);
                    foreach (ChildrenModel inclusionListFolderChild in inclusionListFolderChildren)
                    {
                        //read group
                        right = new NodeRight
                        {
                            RightID = memberAlias.Id, Type = RightTypeAcl,
                            Permissions = inclusionListFolderChild.Subtype == Configuration.TypeDocument
                                ? ModifyPerms
                                : ReadPerms
                        };

                        DocumentManagementUtils.AddPermissionToNode(inclusionListFolderChild.DataId, right, authToken);
                    }
                }

                if (agency == "MOE")
                {
                    Log.Debug($"NTE Folder : {nteFolderId}");
                    //unimplemented, MOE is not ACE agency
                }
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private static void ApplyL3L4ExtraPermisssion(long folderId, long rightId, int level, string extraPermission,
            List<FolderAliasModel> folderAliases)
        {
            Log.Info($"Process L3L4 permissions for {folderId} with right : {rightId}");
            string authToken = AuthenticationUtils.AuthenticateUser();

            foreach (FolderAliasModel folderAlias in folderAliases)
            {
                if (folderAlias.Level != level || !folderAlias.ExtraPermission.Equals("Y")) continue;

                ChildrenModel child = DbHelper.GetChildByName(folderId, -1, folderAlias.FolderName);

                if (child == null) continue;

                //NodePermissions perms = new NodePermissions { SeePermission = true, SeeContentsPermission = true };

                NodeRight right = new() { RightID = rightId, Type = RightTypeAcl, Permissions = ReadPerms };

                DocumentManagementUtils.AddPermissionToNode(child.DataId, right, authToken);

                List<ChildrenModel> leafChildren = DbHelper.GetAllChildrenByTypeAndAncestor(child.DataId, -1);

                foreach (ChildrenModel leaf in leafChildren)
                {
                    right = new NodeRight
                    {
                        RightID = rightId, Type = RightTypeAcl,
                        Permissions = leaf.Subtype == Configuration.TypeDocument ? ModifyPerms : ReadPerms
                    };

                    DocumentManagementUtils.AddPermissionToNode(leaf.DataId, right, authToken);
                }
            }
        }

        public static Boolean RemovedAssignedAccess(Node container)
        {
            Log.Info($"RemovedAssignedAccess called for {container.ID}");
            Boolean success;

            string authToken = AuthenticationUtils.AuthenticateUser();

            NodeRights rights = DocumentManagementUtils.GetPermissions(container.ID, authToken);
            if (rights != null)
            {
                rights.ACLRights = new NodeRight[] { };
                success = DocumentManagementUtils.UpdateRightsToNode(container.ID, rights, authToken);
                Log.Info($"Removed Assigned Access for {container.ID} is {success}");
            }
            else
            {
                Log.Warn($"Unable to get rights for {container.ID}");
                success = false;
            }

            if (success)
            {
                /*GetNodesInContainerOptions options = new GetNodesInContainerOptions
                {
                    MaxDepth = 1,
                    MaxResults = 50
                };*/
                //List<Node> children = DocumentManagementUtils.GetChildNodes(container.ID, authToken, options);

                List<ChildrenModel> children = DbHelper.GetAllChildrenByTypeAndAncestor(container.ID, 0);

                foreach (ChildrenModel child in children)
                {
                    rights = DocumentManagementUtils.GetPermissions(container.ID, authToken);
                    if (rights != null)
                    {
                        rights.ACLRights = new NodeRight[] { };
                        success = DocumentManagementUtils.UpdateRightsToNode(child.DataId, rights, authToken);
                        Log.Info($"Removed Assigned Access for {child.DataId} is {success}");
                    }
                    else
                    {
                        Log.Warn($"Unable to get rights for {child.DataId}");
                        success = false;
                    }

                    if (!success) break;
                }

                /*foreach (Node child in children)
                {
                    success = RemovedAssignedAccess(child);
                    if (!success)
                        break;
                }*/
            }

            return success;
        }
    }
}