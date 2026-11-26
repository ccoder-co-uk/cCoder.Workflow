namespace cCoder.Workflow.Activities.Support;

public static class CertChainValidator
{
    public static bool ValidateCertChain(object _, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return sslPolicyErrors == System.Net.Security.SslPolicyErrors.None;
    }
}