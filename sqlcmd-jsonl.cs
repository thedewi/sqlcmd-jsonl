#if FALSE // Support `dotnet run` as a file-based app. See https://learn.microsoft.com/en-us/dotnet/core/sdk/file-based-apps
#:property PublishAot=false
#:package System.CommandLine@2.0.3
#:package Microsoft.Data.SqlClient@6.1.4
#:package Dapper@2.1.66
#endif

using System.CommandLine;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;

// Simplified from https://github.com/microsoft/go-sqlcmd/blob/main/cmd/sqlcmd/sqlcmd.go
Option<string> queryOption = new("--query", "-Q") { Description = "The queries to execute" };
Option<string> connStrOption = new("--connection-string") { Description = "The ADO.NET connection string" };
Option<string> serverOption = new("--server", "-S") { Description = "The server name or address" };
Option<string> databaseOption = new("--database-name", "--database", "-d") { Description = "The initial database" };
Option<string> usernameOption = new("--user-name", "--username", "-U") { Description = "The login or user name" };
Option<string> passwordOption = new("--password", "-P") { Description = "The password to authenticate with" };
Option<bool> useAadOption = new("--use-aad", "-G") { Description = "Use Active Directory Default authentication" };
Option<bool> trustCertOption = new("--trust-server-certificate", "-C")
    { Description = "Trust the server certificate without validation" };
Option<string[]> paramOption = new("--parameter", "--param", "--variables", "-v")
    { Description = "A parameter in the form name=value" };

RootCommand rootCommand = new("Execute SQL queries, yielding JSONLines results")
{
    Options =
    {
        queryOption, connStrOption, serverOption, databaseOption,
        usernameOption, passwordOption, useAadOption, trustCertOption, paramOption
    }
};
rootCommand.SetAction(async parseResult =>
{
    var queryArg = parseResult.GetValue(queryOption) ?? await Console.In.ReadToEndAsync();
    var connStrArg = parseResult.GetValue(connStrOption);
    var serverArg = parseResult.GetValue(serverOption);
    var databaseArg = parseResult.GetValue(databaseOption);
    var usernameArg = parseResult.GetValue(usernameOption);
    var passwordArg = parseResult.GetValue(passwordOption);
    var useAadArg = parseResult.GetValue(useAadOption);
    var trustCertArg = parseResult.GetValue(trustCertOption);
    var parameters = new DynamicParameters();
    foreach (var arg in parseResult.GetValue(paramOption) ?? [])
    {
        var parts = arg.Split('=', 2);
        if (parts.Length > 1)
            parameters.Add(parts[0], parts[1]);
    }

    try
    {
        // Documented at https://learn.microsoft.com/en-us/dotnet/api/microsoft.data.sqlclient.sqlconnection.connectionstring
        var connStr = new SqlConnectionStringBuilder(connStrArg);
        if (serverArg != null) connStr.DataSource = serverArg;
        if (databaseArg != null) connStr.InitialCatalog = databaseArg;
        if (usernameArg != null) connStr.UserID = usernameArg;
        if (passwordArg != null) connStr.Password = passwordArg;
        if (useAadArg) connStr.Authentication = SqlAuthenticationMethod.ActiveDirectoryDefault;
        if (trustCertArg) connStr.TrustServerCertificate = true;

        if (connStr.Authentication == default && string.IsNullOrEmpty(connStr.UserID)
                                              && string.IsNullOrEmpty(connStr.Password))
            connStr.IntegratedSecurity = true;
        if (string.IsNullOrEmpty(connStr.DataSource))
            connStr.TrustServerCertificate = true;
        await using var conn = new SqlConnection(connStr.ConnectionString);

        await using var stdout = Console.OpenStandardOutput();
        var newline = "\n"u8.ToArray();
        await using var reader = await conn.QueryMultipleAsync(queryArg, parameters);
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
