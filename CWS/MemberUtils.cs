using ACEAutomationProcesses.MemberService;
using log4net;
using System.ServiceModel;

namespace ACEAutomationProcesses.CWS
{
    class MemberUtils
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Member GetNode(string memberName, string authToken)
        {
            Log.Debug("Get Member : " + memberName);

            // Create the DocumentManagement service client
            MemberServiceClient memberClient = new MemberServiceClient();

            // Create the OTAuthentication object and set the authentication token
            OTAuthentication otAuth = new OTAuthentication();
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = AuthenticationUtils.AuthenticateUser();
            }
            otAuth.AuthenticationToken = authToken;

            Member member;

            try
            {
                member = memberClient.GetMemberByLoginName(ref otAuth, memberName);
            }
            catch (FaultException e)
            {
                Log.Error($"Failed to get member by {memberName}");
                Log.Error(e.Message, e);
                return null;
            }
            finally
            {
                // Always close the client
                memberClient.Close();
            }

            return member;
        }

    }
}
