using ACEAutomationProcesses.AuthService;
using log4net;
using System.ServiceModel;

namespace ACEAutomationProcesses.CWS
{
    static class AuthenticationUtils
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        

        public static string AuthenticateUser()
        {

            Log.Debug("Authenticate User with CWS");
            // Create the Authentication service client
            AuthenticationClient authClient = new AuthenticationClient();

            // Store the authentication token
            string authToken;

            // Call the AuthenticateUser() method to get an authentication token
            try
            {
                authToken = authClient.AuthenticateUser(Configuration.Username, Configuration.Password);

            }
            catch (FaultException e)
            {
                Log.Error($"Failed to get auth token for {Configuration.Username}");
                Log.Error(e.Message, e);
                return null;
            }
            finally
            {
                // Always close the client
                authClient.Close();
            }
            return authToken;

        }
    }
}
