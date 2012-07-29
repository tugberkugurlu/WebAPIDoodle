using System;

namespace WebAPIDoodle.Http.Entity {

    public class OAuthCredential {

        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        private string _token;
        private string _tokenSecret;
        private string _callbackUrl;

        public OAuthCredential(string consumerKey, string consumerSecret, string callbackUrl) 
            : this(consumerKey, consumerSecret) {

            _callbackUrl = callbackUrl;
        }

        public OAuthCredential(string consumerKey, string consumerSecret, string token, string tokenSecret)
            : this(consumerKey, consumerSecret) {

            _token = token;
            _tokenSecret = tokenSecret;
        }

        public OAuthCredential(string consumerKey, string consumerSecret) {

            if (string.IsNullOrEmpty(consumerKey)) {
                throw new ArgumentNullException("consumerKey");
            }

            if (string.IsNullOrEmpty(consumerSecret)) {
                throw new ArgumentNullException("consumerSecret");
            }

            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }

        public string ConsumerKey { 

            get {
                return _consumerKey;
            } 
        }
        
        public string ConsumerSecret {

            get {
                return _consumerSecret;
            }
        }

        public string Token {

            get {
                return _token;
            }

            set {
                _token = value;
            }
        }

        public string TokenSecret {

            get {
                return _tokenSecret;
            }

            set {
                _tokenSecret = value;
            }
        }

        public string CallbackUrl {

            get {
                return _callbackUrl;
            }

            set {
                _callbackUrl = value;
            }
        }
    }
}