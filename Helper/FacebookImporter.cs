using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCIFramework.Helper
{
    public class FacebookImporter
    {
        private int _assessmentID;
        public FacebookImporter(int assessmentID)
        {

        }
        public Boolean Start()
        {
            try
            {
                Process();
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError("FacebookImporter", ex);
                return false;
            }

        }
        private void Process()
        {

        }
    }
}
