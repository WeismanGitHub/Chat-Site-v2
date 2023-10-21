﻿namespace API.Endpoints.Account.Update;

public static class Data {
    internal static Task Update(Request newData) {
        var update = DB.Update<User>().MatchID(newData.AccountID);

        if (newData.DisplayName != null) {
            update.Modify(u => u.DisplayName, newData.DisplayName);
        }

        if (newData.Email != null) {
            update.Modify(u => u.Email, newData.Email);
        }

        if (newData.Password != null) {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newData.Password);
            update.Modify(u => u.PasswordHash, passwordHash);
        }
        
        return update.ExecuteAsync();
    }
}