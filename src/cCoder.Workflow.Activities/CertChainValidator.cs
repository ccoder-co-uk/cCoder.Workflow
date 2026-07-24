// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Activities.Support;

public static class CertChainValidator
{
    public static bool ValidateCertChain(object _, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors is System.Net.Security.SslPolicyErrors.None or System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch)
        {
            return true;
        }

        return (sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0 && AnalyseChain(certificate: certificate, chain: chain);
    }

    private static bool AnalyseChain(System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain)
    {
        if (chain?.ChainStatus != null)
        {
            foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
            {
                if (certificate.Subject == certificate.Issuer && status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot)
                {
                    return true;
                }

                if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                {
                    return false;
                }
            }
        }

        return true;
    }
}