using System;

namespace Actionstep.API.WebClient.Domain_Models
{
    public class AppState
    {
        private bool _isSignedIn { get; set; }

        public bool IsSignedIn { get { return _isSignedIn; } }

        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string ApiEndpoint { get; set; }
        public string Orgkey { get; set; }
        public string RefreshToken { get; set; }

        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public event Action OnChange;

        public void SetSignInState(bool isSignedIn)
        {
            _isSignedIn = isSignedIn;
            OnChange?.Invoke();
        }


        public event Action<FilenoteResthookResponseData> OnFilenoteResthookReceived;

        public void FilenoteResthookReceived(FilenoteResthookResponseData responseData)
        {
            OnFilenoteResthookReceived?.Invoke(responseData);
        }
    }
}