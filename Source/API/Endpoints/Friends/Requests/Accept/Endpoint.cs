﻿namespace API.Endpoints.Friends.Requests.Accept;

public class Endpoint : Endpoint<Request> {
    public override void Configure() {
        Post("/{RequestID}/accept");
        Group<RequestGroup>();
        Version(1);

        Description(builder => builder.Accepts<Request>());

        Summary(settings => {
            settings.Summary = "Accept a friend request.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken cancellationToken) {
        var friendRequest = await DB.Find<FriendRequest>().MatchID(req.RequestID).ExecuteSingleAsync(cancellationToken);

        if (friendRequest == null) {
            ThrowError("FriendRequest does not exist.", 404);
        } else if (friendRequest.RecipientID != req.AccountID) {
            ThrowError("You cannot accept this FriendRequest.", 403);
        } else if (friendRequest.Status != Status.Pending) {
			ThrowError("You can only accept pending requests.", 400);
		}

        // These should be whatever the equivalent of Promise.all() is in C#.
        var recipient = await DB.Find<User>().MatchID(friendRequest.RecipientID).ExecuteSingleAsync(cancellationToken);
        var requester = await DB.Find<User>().MatchID(friendRequest.RequesterID).ExecuteSingleAsync(cancellationToken);

        if (requester == null) {
            ThrowError("Could not find requester.", 404);
        } else if (recipient == null) {
            ThrowError("Could not find your account.", 401);
        } else if (recipient.FriendIDs.Contains(requester.ID)) {
            ThrowError("Already friends.", 400);
        }

		if (requester.FriendIDs.Count >= 100) {
			ThrowError("Requester cannot add any more friends.", 400);
		} else if (recipient.FriendIDs.Count >= 100) {
			ThrowError("Recipient cannot add any more friends.", 400);
		}

        friendRequest.Status = Status.Accepted;
        requester.FriendIDs.Add(recipient.ID);
        recipient.FriendIDs.Add(requester.ID);

        var transaction = DB.Transaction();
        await requester.SaveAsync(transaction.Session, cancellationToken);
        await recipient.SaveAsync(transaction.Session, cancellationToken);
        await friendRequest.SaveAsync(transaction.Session, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
