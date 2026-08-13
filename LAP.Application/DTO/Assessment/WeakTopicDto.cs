namespace LAP.Application.DTO.Assessment
{
    using System;

    /// <summary>
    /// Represents a weak topic identified from assessment performance.
    /// </summary>
    public class WeakTopicDto
    {
        /// <summary>
        /// Gets or sets the meta-topic identifier.
        /// </summary>
        public Guid MetaTopicId { get; set; }

        /// <summary>
        /// Gets or sets the name of the topic.
        /// </summary>
        public string? TopicName { get; set; }

        /// <summary>
        /// Gets or sets the average score achieved for this topic.
        /// </summary>
        public double AverageScore { get; set; }

        /// <summary>
        /// Gets or sets the number of failed attempts for this topic.
        /// </summary>
        public int FailedAttempts { get; set; }
    }
}
