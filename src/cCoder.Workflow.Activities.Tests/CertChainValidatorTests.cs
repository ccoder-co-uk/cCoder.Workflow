// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Security;
using cCoder.Workflow.Activities.Support;
using Xunit;

namespace cCoder.Workflow.Activities.Tests;

public sealed partial class CertChainValidatorTests
{
    [Theory]
    [InlineData(SslPolicyErrors.None, true)]
    [InlineData(SslPolicyErrors.RemoteCertificateNameMismatch, false)]
    [InlineData(SslPolicyErrors.RemoteCertificateNotAvailable, false)]
    [InlineData(SslPolicyErrors.RemoteCertificateChainErrors, false)]
    public void ShouldAcceptOnlyValidTlsPeer(SslPolicyErrors errors, bool expected)
    {
        // Given
        SslPolicyErrors peerErrors = errors;

        // When
        bool actual = CertChainValidator.ValidateCertChain(
            _: null,
            certificate: null,
            chain: null,
            sslPolicyErrors: peerErrors);

        // Then
        Assert.Equal(expected: expected, actual: actual);
    }
}