using System.ComponentModel;
using Cryptie.Client.Features.Authentication.Models;
using Xunit;

namespace Cryptie.Client.Tests.Features.Authentication.Models;

public class LoginModelTests
{
    [Fact]
    public void LoginModel_Property_Setters_Work_Correctly()
    {
        // Arrange
        var model = new LoginModel();
        var changedProperties = new List<string>();
        model.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName!);

        // Act
        model.Username = "testuser";
        model.Password = "securepass";

        // Assert
        Assert.Equal("testuser", model.Username);
        Assert.Equal("securepass", model.Password);
        Assert.Contains("Username", changedProperties);
        Assert.Contains("Password", changedProperties);
    }

    [Fact]
    public void LoginModel_DoesNotRaiseEvent_WhenValueIsUnchanged()
    {
        // Arrange
        var model = new LoginModel();
        model.Username = "initial";
        model.Password = "123";

        var raised = false;
        model.PropertyChanged += (_, e) => raised = true;

        // Act
        model.Username = "initial";  // No change
        model.Password = "123";      // No change

        // Assert
        Assert.False(raised);
    }
}