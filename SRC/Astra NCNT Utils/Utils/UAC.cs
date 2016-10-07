using System;
using System.Security.Permissions;
using System.Security.Principal;

namespace Astra_NICNT_Utils.Utils
{
    public static class UAC
    {
        // Who's run my programm? 
        public static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
                //Console.WriteLine(ex);
            }
            catch (Exception)
            {
                isAdmin = false;
                //Console.WriteLine(ex);
            }

            return isAdmin;
        }
    }
}
