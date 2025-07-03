using Cryptie.Client.Features.Authentication.Models;

namespace Cryptie.Client.Tests.Features.Authentication.Models;

public class RegisterModelTests
{
    [Fact]
    public void RegisterModel_Property_Setters_Work_Correctly()
    {
        // Arrange
        var model = new RegisterModel();
        var changedProperties = new List<string>();
        model.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName!);

        // Act
        model.Username = "testuser";
        model.DisplayName = "Test User";
        model.Email = "test@example.com";
        model.Password = "strongpassword";

        // Assert
        Assert.Equal("testuser", model.Username);
        Assert.Equal("Test User", model.DisplayName);
        Assert.Equal("test@example.com", model.Email);
        Assert.Equal("strongpassword", model.Password);

        Assert.Contains("Username", changedProperties);
        Assert.Contains("DisplayName", changedProperties);
        Assert.Contains("Email", changedProperties);
        Assert.Contains("Password", changedProperties);
    }

    [Fact]
    public void RegisterModel_DoesNotRaiseEvent_WhenValueUnchanged()
    {
        // Arrange
        var model = new RegisterModel
        {
            Username = "sameuser",
            DisplayName = "Same Name",
            Email = "same@example.com",
            Password = "samepass"
        };

        var raised = false;
        model.PropertyChanged += (_, _) => raised = true;

        // Act — assign same values again
        model.Username = "sameuser";
        model.DisplayName = "Same Name";
        model.Email = "same@example.com";
        model.Password = "samepass";

        // Assert
        Assert.False(raised);
    }
}