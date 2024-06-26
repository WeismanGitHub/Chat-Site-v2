﻿using SigninAPI = API.Endpoints.Account.Signin;
using API.Database.Entities;
using MongoDB.Entities;
using MongoDB.Bson;

namespace Tests.API.Conversations.SingleConvo.Leave;

public class Fixture : TestFixture<Program> {
	public Fixture(IMessageSink sink) : base(sink) { }
	public readonly string AccountID = ObjectId.GenerateNewId().ToString();
	public readonly string ConvoID = ObjectId.GenerateNewId().ToString();
	public string FullConvoID = ObjectId.GenerateNewId().ToString();

	protected override async Task SetupAsync() {
		await DB.InsertAsync(new User() {
			ID = AccountID,
			Name = ValidAccount.DisplayName,
			Email = ValidAccount.Email,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(ValidAccount.Password),
		});

		await DB.InsertAsync(new List<ChatRoom>() {
			new ChatRoom() {
				ID = ConvoID,
				Name = "Convo 1",
				MemberIDs = new() { AccountID }
			}
		});

		await DB.InsertAsync(new List<ChatRoom>() {
			new ChatRoom() {
				ID = FullConvoID,
				Name = "Convo 2",
				MemberIDs = new() {
					AccountID,
					ObjectId.GenerateNewId().ToString()
				}
			}
		});

		await Client.POSTAsync<API.Endpoints.Account.Signin, API.Endpoints.Account.Request>(new SigninAPI.Request() {
			Email = ValidAccount.Email,
			Password = ValidAccount.Password,
		});
	}

	protected override async Task TearDownAsync() {
		await DB.DeleteAsync<User>(AccountID);
		await DB.DeleteAsync<ChatRoom>(ConvoID);
		await DB.DeleteAsync<ChatRoom>(FullConvoID);
	}
}
