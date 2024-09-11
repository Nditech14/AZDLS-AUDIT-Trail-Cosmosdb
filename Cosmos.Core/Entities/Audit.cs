namespace Cosmos.Core.Entities
{
    public class Audit
    {
        public string id { get; set; }              // Unique identifier for the audit entry.
        public string UserId { get; set; }          // The ID of the user performing the action.
        public string UserName { get; set; }        // The name of the user performing the action.
        public string IpAddress { get; set; }        // The role of the user performing the action.
        public string Action { get; set; }          // Description of the action (e.g., "Create", "Update", "Delete")
                                                    // The IP address of the user performing the action.
        public DateTime Timestamp { get; set; }     // The time the action occurred.
        public string ResponseStatusCode { get; set; } // HTTP status code returned after the action.

    }
}
