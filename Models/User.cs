// ============================================================
// User.cs — Data Model for Users
// ============================================================
// TUTORIAL STEP: This class represents a user account in the system.
// It's used by the WCF service when returning user information
// (e.g., after a successful login).
//
// PLACE THIS FILE IN: OrbitechWeb/Models/User.cs
// ============================================================

using System.Runtime.Serialization;

namespace OrbitechWeb.Models
{
    [DataContract]
    public class User
    {
        [DataMember]
        public int UserID { get; set; }          // Maps to u_ID

        [DataMember]
        public string Username { get; set; }     // Maps to u_Username

        [DataMember]
        public string Role { get; set; }         // Maps to u_Role ('admin' or 'customer')

        [DataMember]
        public string Email { get; set; }         // Maps to u_Email

        // NOTE: We deliberately do NOT include the password hash here.
        // Passwords should never be sent back to the frontend, even in hashed form.
    }
}
