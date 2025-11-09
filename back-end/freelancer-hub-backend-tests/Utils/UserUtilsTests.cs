using System;
using System.Collections.Generic;
using System.Security.Claims;
using freelancer_hub_backend.Utils;
using Xunit;

namespace freelancer_hub_backend.Tests.Utils
{
    public class UserUtilsTests
    {
        private readonly UserUtils _userUtils;

        public UserUtilsTests()
        {
            _userUtils = new UserUtils();
        }

        [Fact]
        public void GetSupabaseUserId_DeveRetornarId_QuandoClaimExiste()
        {
            // Arrange
            var userIdEsperado = "abc123";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userIdEsperado)
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            // Act
            var resultado = _userUtils.GetSupabaseUserId(user);

            // Assert
            Assert.Equal(userIdEsperado, resultado);
        }

        [Fact]
        public void GetSupabaseUserId_DeveLancarExcecao_QuandoClaimNaoExiste()
        {
            // Arrange
            var claims = new List<Claim>(); // sem ClaimTypes.NameIdentifier
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => _userUtils.GetSupabaseUserId(user));
            Assert.Equal("Usuário sem ID (nameidentifier) no token.", ex.Message);
        }

        [Fact]
        public void GetSupabaseUserId_DeveLancarExcecao_QuandoClaimVazia()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, string.Empty)
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => _userUtils.GetSupabaseUserId(user));
            Assert.Equal("Usuário sem ID (nameidentifier) no token.", ex.Message);
        }
    }
}
