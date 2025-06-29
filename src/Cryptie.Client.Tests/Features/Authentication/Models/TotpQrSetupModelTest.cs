using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cryptie.Client.Features.Authentication.Models;
using Xunit;

namespace Cryptie.Client.Tests.Features.Authentication.Models;

public class TotpQrSetupModelTests
{
    [Fact]
    public void TotpQrSetupModel_Properties_SetCorrectly_And_RaisePropertyChanged()
    {
        // Arrange
        var model = new TotpQrSetupModel();
        var changedProperties = new List<string>();
        model.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName!);

        var expectedSecret = "MYSECRET123";
        var expectedToken = Guid.NewGuid();

        // Act
        model.Secret = expectedSecret;
        model.TotpToken = expectedToken;

        // Assert
        Assert.Equal(expectedSecret, model.Secret);
        Assert.Equal(expectedToken, model.TotpToken);

        Assert.Contains(nameof(model.Secret), changedProperties);
        Assert.Contains(nameof(model.TotpToken), changedProperties);
    }

    [Fact]
    public void TotpQrSetupModel_DoesNotRaisePropertyChanged_WhenValueUnchanged()
    {
        // Arrange
        var token = Guid.NewGuid();
        var model = new TotpQrSetupModel
        {
            Secret = "unchanged",
            TotpToken = token
        };

        var raised = false;
        model.PropertyChanged += (_, _) => raised = true;

        // Act
        model.Secret = "unchanged";
        model.TotpToken = token;

        // Assert
        Assert.False(raised);
    }
}