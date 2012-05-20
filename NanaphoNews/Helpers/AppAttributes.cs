using Coding4Fun.Phone.Controls.Data;

namespace Helpers
{
    public class AppAttributes
    {
        public static string Version
        {
            get
            {
                return PhoneHelper.GetAppAttribute("Version");
            }
        }
    }
}
