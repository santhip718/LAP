namespace LAP.Application.DTO.Paginated
{
    using System.Collections.Generic;
    using LAP.Application.DTO.User;

    /// <summary>
    /// Represents a paginated response containing a collection of user details.
    /// </summary>
    public class PaginatedUsersDto
    {
        /// <summary>
        /// Gets or sets the collection of user details for the current page.
        /// </summary>
        public ICollection<UserDetailDto> Data { get; set; }

        /// <summary>
        /// Gets or sets the total number of users across all pages.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PageSize { get; set; }
    }
}
