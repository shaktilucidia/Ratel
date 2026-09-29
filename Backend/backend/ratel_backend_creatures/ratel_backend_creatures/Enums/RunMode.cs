namespace ratel_backend_creatures.Enums
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
        /// Init creatures/roles
        /// </summary>
        InitCreatures,

        /// <summary>
        /// Update creatures/roles
        /// </summary>
        UpdateCreatures
    }
}
