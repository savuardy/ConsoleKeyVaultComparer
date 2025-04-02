using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Spectre.Console;

namespace ConsoleKeyVaulComparer
{
    /// <summary>
    /// A command-line tool for comparing secrets between Azure Key Vaults.
    /// </summary>
    public class Program
    {
        private const string DevKeyVaultUri = "https://kv-ft-als-dev.vault.azure.net/";
        private const string StageKeyVaultUri = "https://kv-ft-als-stage.vault.azure.net/";

        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>0 for success, 1 for failure.</returns>
        public static async Task<int> Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    PrintUsage();
                    return 1;
                }

                var command = args[0].ToLower();
                var devClient = new SecretClient(new Uri(DevKeyVaultUri), new DefaultAzureCredential());
                var stageClient = new SecretClient(new Uri(StageKeyVaultUri), new DefaultAzureCredential());

                return await ExecuteCommand(command, devClient, stageClient);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                return 1;
            }
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        private static async Task<int> ExecuteCommand(string command, SecretClient devClient, SecretClient stageClient)
        {
            switch (command)
            {
                case "dev":
                    await ExecuteDevCommand(devClient);
                    break;

                case "stage":
                    await ExecuteStageCommand(stageClient);
                    break;

                case "compare":
                    await ExecuteCompareCommand(devClient, stageClient);
                    break;

                default:
                    AnsiConsole.MarkupLine($"[red]Unknown command: {command}[/]");
                    PrintUsage();
                    return 1;
            }

            return 0;
        }

        /// <summary>
        /// Executes the dev command to list secrets from dev Key Vault.
        /// </summary>
        private static async Task ExecuteDevCommand(SecretClient client)
        {
            var table = new Table();
            table.AddColumn("№").RightAligned();
            table.AddColumn("Secret Name");
            table.AddColumn("Value");
            table.Title = new TableTitle("Secrets from dev Key Vault");
            table.Border = TableBorder.Rounded;
            table.Expand();

            var count = 0;
            var errorCount = 0;
            var maxLength = 0;
            var totalLength = 0;

            await foreach (var secretProperties in client.GetPropertiesOfSecretsAsync())
            {
                try
                {
                    var secret = await client.GetSecretAsync(secretProperties.Name);
                    var value = secret.Value.Value ?? string.Empty;
                    maxLength = Math.Max(maxLength, value.Length);
                    totalLength += value.Length;
                    table.AddRow($"{count + 1}", secretProperties.Name, value);
                    count++;
                }
                catch (Exception ex)
                {
                    table.AddRow($"{count + 1}", secretProperties.Name, $"[red]Error - {ex.Message}[/]");
                    errorCount++;
                    count++;
                }
            }

            AnsiConsole.Write(table);

            var statsTable = new Table();
            statsTable.AddColumn("Metric");
            statsTable.AddColumn("Value");
            statsTable.Title = new TableTitle("Statistics");
            statsTable.Border = TableBorder.Rounded;
            statsTable.Expand();

            statsTable.AddRow("Total secrets", count.ToString());
            statsTable.AddRow("Success rate", $"{(count - errorCount) * 100.0 / count:F1}% ({count - errorCount}/{count})");
            statsTable.AddRow("Max value length", $"{maxLength} characters");
            statsTable.AddRow("Avg value length", $"{totalLength / (count - errorCount):F1} characters");

            AnsiConsole.Write(statsTable);

            var successRate = (count - errorCount) * 100.0 / count;
            AnsiConsole.Write(new Rule("[blue]Success Rate[/]"));
            
            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask("Success Rate");
                    task.MaxValue = 100;
                    task.Increment(successRate);
                });
        }

        /// <summary>
        /// Executes the stage command to list secrets from stage Key Vault.
        /// </summary>
        private static async Task ExecuteStageCommand(SecretClient client)
        {
            var table = new Table();
            table.AddColumn("№").RightAligned();
            table.AddColumn("Secret Name");
            table.AddColumn("Value");
            table.Title = new TableTitle("Secrets from stage Key Vault");
            table.Border = TableBorder.Rounded;
            table.Expand();

            var count = 0;
            var errorCount = 0;
            var maxLength = 0;
            var totalLength = 0;

            await foreach (var secretProperties in client.GetPropertiesOfSecretsAsync())
            {
                try
                {
                    var secret = await client.GetSecretAsync(secretProperties.Name);
                    var value = secret.Value.Value ?? string.Empty;
                    maxLength = Math.Max(maxLength, value.Length);
                    totalLength += value.Length;
                    table.AddRow($"{count + 1}", secretProperties.Name, value);
                    count++;
                }
                catch (Exception ex)
                {
                    table.AddRow($"{count + 1}", secretProperties.Name, $"[red]Error - {ex.Message}[/]");
                    errorCount++;
                    count++;
                }
            }

            AnsiConsole.Write(table);

            var statsTable = new Table();
            statsTable.AddColumn("Metric");
            statsTable.AddColumn("Value");
            statsTable.Title = new TableTitle("Statistics");
            statsTable.Border = TableBorder.Rounded;
            statsTable.Expand();

            statsTable.AddRow("Total secrets", count.ToString());
            statsTable.AddRow("Success rate", $"{(count - errorCount) * 100.0 / count:F1}% ({count - errorCount}/{count})");
            statsTable.AddRow("Max value length", $"{maxLength} characters");
            statsTable.AddRow("Avg value length", $"{totalLength / (count - errorCount):F1} characters");

            AnsiConsole.Write(statsTable);

            var successRate = (count - errorCount) * 100.0 / count;
            AnsiConsole.Write(new Rule("[blue]Success Rate[/]"));
            
            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask("Success Rate");
                    task.MaxValue = 100;
                    task.Increment(successRate);
                });
        }

        /// <summary>
        /// Executes the compare command to compare secrets between Key Vaults.
        /// </summary>
        private static async Task ExecuteCompareCommand(SecretClient devClient, SecretClient stageClient)
        {
            AnsiConsole.MarkupLine("\n[blue]Comparing secrets from both Key Vaults...[/]");
            var devSecrets = await GetSecretsAsync(devClient);
            var stageSecrets = await GetSecretsAsync(stageClient);
            CompareSecrets(devSecrets, stageSecrets);
        }

        /// <summary>
        /// Prints usage instructions to the console.
        /// </summary>
        private static void PrintUsage()
        {
            var table = new Table();
            table.AddColumn("Command");
            table.AddColumn("Description");
            table.Title = new TableTitle("Usage Instructions");
            table.Border = TableBorder.Rounded;
            table.Expand();

            table.AddRow("dev", "List secrets from dev Key Vault");
            table.AddRow("stage", "List secrets from stage Key Vault");
            table.AddRow("compare", "Compare secrets between dev and stage Key Vaults");

            AnsiConsole.Write(table);
        }

        /// <summary>
        /// Retrieves all secrets from the specified Key Vault client.
        /// </summary>
        private static async Task<Dictionary<string, string?>> GetSecretsAsync(SecretClient client)
        {
            var result = new Dictionary<string, string?>();
            var errorCount = 0;
            var totalCount = 0;

            await foreach (var secretProperties in client.GetPropertiesOfSecretsAsync())
            {
                totalCount++;
                try
                {
                    var secret = await client.GetSecretAsync(secretProperties.Name);
                    result[secretProperties.Name] = secret.Value.Value ?? string.Empty;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    AnsiConsole.MarkupLine($"[yellow]Warning: Could not retrieve secret {secretProperties.Name}: {ex.Message}[/]");
                    result[secretProperties.Name] = "[FETCH_ERROR]";
                }
            }

            if (errorCount > 0)
            {
                AnsiConsole.MarkupLine($"[yellow]Warning: {errorCount} out of {totalCount} secrets could not be retrieved.[/]");
            }

            return result;
        }

        /// <summary>
        /// Compares secrets between two Key Vaults and prints differences.
        /// </summary>
        private static void CompareSecrets(Dictionary<string, string?> kv1, Dictionary<string, string?> kv2)
        {
            var allKeys = new HashSet<string>(kv1.Keys);
            allKeys.UnionWith(kv2.Keys);

            var differencesFound = false;
            var table = new Table();
            table.AddColumn("Secret Name");
            table.AddColumn("Dev Value");
            table.AddColumn("Stage Value");
            table.Title = new TableTitle("Differences Found");
            table.Border = TableBorder.Rounded;
            table.Expand();

            var totalSecrets = allKeys.Count;
            var matchingSecrets = 0;
            var devOnlySecrets = 0;
            var stageOnlySecrets = 0;
            var differentValues = 0;

            foreach (var key in allKeys)
            {
                kv1.TryGetValue(key, out var value1);
                kv2.TryGetValue(key, out var value2);

                if (value1 != value2)
                {
                    differencesFound = true;
                    if (value1 == null)
                    {
                        stageOnlySecrets++;
                        table.AddRow(
                            key,
                            "[red]Not Found[/]",
                            FormatValue(value2)
                        );
                    }
                    else if (value2 == null)
                    {
                        devOnlySecrets++;
                        table.AddRow(
                            key,
                            FormatValue(value1),
                            "[red]Not Found[/]"
                        );
                    }
                    else
                    {
                        differentValues++;
                        table.AddRow(
                            key,
                            FormatValue(value1),
                            FormatValue(value2)
                        );
                    }
                }
                else
                {
                    matchingSecrets++;
                }
            }

            var statsTable = new Table();
            statsTable.AddColumn("Metric");
            statsTable.AddColumn("Value");
            statsTable.Title = new TableTitle("Comparison Statistics");
            statsTable.Border = TableBorder.Rounded;
            statsTable.Expand();

            statsTable.AddRow("Total secrets", totalSecrets.ToString());
            statsTable.AddRow("Matching secrets", matchingSecrets.ToString());
            statsTable.AddRow("Different values", differentValues.ToString());
            statsTable.AddRow("Dev-only secrets", devOnlySecrets.ToString());
            statsTable.AddRow("Stage-only secrets", stageOnlySecrets.ToString());
            statsTable.AddRow("Match rate", $"{matchingSecrets * 100.0 / totalSecrets:F1}%");

            AnsiConsole.Write(statsTable);

            if (!differencesFound)
            {
                AnsiConsole.MarkupLine("\n[green]No differences found between Key Vaults.[/]");
            }
            else
            {
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine("\n[green]Comparison complete.[/]");
            }

            var matchRate = matchingSecrets * 100.0 / totalSecrets;
            AnsiConsole.Write(new Rule("[blue]Match Rate[/]"));
            
            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask("Match Rate");
                    task.MaxValue = 100;
                    task.Increment(matchRate);
                });
        }

        private static string FormatValue(string? value)
        {
            if (value == null) return "[red]Not Found[/]";
            if (value.Length > 300) return "[yellow]Value too long to display[/]";
            return value;
        }
    }
} 