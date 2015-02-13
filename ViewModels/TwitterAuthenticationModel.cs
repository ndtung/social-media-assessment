using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using MCIFramework.Models;
using TweetSharp;
using System.Windows;

namespace MCIFramework.ViewModels
{
    public class TwitterAuthenticationModel : ViewModelBase, IPageViewModel
    {
        private Database _dbContext = new Database();
        private Uri _browserUri;
        private Visibility _isPINVisible = Visibility.Visible;
        private String _pin;
        private ICommand _cancelAuthCommand;
        private ICommand _submitPinCommand;
        OAuthHelper _oau = new OAuthHelper();
        private string _requestToken;
        
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
        
        public String Name
        {
            get { return "FBAuthentication"; }
        }

        public Visibility IsPINVisible
        {
            get { return _isPINVisible; }
            set
            {
                if (_isPINVisible != value)
                {
                    _isPINVisible = value;
                    OnPropertyChanged("IsPINVisible");
                }
            }
        }

        public String Pin
        {
            get { return _pin; }
            set
            {
                if (_pin != value)
                {
                    _pin = value;
                    OnPropertyChanged("Pin");
                }
            }
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

        public ICommand SubmitPINCommand
        {
            get
            {
                if (_submitPinCommand == null)
                {
                    _submitPinCommand = new RelayCommand
                    (
                        param =>
                        {
                            PinSubmit();
                        },
                        param =>
                        {
                            return true;
                        }
                    );
                }

                return _submitPinCommand;
            }
        }
        
        public TwitterAuthenticationModel()
        {
            _requestToken = _oau.GetRequestToken(Properties.Resources._api_twitter_consumer_key, Properties.Resources._api_twitter_consumer_secret);
            BrowserUri = new Uri(_oau.GetAuthorizeUrl(_requestToken));
            Messenger.Default.Register<Uri>(this, "TwitterBrowserChangedURL", MyCallbackMethod);
        }
        private void MyCallbackMethod(Uri uri)
        {
            if (uri != null)
            {
                if (uri.ToString().StartsWith("https://api.twitter.com/oauth/authorize"))
                {
                }
            }
        }

        private void GoBackToAssessmentDetail()
        {
            TwitterAuthenCancelGlobalEvent.Instance.Publish("Cancel");
        }

        private void PinSubmit()
        {
            _oau.GetUserTwAccessToken(_requestToken,Pin,Properties.Resources._api_twitter_consumer_key, Properties.Resources._api_twitter_consumer_secret);
            if (_oau.oauth_error == null)
            {
                SaveToDB(_oau);
                TwitterAuthenEndGlobalEvent.Instance.Publish("Twitter Authentication completed");
            }

            else
                TwitterAuthenEndGlobalEvent.Instance.Publish("Twitter Authentication failed");
        }

        private void SaveToDB(OAuthHelper accessToken)
        {
            API token = _dbContext.apis.Where(x => x.Name == "TwitterToken").FirstOrDefault();
            API tokenSecret = _dbContext.apis.Where(x => x.Name == "TwitterTokenSecret").FirstOrDefault();
            if (token != null)
            {
                token.Value = accessToken.oauth_access_token;
                token.Created = DateTime.Now;
                _dbContext.SaveChanges();
            }
            else
            {
                API newToken = new API();
                newToken.Name = "TwitterToken";
                newToken.Value = accessToken.oauth_access_token;
                newToken.Created = DateTime.Now;
                _dbContext.apis.Add(newToken);
                _dbContext.SaveChanges();


            }
            if (tokenSecret !=null)
            {
                tokenSecret.Value = accessToken.oauth_access_token_secret;
                tokenSecret.Created = DateTime.Now;
                _dbContext.SaveChanges();
            }
            else
            {
                API newTokenSecret = new API();
                newTokenSecret.Name = "TwitterTokenSecret";
                newTokenSecret.Value = accessToken.oauth_access_token_secret;
                newTokenSecret.Created = DateTime.Now;
                _dbContext.apis.Add(newTokenSecret);
                _dbContext.SaveChanges();

            }
        }
    }
}
