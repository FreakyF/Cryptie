// using Cryptie.Common.Entities;
// using Cryptie.Common.Entities.Group;
// using Cryptie.Common.Entities.Honeypot;
// using Cryptie.Common.Entities.LoginPolicy;
// using Cryptie.Common.Entities.SessionTokens;
// using Cryptie.Common.Entities.User;
//
// namespace Cryptie.Common.Tests.Entities;
//
// public class EntitiesTests
// {
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void Group_ShouldInitializeAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         const string name = "TestGroup";
//         var group = new Group
//         {
//             Id = id,
//             Name = name
//         };
//
//         Assert.Equal(id, group.Id);
//         Assert.Equal(name, group.Name);
//         Assert.NotNull(group.Users);
//         Assert.Empty(group.Users);
//         Assert.NotNull(group.Messages);
//         Assert.Empty(group.Messages);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void GroupMessage_ShouldSetAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         var refId = Guid.NewGuid();
//         var fromUser = new User
//         {
//             Id = Guid.NewGuid(), Login = "from", DisplayName = "From", Email = "from@x.com",
//             Password = new Password { Secret = "p" }, Totp = new Totp { Secret = [1] }
//         };
//         var toUser = new User
//         {
//             Id = Guid.NewGuid(), Login = "to", DisplayName = "To", Email = "to@x.com",
//             Password = new Password { Secret = "q" }, Totp = new Totp { Secret = [1] }
//         };
//         var messageText = "hello";
//         var dt = DateTime.UtcNow;
//
//         var msg = new GroupMessage
//         {
//             Id = id,
//             ReferenceId = refId,
//             FromUser = fromUser,
//             ToUser = toUser,
//             Message = messageText,
//             DateTime = dt
//         };
//
//         Assert.Equal(id, msg.Id);
//         Assert.Equal(refId, msg.ReferenceId);
//         Assert.Equal(fromUser, msg.FromUser);
//         Assert.Equal(toUser, msg.ToUser);
//         Assert.Equal(messageText, msg.Message);
//         Assert.Equal(dt, msg.DateTime);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void UserAccountHoneypotLock_ShouldSetAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         const string user = "u";
//         var until = DateTime.UtcNow.AddSeconds(1);
//
//         var lockDto = new UserAccountHoneypotLock
//         {
//             Id = id,
//             User = user,
//             Until = until
//         };
//
//         Assert.Equal(id, lockDto.Id);
//         Assert.Equal(user, lockDto.User);
//         Assert.Equal(until, lockDto.Until);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void UserLoginHoneypotAttempt_ShouldSetAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         const string user = "u";
//         var ts = DateTime.UtcNow;
//
//         var attempt = new UserLoginHoneypotAttempt
//         {
//             Id = id,
//             User = user,
//             TimeStamp = ts
//         };
//
//         Assert.Equal(id, attempt.Id);
//         Assert.Equal(user, attempt.User);
//         Assert.Equal(ts, attempt.TimeStamp);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void UserAccountLock_ShouldSetAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         var user = new User
//         {
//             Id = Guid.NewGuid(), Login = "u", DisplayName = "U", Email = "u@x.com",
//             Password = new Password { Secret = "p" }, Totp = new Totp { Secret = [1] }
//         };
//         var until = DateTime.UtcNow.AddMinutes(15);
//
//         var acctLock = new UserAccountLock
//         {
//             Id = id,
//             User = user,
//             Until = until
//         };
//
//         Assert.Equal(id, acctLock.Id);
//         Assert.Equal(user, acctLock.User);
//         Assert.Equal(until, acctLock.Until);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void UserLoginAttempt_ShouldSetAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         var user = new User
//         {
//             Id = Guid.NewGuid(), Login = "u", DisplayName = "U", Email = "u@x.com",
//             Password = new Password { Secret = "p" }, Totp = new Totp { Secret = [1] }
//         };
//         var ts = DateTime.UtcNow;
//
//         var attempt = new UserLoginAttempt
//         {
//             Id = id,
//             User = user,
//             TimeStamp = ts
//         };
//
//         Assert.Equal(id, attempt.Id);
//         Assert.Equal(user, attempt.User);
//         Assert.Equal(ts, attempt.TimeStamp);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void TotpToken_ShouldSetAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         var user = new User
//         {
//             Id = Guid.NewGuid(), Login = "u", DisplayName = "U", Email = "u@x.com",
//             Password = new Password { Secret = "p" }, Totp = new Totp { Secret = [1] }
//         };
//         var until = DateTime.UtcNow.AddHours(1);
//
//         var token = new TotpToken
//         {
//             Id = id,
//             User = user,
//             Until = until
//         };
//
//         Assert.Equal(id, token.Id);
//         Assert.Equal(user, token.User);
//         Assert.Equal(until, token.Until);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void UserToken1_ShouldSetAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         var user = new User
//         {
//             Id = Guid.NewGuid(), Login = "u", DisplayName = "u", Email = "u@x.com",
//             Password = new Password { Secret = "p" }, Totp = new Totp { Secret = [1] }
//         };
//
//         var token = new UserToken
//         {
//             Id = id,
//             User = user
//         };
//
//         Assert.Equal(id, token.Id);
//         Assert.Equal(user, token.User);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void UserToken2_ShouldSetAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         User? user = null;
//
//         var token = new UserToken
//         {
//             Id = id,
//             User = user
//         };
//
//         Assert.Equal(id, token.Id);
//         Assert.Equal(user, token.User);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void Password_ShouldSetAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         const string secret = "p";
//         var password = new Password
//         {
//             Id = id,
//             Secret = secret
//         };
//
//         Assert.Equal(id, password.Id);
//         Assert.Equal(secret, password.Secret);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void Totp_ShouldSetAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         byte[] secret = [1];
//
//         var t = new Totp
//         {
//             Id = id,
//             Secret = secret
//         };
//
//         Assert.Equal(id, t.Id);
//         Assert.Equal(secret, t.Secret);
//     }
//
//     [Trait("TestCategory", "Unit")]
//     [Fact]
//     public void User_ShouldInitializeAndReturnCorrectValues()
//     {
//         var id = Guid.NewGuid();
//         const string login = "u";
//         const string displayName = "U";
//         const string email = "u@x.com";
//         var pwd = new Password { Secret = "p" };
//         var totp = new Totp { Secret = [1] };
//         var key = "k";
//
//         var user = new User
//         {
//             Id = id,
//             Login = login,
//             DisplayName = displayName,
//             Email = email,
//             Password = pwd,
//             Totp = totp,
//             PrivateKey = key
//         };
//
//         Assert.Equal(id, user.Id);
//         Assert.Equal(login, user.Login);
//         Assert.Equal(displayName, user.DisplayName);
//         Assert.Equal(email, user.Email);
//         Assert.Equal(pwd, user.Password);
//         Assert.Equal(totp, user.Totp);
//         Assert.Equal(key, user.PrivateKey);
//
//         Assert.NotNull(user.Groups);
//         Assert.Empty(user.Groups);
//         Assert.NotNull(user.Friends);
//         Assert.Empty(user.Friends);
//     }
// }