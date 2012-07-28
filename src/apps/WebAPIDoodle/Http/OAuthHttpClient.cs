using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using WebAPIDoodle.Http.Entity;

namespace WebAPIDoodle.Http {

    public class OAuthHttpClient : HttpClient {

        private readonly OAuthCredential _oAuthCredential;

        public OAuthHttpClient(string consumerKey, string consumerSecret, string token, string tokenSecret) 
            : this(new OAuthCredential(consumerKey, consumerSecret, token, tokenSecret)) {
        }

        public OAuthHttpClient(OAuthCredential oAuthCredential) 
            : base(new OAuthMessageHandler(oAuthCredential, new HttpClientHandler())) {

            if (oAuthCredential == null) {
                throw new NullReferenceException("oAuthCredential");
            }

            _oAuthCredential = oAuthCredential;
        }

        public OAuthCredential OAuthCredential { 

            get {
                return _oAuthCredential;
            } 
        }
    }
}