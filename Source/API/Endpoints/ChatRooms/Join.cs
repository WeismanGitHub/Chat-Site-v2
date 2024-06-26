﻿using Microsoft.AspNetCore.SignalR;

namespace API.Endpoints.ChatRooms.Join;

public sealed class Request {
	[From(Claim.AccountID, IsRequired = true)]
	public string AccountID { get; set; }
	public required string ID { get; set; }
}

internal sealed class Validator : Validator<Request> {
	public Validator() {
		RuleFor(req => req.ID)
			.NotEmpty();
	}
}

public sealed class Endpoint : Endpoint<Request> {
	public override void Configure() {
		Post("/Join");
		Group<ChatRoomGroup>();
		Version(1);

		Description(builder => builder.Accepts<Request>());
	}

	public override async Task HandleAsync(Request req, CancellationToken cancellationToken) {
		var chat = await DB.Find<ChatRoom>()
			.MatchID(req.ID)
			.ExecuteSingleAsync(cancellationToken);

		if (chat == null) {
			ThrowError("Invalid ID", 400);
		} else if (chat.MemberIDs.Count >= 100) {
			ThrowError("Chat room is at capacity.", 400);
		} else if (chat.MemberIDs.Contains(req.AccountID)) {
			ThrowError("You are already a member of this chat room.", 400);
		}

		var transaction = new Transaction();

		chat.MemberIDs.Add(req.AccountID);
		await chat.SaveAsync(transaction.Session, cancellationToken);

		var user = await transaction.UpdateAndGet<User>()
			.MatchID(req.AccountID)
			.Modify(user => user.Push(u => u.ChatRoomIDs, chat.ID))
			.ExecuteAsync(cancellationToken);

		await transaction.CommitAsync(cancellationToken);

		var hub = HttpContext.RequestServices.GetRequiredService<IHubContext<ChatHub>>();

		if (hub != null) {
			await hub.Clients.Group(chat.ID).SendAsync("UserJoined", new Member() { Id = req.AccountID, Name = user.Name }, cancellationToken: cancellationToken);
		}
	}
}
