// UserSession.cs
// This file defines a static class to hold the current user's session data.
// It acts like a global store for user information once they log in.

namespace BookingSystem // IMPORTANT: Ensure this matches your project's namespace
{
    public static class UserSession
    {
        // Properties to store user data.
        // 'public static' makes them accessible from anywhere in your application.
        // '{ get; set; }' makes them readable (get) and writable (set).

        public static int UserId { get; set; }        // The unique ID of the user from the database
        public static string FullName { get; set; }   // The user's full name
        public static string Email { get; set; }      // The user's email address
        public static string Role { get; set; }       // The user's role (e.g., "user", "admin")

        // A method to clear the session information when the user logs out.
        public static void ClearSession()
        {
            UserId = 0;
            FullName = null;
            Email = null;
            Role = null;
        }
    }
}