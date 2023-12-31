type APIErrorRes<APIError> = {
    errors: APIError & { generalErrors?: string };
    message: string;
    statusCode: number;
};

type Friend = {
    id: string;
    displayName: string;
    createdAt: string;
};

type ConversationsData = {
    id: string;
    name: string;
    createdAt: string;
}[];

type AccountData = {
    id: string;
    displayName: string;
    email: string;
    totalConversations: number;
    totalFriends: number;
    createdAt: string;
};

type SingleConvoData = {
    id: string;
    name: string;
    createdAt: string;
    members: { id: string; name: string }[];
};

type FriendReqStatus = 'Accepted' | 'Declined' | 'Pending';

type FriendRequest = {
    id: string;
    requesterID: string;
    recipientID: string;
    message?: string;
    status: number;
    createdAt: string;
};
