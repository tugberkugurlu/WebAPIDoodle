using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using WebAPIDoodle.Util;
using System.Net.Http.Headers;
using WebAPIDoodle.Http.Entity;

namespace WebAPIDoodle.Http {

    public class OAuthMessageHandler : DelegatingHandler {

        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _token;
        private readonly string _tokenSecret;
        private readonly string _callbackUrl;
        private readonly OAuthBase _oauthBase;

        public OAuthMessageHandler(string consumerKey, string consumerSecret, string token, string tokenSecret, HttpMessageHandler innerHandler)
            : this(new OAuthCredential(consumerKey, consumerSecret, token, tokenSecret), innerHandler) {
        }

        public OAuthMessageHandler(OAuthCredential oAuthCredential, HttpMessageHandler innerHandler)
            : base(innerHandler) {

            if (oAuthCredential == null) {
                throw new NullReferenceException("oAuthCredential");
            }

            _consumerKey = oAuthCredential.ConsumerKey;
            _consumerSecret = oAuthCredential.ConsumerSecret;
            _token = oAuthCredential.Token;
            _tokenSecret = oAuthCredential.TokenSecret;
            _callbackUrl = oAuthCredential.CallbackUrl;

            _oauthBase = new OAuthBase();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            //TODO: Make a better OAuth Utility to compute headers
            string normalizedUri;
            string normalizedParameters;
            string authHeader;

            _oauthBase.GenerateSignature(
                request.RequestUri,
                _consumerKey,
                _consumerSecret,
                _token,
                _tokenSecret,
                request.Method.Method,
                _oauthBase.GenerateTimeStamp(),
                _oauthBase.GenerateNonce(),
                _callbackUrl,
                out normalizedUri,
                out normalizedParameters,
                out authHeader);

            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", authHeader);
            return base.SendAsync(request, cancellationToken);
        }
    }
}