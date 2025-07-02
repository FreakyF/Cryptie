// using System;
// using Cryptie.Common.Features.GroupManagement.DTOs;
// using Xunit;
//
// namespace Cryptie.Common.Tests.Features.GroupManagement.DTOs;
//
// public class GetGroupsNamesRequestDtoTests
// {
//     [Fact]
//     public void Can_Create_GetGroupsNamesRequestDto_With_Valid_SessionToken()
//     {
//         // Arrange
//         var token = Guid.NewGuid();
//         // Act
//         var dto = new GetGroupsNamesRequestDto { SessionToken = token };
//         // Assert
//         Assert.Equal(token, dto.SessionToken);
//     }
//
//     [Fact]
//     public void SessionToken_Default_Should_Be_EmptyGuid()
//     {
//         // Arrange
//         var dto = new GetGroupsNamesRequestDto();
//         // Assert
//         Assert.Equal(Guid.Empty, dto.SessionToken);
//     }
//
//     [Fact]
//     public void Can_Set_And_Get_SessionToken()
//     {
//         // Arrange
//         var dto = new GetGroupsNamesRequestDto();
//         var token = Guid.NewGuid();
//         // Act
//         dto.SessionToken = token;
//         // Assert
//         Assert.Equal(token, dto.SessionToken);
//     }
// }
