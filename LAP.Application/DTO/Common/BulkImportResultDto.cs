namespace LAP.Application.DTO.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the result of a bulk import operation.
    /// </summary>
    public class BulkImportResultDto
    {
        /// <summary>
        /// Gets or sets the total number of rows processed.
        /// </summary>
        public int TotalRows { get; set; }

        /// <summary>
        /// Gets or sets the number of rows successfully imported.
        /// </summary>
        public int Imported { get; set; }

        /// <summary>
        /// Gets or sets the number of rows skipped during import.
        /// </summary>
        public int Skipped { get; set; }

        /// <summary>
        /// Gets or sets the collection of errors encountered during import.
        /// </summary>
        public ICollection<Errors> Errors { get; set; } = new List<Errors>();
    }
}
