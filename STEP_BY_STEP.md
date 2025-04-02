# Step-by-Step Guide: Azure Key Vault Secret Comparer

This guide will help you use the Azure Key Vault Secret Comparer tool to compare secrets between different Azure Key Vaults.

## Prerequisites

### 1. Install Azure CLI
First, you need to install the Azure CLI:

1. Open Terminal
2. Run the following command:
   ```bash
   brew install azure-cli
   ```

### 2. Log in to Azure
1. Open Terminal
2. Run:
   ```bash
   az login
   ```
3. A browser window will open
4. Log in with your Azure account
5. Close the browser when done

### 3. Verify Installation
1. Run this command to check if you're logged in:
   ```bash
   az account show
   ```
2. You should see your account information

## Using the Tool

### 1. Download and Extract
1. Download the latest release ZIP file
2. Extract it to a folder
3. Open Terminal
4. Navigate to the extracted folder:
   ```bash
   cd path/to/extracted/folder
   ```
5. Make the script executable:
   ```bash
   chmod +x run.sh
   ```

### 2. Get Your Key Vault URIs
1. Open Azure Portal (portal.azure.com)
2. Go to your Key Vault
3. Click on "Overview"
4. Copy the "Vault URI" (looks like: https://your-vault.vault.azure.net/)

### 3. Available Commands

#### List Secrets from a Key Vault
```bash
./run.sh list <vault-uri>
```
Example:
```bash
./run.sh list https://my-vault.vault.azure.net/
```
This will show:
- All secrets in the specified Key Vault
- Total number of secrets
- A table with secret names and their values

#### Compare Two Key Vaults
```bash
./run.sh compare <source-vault-uri> <target-vault-uri>
```
Example:
```bash
./run.sh compare https://source-vault.vault.azure.net/ https://target-vault.vault.azure.net/
```

## Understanding the Output

### List Command Output
- Shows all secrets in the specified Key Vault
- Displays total number of secrets
- Shows a table with secret names and their values
- Values are truncated if too long for display

### Compare Command Output
The tool will show:
- Total number of secrets
- Number of matching secrets
- Number of secrets with different values
- Secrets only in source vault
- Secrets only in target vault
- Match rate percentage

### Value Display Rules
- Values are shown as "Not Found" if they don't exist in one vault
- Values are shown as "Different" if they exist in both vaults but have different values
- Values are shown as "Same" if they match exactly
- Long values are truncated for better readability

## Troubleshooting

### Common Issues

1. **"Not logged in" error**
   - Run `az login` again
   - Make sure you're using the correct account

2. **"Access denied" error**
   - Check if you have access to the Key Vault(s)
   - Ask your Azure administrator for access

3. **"Invalid URI" error**
   - Make sure you copied the full Key Vault URI
   - URI should end with .vault.azure.net/

4. **"Script not found" error**
   - Make sure you're in the correct directory
   - Verify the script is executable (`chmod +x run.sh`)

### Getting Help
If you encounter issues:
1. Check the error message
2. Verify your Azure CLI installation
3. Make sure you have the correct permissions
4. Contact your Azure administrator

## Security Notes

1. Never share your Azure credentials
2. Don't store Key Vault URIs in plain text
3. Use appropriate access controls
4. Log out when done:
   ```bash
   az logout
   ```

## Best Practices

1. Always verify Key Vault URIs before running commands
2. Keep your Azure CLI updated
3. Use the same account for both Key Vaults when comparing
4. Check permissions before running commands

## Example Workflows

### List Secrets from a Key Vault
1. Log in to Azure:
   ```bash
   az login
   ```

2. Navigate to tool directory:
   ```bash
   cd path/to/tool
   ```

3. List secrets:
   ```bash
   ./run.sh list https://my-vault.vault.azure.net/
   ```

4. Review the results in the table

### Compare Two Key Vaults
1. Log in to Azure:
   ```bash
   az login
   ```

2. Navigate to tool directory:
   ```bash
   cd path/to/tool
   ```

3. Compare Key Vaults:
   ```bash
   ./run.sh compare https://source-vault.vault.azure.net/ https://target-vault.vault.azure.net/
   ```

4. Review the results in the table

5. Log out when done:
   ```bash
   az logout
   ```

## Need More Help?
- Check the error messages in red
- Verify your Azure CLI is up to date
- Make sure you have the latest version of the tool
- Contact your Azure administrator for access issues 