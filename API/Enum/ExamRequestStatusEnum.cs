namespace API.Enum
{
    /// <summary>
    /// Defines the possible statuses for an exam request
    /// </summary>
    public static class ExamRequestStatusEnum
    {
        /// <summary>
        /// Initial status when exam request is created
        /// </summary>
        public const string Pending = "Pending";

        /// <summary>
        /// Status when exam request is approved
        /// </summary>
        public const string Approved = "Approved";

        /// <summary>
        /// Status when exam request is rejected
        /// </summary>
        public const string Rejected = "Rejected";

        /// <summary>
        /// Status when exam request is cancelled
        /// </summary>
        public const string Cancelled = "Cancelled";
    }
} 