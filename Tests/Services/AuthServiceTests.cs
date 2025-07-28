using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using TimeRecorderBACKEND.Services;
using TimeRecorderBACKEND.Dtos;

public class AuthServiceTests
{
    [Fact]
    public void SetAuthCookie_SetsCookie()
    {
        AuthService service = new AuthService();
        Mock<HttpResponse> responseMock = new Mock<HttpResponse>();
        Mock<IResponseCookies> cookiesMock = new Mock<IResponseCookies>();
        responseMock.SetupGet(r => r.Cookies).Returns(cookiesMock.Object);

        service.SetAuthCookie(responseMock.Object, "token");

        cookiesMock.Verify(c => c.Append("access_token", "token", It.IsAny<CookieOptions>()), Times.Once);
    }

    [Fact]
    public void RemoveAuthCookie_RemovesCookie()
    {
        AuthService service = new AuthService();
        Mock<HttpResponse> responseMock = new Mock<HttpResponse>();
        Mock<IResponseCookies> cookiesMock = new Mock<IResponseCookies>();
        responseMock.SetupGet(r => r.Cookies).Returns(cookiesMock.Object);

        service.RemoveAuthCookie(responseMock.Object);

        cookiesMock.Verify(c => c.Append("access_token", "", It.IsAny<CookieOptions>()), Times.Once);
    }

    [Fact]
    public void GetUserInfo_ReturnsUserInfo()
    {
        AuthService service = new AuthService();
        List<Claim> claims = new List<Claim>
        {
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "123"),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "test@example.com"),
            new Claim(ClaimTypes.GivenName, "Jan"),
            new Claim(ClaimTypes.Surname, "Kowalski"),
            new Claim(ClaimTypes.Role, "Admin")
        };
        ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuth");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

        UserInfoDto result = service.GetUserInfo(principal);

        Assert.NotNull(result);
        Assert.Contains("Admin", ((IEnumerable<string>)((dynamic)result).Roles));
        Assert.Equal("Jan", ((dynamic)result).Name);
        Assert.Equal("Kowalski", ((dynamic)result).Surname);
        Assert.Equal("test@example.com", ((dynamic)result).Email);
        Assert.Equal("123", ((dynamic)result).Id);
        Assert.True(((dynamic)result).IsAuthenticated);
    }

    [Fact]
    public void GetUserInfo_ReturnsAnonymous_WhenNoClaims()
    {
        AuthService service = new AuthService();
        ClaimsIdentity identity = new ClaimsIdentity();
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

        UserInfoDto result = service.GetUserInfo(principal);

        Assert.NotNull(result);
        Assert.False(((dynamic)result).IsAuthenticated);
    }
}