using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCIFramework.Helper
{
    public class TwitterImporter
    {
        private int _assessmentID;
        public TwitterImporter(int assessmentID)
        {

        }
        public Boolean Start()
        {
            try
            {
                Process();
                return true;
            }
            catch
            {
                return false;
            }

        }
        private void Process()
        {

        }
    }
}
