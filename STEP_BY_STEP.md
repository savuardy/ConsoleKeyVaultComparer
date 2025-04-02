# Step-by-Step Guide: Azure Key Vault Secret Comparer

## Prerequisites

### 1. Install Azure CLI
```bash
# For macOS (using Homebrew)
brew update && brew install azure-cli
```

### 2. Login to Azure
```bash
az login
```
- This will open your browser
- Sign in with your Azure account
- Close the browser when done

### 3. Verify Azure CLI Installation
```bash
az account show
```
- Should display your Azure account information

## Using the Tool

### 1. Download and Extract
1. Download the latest release ZIP file
2. Extract the ZIP file to a folder
3. Open Terminal
4. Navigate to the extracted folder:
   ```bash
   cd /path/to/extracted/folder
   ```

### 2. Make the Script Executable
```bash
chmod +x run.sh
```

### 3. Get Your Key Vault URIs
1. Go to Azure Portal (portal.azure.com)
2. Navigate to your source Key Vault
3. Copy the Key Vault URI (looks like: `https://your-vault-name.vault.azure.net/`)
4. Repeat for your target Key Vault

### 4. Run the Tool

#### Option 1: Compare Two Key Vaults
```bash
./run.sh compare https://source-vault.vault.azure.net/ https://target-vault.vault.azure.net/
```
This will:
- Show a table of all differences
- Display statistics about matching and different secrets
- Show a progress bar of the match rate

#### Option 2: List Secrets from Source Key Vault
```bash
./run.sh source https://source-vault.vault.azure.net/ https://target-vault.vault.azure.net/
```
This will:
- Show all secrets from the source Key Vault
- Display statistics about the secrets
- Show a progress bar of successful retrievals

#### Option 3: List Secrets from Target Key Vault
```bash
./run.sh target https://source-vault.vault.azure.net/ https://target-vault.vault.azure.net/
```
This will:
- Show all secrets from the target Key Vault
- Display statistics about the secrets
- Show a progress bar of successful retrievals

### 5. Understanding the Output

#### Comparison Results
- **Matching Secrets**: Secrets that exist in both Key Vaults with the same value
- **Different Values**: Secrets that exist in both Key Vaults but have different values
- **Source-only Secrets**: Secrets that only exist in the source Key Vault
- **Target-only Secrets**: Secrets that only exist in the target Key Vault
- **Match Rate**: Percentage of secrets that match between the Key Vaults

#### Value Display Rules
- Values up to 300 characters are shown in full
- Values longer than 300 characters show as "[yellow]Value too long to display[/]"
- Missing secrets show as "[red]Not Found[/]"

### 6. Troubleshooting

#### Common Issues
1. **Permission Errors**
   - Make sure you're logged in to Azure CLI
   - Verify you have access to both Key Vaults
   - Try logging in again: `az login`

2. **Script Not Found**
   - Make sure you're in the correct directory
   - Verify the script is executable: `ls -l run.sh`

3. **Invalid URI**
   - Make sure the Key Vault URIs end with a forward slash (/)
   - Verify the URIs are correct from Azure Portal

#### Getting Help
- Check the error messages in red
- Verify your Azure CLI is up to date: `az upgrade`
- Make sure you have the latest version of the tool

## Security Notes
- The tool uses your Azure CLI credentials
- No secrets are stored locally
- All operations are performed in memory
- Make sure to close the terminal when done

## Best Practices
1. Always verify the Key Vault URIs before running
2. Use the compare command first to get an overview
3. Use source/target commands for detailed inspection
4. Keep your Azure CLI up to date
5. Log out of Azure CLI when done: `az logout`

## Example Workflow

### 1. First Time Setup
```bash
# Install Azure CLI
brew update && brew install azure-cli

# Login to Azure
az login

# Verify login
az account show
```

### 2. Using the Tool
```bash
# Navigate to the tool directory
cd /path/to/ConsoleKeyVaultComparer

# Make the script executable
chmod +x run.sh

# Compare two Key Vaults
./run.sh compare https://source-vault.vault.azure.net/ https://target-vault.vault.azure.net/
```

### 3. Cleanup
```bash
# Log out of Azure CLI
az logout

# Close the terminal
```

## Need More Help?
- Check the error messages in red
- Verify your Azure CLI is up to date
- Make sure you have the latest version of the tool
- Contact your Azure administrator for access issues 