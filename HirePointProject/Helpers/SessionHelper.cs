
using Microsoft.AspNetCore.Http;

namespace HirePoint.Helpers
{
    public static class SessionHelper
    {
        // =========================
        // SET USER SESSION
        // =========================

        public static void SetUserSession(
            ISession session,
            int userID,
            string userType,
            string userName)
        {
            session.SetInt32(
                "UserID",
                userID
            );

            session.SetString(
                "UserType",
                userType?.Trim() ?? ""
            );

            session.SetString(
                "UserName",
                userName?.Trim() ?? ""
            );
        }


        // =========================
        // GET USER ID
        // =========================

        public static int? GetUserID(ISession session)
        {
            return session.GetInt32("UserID");
        }


        // =========================
        // GET USER TYPE
        // =========================

        public static string? GetUserType(ISession session)
        {
            string? userType =
                session.GetString("UserType");

            return userType?.Trim();
        }


        // =========================
        // GET USER NAME
        // =========================

        public static string? GetUserName(ISession session)
        {
            string? userName =
                session.GetString("UserName");

            return userName?.Trim();
        }


        // =========================
        // CLEAR SESSION
        // =========================

        public static void ClearSession(ISession session)
        {
            session.Clear();
        }
    }
}
