using OnyxCore.CertStore;
using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace OnyxCore.Server
{
    public class TlsManager : IDisposable
    {
        public void Dispose()
        {

        }

        public bool Authenticate()
        {
            var response = true;

            try
            {
                using (var log = new Log())
                {
                    if (!new Authentication().VerifyCert())
                    {
                        log.Error("TLS authentication failed...");
                        response = false;
                    }
                }
            }
            catch (Exception ex)
            {
                response = false;
            }

            return response;
        }

        public static void ServerSideHandshake(SslStream _sslStream)
        {
            X509Certificate2 certificate = GetServerCertificate("ssl_server");

            bool requireClientCertificate = true;
            SslProtocols enabledSslProtocols = SslProtocols.Tls; // SslProtocols.Ssl3 | 
            bool checkCertificateRevocation = true;


            _sslStream.AuthenticateAsServerAsync(certificate, requireClientCertificate, enabledSslProtocols, checkCertificateRevocation);
        }

        private static X509Certificate2 GetServerCertificate(string name)
        {
            return new X509Certificate2();
        }
    }
}
