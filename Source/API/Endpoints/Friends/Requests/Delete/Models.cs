﻿namespace API.Endpoints.Friends.Requests.Delete;

public class Request {
        [From(Claim.AccountID, IsRequired = true)]
        public string AccountID { get; set; }
        public string RequestID { get; set; }
}

internal sealed class Validator : Validator<Request> {
    public Validator() {
        RuleFor(req => req.RequestID)
            .NotEmpty()
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("Invalid RequestID.");
    }
}
