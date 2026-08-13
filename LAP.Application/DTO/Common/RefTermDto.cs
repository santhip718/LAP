namespace LAP.Application.DTO.Common
{
    using System;

    /// <summary>
    /// Represents a reference term with an identifier and name.
    /// </summary>
    public class RefTermDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the reference term.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the reference term.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
