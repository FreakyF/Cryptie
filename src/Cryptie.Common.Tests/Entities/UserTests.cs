using System;
using System.Collections.Generic;
using Cryptie.Common.Entities;
using Xunit;

namespace Cryptie.Common.Tests.Entities
{
    public class UserTests
    {
        [Fact]
        public void User_DefaultConstructor_InitializesCollectionsAndStrings()
        {
            var user = new User();
            Assert.Equal(Guid.Empty, user.Id);
            Assert.Equal(string.Empty, user.Login);
            Assert.Equal(string.Empty, user.DisplayName);
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.PrivateKey);
            Assert.Equal(string.Empty, user.PublicKey);
            Assert.Equal(Guid.Empty, user.PasswordId);
            Assert.Equal(Guid.Empty, user.TotpId);
            Assert.NotNull(user.Friends);
            Assert.NotNull(user.FriendsOf);
            Assert.NotNull(user.Groups);
            Assert.NotNull(user.Tokens);
            Assert.NotNull(user.TotpTokens);
            Assert.NotNull(user.AccountLocks);
            Assert.NotNull(user.LoginAttempts);
            Assert.NotNull(user.GroupEncryptionKeys);
        }

        [Fact]
        public void User_SetAndGetProperties_WorksCorrectly()
        {
            var id = Guid.NewGuid();
            var login = "testuser";
            var displayName = "Test User";
            var email = "test@example.com";
            var privateKey = "privateKeyData";
            var publicKey = "publicKeyData";
            var passwordId = Guid.NewGuid();
            var totpId = Guid.NewGuid();
            var password = new Password();
            var totp = new Totp();
            var friends = new List<User>();
            var friendsOf = new List<User>();
            var groups = new List<Group>();
            var tokens = new List<UserToken>();
            var totpTokens = new List<TotpToken>();
            var accountLocks = new List<UserAccountLock>();
            var loginAttempts = new List<UserLoginAttempt>();
            var groupEncryptionKeys = new List<GroupEncryptionKey>();

            var user = new User
            {
                Id = id,
                Login = login,
                DisplayName = displayName,
                Email = email,
                PrivateKey = privateKey,
                PublicKey = publicKey,
                PasswordId = passwordId,
                TotpId = totpId,
                Password = password,
                Totp = totp,
                Friends = friends,
                FriendsOf = friendsOf,
                Groups = groups,
                Tokens = tokens,
                TotpTokens = totpTokens,
                AccountLocks = accountLocks,
                LoginAttempts = loginAttempts,
                GroupEncryptionKeys = groupEncryptionKeys
            };

            Assert.Equal(id, user.Id);
            Assert.Equal(login, user.Login);
            Assert.Equal(displayName, user.DisplayName);
            Assert.Equal(email, user.Email);
            Assert.Equal(privateKey, user.PrivateKey);
            Assert.Equal(publicKey, user.PublicKey);
            Assert.Equal(passwordId, user.PasswordId);
            Assert.Equal(totpId, user.TotpId);
            Assert.Equal(password, user.Password);
            Assert.Equal(totp, user.Totp);
            Assert.Equal(friends, user.Friends);
            Assert.Equal(friendsOf, user.FriendsOf);
            Assert.Equal(groups, user.Groups);
            Assert.Equal(tokens, user.Tokens);
            Assert.Equal(totpTokens, user.TotpTokens);
            Assert.Equal(accountLocks, user.AccountLocks);
            Assert.Equal(loginAttempts, user.LoginAttempts);
            Assert.Equal(groupEncryptionKeys, user.GroupEncryptionKeys);
        }
    }
}

