﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Jose;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace MobileConnect.Helpers
{
    public static class JwtHelper
    {
        public static string ToJwtTokenWithRs256<T>(this T payloadObject, string privateRsaKey)
        {
            var json = JsonConvert.SerializeObject(payloadObject);
            var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            return payload.ToJwtTokenWithRs256(privateRsaKey);
        }

        public static string ToJwtTokenWithRs256(this Dictionary<string, object> payload, string privateRsaKey)
        {
            RSAParameters rsaParams;

            // Bouncy Castle (to create the signing key):
            using (var stringReader = new StringReader(privateRsaKey))
            {
                var pemReader = new PemReader(stringReader);

                var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;
                if (keyPair == null) throw new Exception("Could not read RSA private key");

                var privateRsaParams = keyPair.Private as RsaPrivateCrtKeyParameters;
                if (privateRsaParams == null) throw new Exception("Could not read RSA private key");

                rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
            }

            // Jose JWT (to encode the token):
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);

                return JWT.Encode(payload, rsa, JwsAlgorithm.RS256);
            }
        }

        public static List<Claim> GetJwtTokenClaims(this string jwtInput)
        {
            if (string.IsNullOrEmpty(jwtInput))
                return new List<Claim>();

            var jwtHandler = new JwtSecurityTokenHandler();

            if (!jwtHandler.CanReadToken(jwtInput)) 
                return new List<Claim>();

            var token = jwtHandler.ReadJwtToken(jwtInput);

            return token.Claims.ToList();
        }
    }
}