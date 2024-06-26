using API.Endpoints.Account;

namespace Tests.API.Account.Signin;

public class Tests : TestClass<Fixture> {
	public Tests(Fixture fixture, ITestOutputHelper output) : base(fixture, output) { }

	[Fact]
	public async Task Valid_Input() {
		var res = await Fixture.Client.POSTAsync<Signin, Request>(new() {
			Email = ValidAccount.Email,
			Password = ValidAccount.Password
		});

		res.IsSuccessStatusCode.Should().BeTrue();
		res.Headers.Any(header => header.Key == "Set-Cookie").Should().BeTrue();
	}

	[Fact]
	public async Task Invalid_Email() {
		var res = await Fixture.Client.POSTAsync<Signin, Request>(new() {
			Email = "invalid@email.com",
			Password = ValidAccount.Password
		});

		res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task Invalid_Password() {
		var res = await Fixture.Client.POSTAsync<Signin, Request>(new() {
			Email = ValidAccount.Email,
			Password = "invalidpassword"
		});

		res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}
}
