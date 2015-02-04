using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCIFramework.Helper
{
    public class YoutubeImporter
    {
        private int _assessmentID;
        public YoutubeImporter(int assessmentID)
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
                Log.LogError("YoutubeImporter", ex);
                return false;
            }

        }
        private void Process()
        {

        }
    }
}
