﻿namespace API.Endpoints.Conversations.Get;

public sealed class Request {
    [From(Claim.AccountID, IsRequired = true)]
    public string AccountID { get; set; }
}