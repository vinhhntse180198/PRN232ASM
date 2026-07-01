namespace PRN232ASM.Shared;

public static class SupabaseConnectionHelper
{
    public static string? Build(IConfiguration configuration, string schema)
    {
        var host = Environment.GetEnvironmentVariable("SUPABASE_DB_HOST")
            ?? configuration["SUPABASE_DB_HOST"];
        if (string.IsNullOrWhiteSpace(host))
            return null;

        var port = Environment.GetEnvironmentVariable("SUPABASE_DB_PORT")
            ?? configuration["SUPABASE_DB_PORT"] ?? "5432";
        var database = Environment.GetEnvironmentVariable("SUPABASE_DB_NAME")
            ?? configuration["SUPABASE_DB_NAME"] ?? "postgres";
        var user = Environment.GetEnvironmentVariable("SUPABASE_DB_USER")
            ?? configuration["SUPABASE_DB_USER"] ?? "postgres";
        var password = Environment.GetEnvironmentVariable("SUPABASE_DB_PASSWORD")
            ?? configuration["SUPABASE_DB_PASSWORD"];

        return $"Host={host};Port={port};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true;Search Path={schema}";
    }

    public static bool UseSupabaseDatabase()
        => string.Equals(Environment.GetEnvironmentVariable("USE_SUPABASE_DB"), "true", StringComparison.OrdinalIgnoreCase);
}
