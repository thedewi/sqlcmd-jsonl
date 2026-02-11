#:property PublishAot=false
#:package System.CommandLine@2.0.3
#:package Microsoft.Data.SqlClient@6.1.4
#:package Dapper@2.1.66

using System.CommandLine;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;

Option<string> queryOption = new("--query", "-Q") { Description = "The queries to execute" };
Option<string> connStrOption = new("--connection-string") { Description = "The ADO.NET connection string" };
Option<string> serverOption = new("--server", "-S") { Description = "The server name or address" };
Option<string> databaseOption = new("--database-name", "--database", "-d") { Description = "The initial database" };
Option<bool> useAadOption = new("--use-aad", "-G") { Description = "Use Active Directory Default authentication" };

RootCommand rootCommand = new("Execute SQL queries, yielding JSONLines results")
{
    Options =
    {
        queryOption, connStrOption, serverOption, databaseOption,
        useAadOption
    }
};
rootCommand.SetAction(async parseResult =>
{
    var queryArg = parseResult.GetValue(queryOption) ?? await Console.In.ReadToEndAsync();
    var connStrArg = parseResult.GetValue(connStrOption);
    var serverArg = parseResult.GetValue(serverOption);
    var databaseArg = parseResult.GetValue(databaseOption);
    var useAadArg = parseResult.GetValue(useAadOption);

    try
    {
        var connStr = new SqlConnectionStringBuilder(connStrArg);
        if (serverArg != null) connStr.DataSource = serverArg;
        if (databaseArg != null) connStr.InitialCatalog = databaseArg;
        if (useAadArg) connStr.Authentication = SqlAuthenticationMethod.ActiveDirectoryDefault;

        if (connStr.Authentication == default)
            connStr.IntegratedSecurity = true;
        await using var conn = new SqlConnection(connStr.ConnectionString);

        await using var stdout = Console.OpenStandardOutput();
        var newline = "\n"u8.ToArray();
        await using var reader = await conn.QueryMultipleAsync(queryArg);
        while (!reader.IsConsumed)
            await foreach (var row in reader.ReadUnbufferedAsync())
            {
                await JsonSerializer.SerializeAsync(stdout, row);
                await stdout.WriteAsync(newline);
            }

        return 0;
    }
    catch (SqlException ex)
    {
        var origColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(ex.Message);
        Console.ForegroundColor = origColor;
        return 1;
    }
});
return await rootCommand.Parse(args).InvokeAsync();
