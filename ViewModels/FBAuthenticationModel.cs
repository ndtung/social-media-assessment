using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using MCIFramework.Models;

namespace MCIFramework.ViewModels
{
    public class FBAuthenticationModel : ViewModelBase, IPageViewModel
    {
        private ICommand _navigatedCommand;
        private Database _dbContext = new Database();
        private Uri _browserUri;
        private ICommand _cancelAuthCommand;

        public Uri BrowserUri
        {
            get { return _browserUri; }
            set
            {
                if (_browserUri != value)
                {
                    _browserUri = value;
                    OnPropertyChanged("BrowserUri");
                }
            }
        }
        public string Name
        {
            get { return "FBAuthentication"; }
        }


        public FBAuthenticationModel()
        {
            BrowserUri = new Uri("https://graph.facebook.com/oauth/authorize?client_id=" + Properties.Resources._api_facebook_app_id + "&redirect_uri=http://www.facebook.com/connect/login_success.html&type=user_agent");
            Messenger.Default.Register<Uri>(this, GlobalConstant.BrowserChangedURL, MyCallbackMethod);
        }

        public ICommand CancelAuthCommand
        {
            get
            {
                if (_cancelAuthCommand == null)
                {
                    _cancelAuthCommand = new RelayCommand
                    (
                        param =>
                        {
                            GoBackToAssessmentDetail();
                        },
                        param =>
                        {
                            return true;
                        }
                    );
                }

                return _cancelAuthCommand;
            }
        }

        private void MyCallbackMethod(Uri uri)
        {
            if (uri != null)
            {
                if (uri.ToString().StartsWith("http://www.facebook.com/connect/login_success.html"))
                {
                    string accessToken = uri.Fragment.Split('&')[0].Replace("#access_token=", "");
                    SaveToDB(accessToken);

                    FBAuthenEndGlobalEvent.Instance.Publish(GlobalConstant.MessagFBAuthenCompleted);
                }
                
                
            }
        }

        private void GoBackToAssessmentDetail()
        {
            FBAuthenCancelGlobalEvent.Instance.Publish(GlobalConstant.Cancel);
        }

        private void SaveToDB(string accessToken)
        {
            API tokenSetting = _dbContext.apis.Where(x => x.Name == "FBAccessToken").FirstOrDefault();
            if (tokenSetting != null)
            {
                tokenSetting.Value = accessToken;
                tokenSetting.Created = DateTime.Now;
                _dbContext.SaveChanges();
            }
            else
            {
                API newTokenSetting = new API();
                newTokenSetting.Name = "FBAccessToken";
                newTokenSetting.Value = accessToken;
                newTokenSetting.Created = DateTime.Now;
                _dbContext.apis.Add(newTokenSetting);
                _dbContext.SaveChanges();
            }
        }
    }
}
