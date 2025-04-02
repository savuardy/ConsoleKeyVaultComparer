using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Spectre.Console;
using System.CommandLine;

namespace ConsoleKeyVaultComparer
{
    /// <summary>
    /// A command-line tool for comparing secrets between Azure Key Vaults.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>0 for success, 1 for failure.</returns>
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Azure Key Vault Secret Comparer - A tool for comparing secrets between Azure Key Vaults");

            var listCommand = new Command("list", "List secrets from a Key Vault");
            var vaultUriArgument = new Argument<string>("vault-uri", "The URI of the Key Vault");
            listCommand.AddArgument(vaultUriArgument);
            listCommand.SetHandler(async (string vaultUri) =>
            {
                await ExecuteListCommand(vaultUri);
            }, vaultUriArgument);

            var compareCommand = new Command("compare", "Compare secrets between two Key Vaults");
            var sourceVaultArgument = new Argument<string>("source-vault-uri", "The URI of the source Key Vault");
            var targetVaultArgument = new Argument<string>("target-vault-uri", "The URI of the target Key Vault");
            compareCommand.AddArgument(sourceVaultArgument);
            compareCommand.AddArgument(targetVaultArgument);
            compareCommand.SetHandler(async (string sourceVaultUri, string targetVaultUri) =>
            {
                await ExecuteCompareCommand(sourceVaultUri, targetVaultUri);
            }, sourceVaultArgument, targetVaultArgument);

            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(compareCommand);

            return await rootCommand.InvokeAsync(args);
        }

        /// <summary>
        /// Executes the list command to list secrets from a Key Vault.
        /// </summary>
        private static async Task ExecuteListCommand(string vaultUri)
        {
            try
            {
                var credential = new DefaultAzureCredential();
                var client = new SecretClient(new Uri(vaultUri), credential);

                var table = new Table();
                table.AddColumn("№").RightAligned();
                table.AddColumn("Secret Name");
                table.AddColumn("Value");
                table.Title = new TableTitle($"Secrets from Key Vault: {vaultUri}");
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
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }
        }

        /// <summary>
        /// Executes the compare command to compare secrets between two Key Vaults.
        /// </summary>
        private static async Task ExecuteCompareCommand(string sourceVaultUri, string targetVaultUri)
        {
            try
            {
                var credential = new DefaultAzureCredential();
                var sourceClient = new SecretClient(new Uri(sourceVaultUri), credential);
                var targetClient = new SecretClient(new Uri(targetVaultUri), credential);

                AnsiConsole.MarkupLine("\n[blue]Comparing secrets from both Key Vaults...[/]");
                var sourceSecrets = await GetSecretsAsync(sourceClient);
                var targetSecrets = await GetSecretsAsync(targetClient);
                CompareSecrets(sourceSecrets, targetSecrets, sourceVaultUri, targetVaultUri);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }
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
                    result[secretProperties.Name] = null;
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
        private static void CompareSecrets(
            Dictionary<string, string?> sourceSecrets,
            Dictionary<string, string?> targetSecrets,
            string sourceVaultUri,
            string targetVaultUri)
        {
            var allKeys = new HashSet<string>(sourceSecrets.Keys);
            allKeys.UnionWith(targetSecrets.Keys);

            var differencesFound = false;
            var table = new Table();
            table.AddColumn("Secret Name");
            table.AddColumn("Source Value");
            table.AddColumn("Target Value");
            table.Title = new TableTitle("Differences Found");
            table.Border = TableBorder.Rounded;
            table.Expand();

            var totalSecrets = allKeys.Count;
            var matchingSecrets = 0;
            var sourceOnlySecrets = 0;
            var targetOnlySecrets = 0;
            var differentValues = 0;

            foreach (var key in allKeys)
            {
                sourceSecrets.TryGetValue(key, out var value1);
                targetSecrets.TryGetValue(key, out var value2);

                if (value1 != value2)
                {
                    differencesFound = true;
                    if (value1 == null)
                    {
                        targetOnlySecrets++;
                        table.AddRow(
                            key,
                            "[red]Not Found[/]",
                            FormatValue(value2)
                        );
                    }
                    else if (value2 == null)
                    {
                        sourceOnlySecrets++;
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
            statsTable.AddRow("Source-only secrets", sourceOnlySecrets.ToString());
            statsTable.AddRow("Target-only secrets", targetOnlySecrets.ToString());
            statsTable.AddRow("Match rate", $"{matchingSecrets * 100.0 / totalSecrets:F1}%");

            AnsiConsole.Write(new Rule($"[bold blue]Source Key Vault: {sourceVaultUri}[/]").RuleStyle("blue"));
            AnsiConsole.Write(new Rule($"[bold blue]Target Key Vault: {targetVaultUri}[/]").RuleStyle("blue"));
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

        /// <summary>
        /// Formats a secret value for display.
        /// </summary>
        private static string FormatValue(string? value)
        {
            if (value == null) return "[red]Not Found[/]";
            if (value.Length > 300) return "[yellow]Value too long to display[/]";
            return value;
        }
    }
} 