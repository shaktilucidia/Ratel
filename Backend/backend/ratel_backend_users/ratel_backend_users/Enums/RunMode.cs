namespace ratel_backend_users.Enums
{
    /// <summary>
    /// Service run mode
    /// </summary>
    public enum RunMode
    {
        /// <summary>
        /// Normal run
        /// </summary>
        Normal,

        /// <summary>
        /// Apply migrations to DB
        /// </summary>
        ApplyMigrations,

        /// <summary>
        /// Init users/roles
        /// </summary>
        InitUsers,

        /// <summary>
        /// Update users/roles
        /// </summary>
        UpdateUsers
    }
}
