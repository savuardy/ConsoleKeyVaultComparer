# Azure Key Vault Secret Comparer

A command-line tool for comparing secrets between Azure Key Vaults. This tool helps you identify differences between secrets stored in different Azure Key Vault environments.

## Prerequisites

- Azure CLI installed and configured with appropriate access to Key Vaults
- Azure Key Vault access permissions for the vaults you want to compare

## Installation

### For macOS (x64)

1. Download the latest release for macOS
2. Extract the ZIP file to a folder
3. Open Terminal
4. Navigate to the extracted folder
5. Make the script executable:
   ```bash
   chmod +x run.sh
   ```

## Usage

The tool supports two main commands:

1. List secrets from a Key Vault:
   ```bash
   ./run.sh list <vault-uri>
   ```

2. Compare secrets between two Key Vaults:
   ```bash
   ./run.sh compare <source-vault-uri> <target-vault-uri>
   ```

### Examples
```bash
# List secrets from a Key Vault
./run.sh list https://my-vault.vault.azure.net/

# Compare two Key Vaults
./run.sh compare https://source-vault.vault.azure.net/ https://target-vault.vault.azure.net/
```

## Building from Source

To build the application from source:

1. Clone the repository
2. Navigate to the project directory
3. Run the following command:
   ```bash
   dotnet publish -c Release -r osx-x64
   ```

The executable will be created in the `bin/Release/net9.0/osx-x64/publish` directory.

## Security Note

This tool requires Azure credentials to access Key Vaults. Make sure you have the appropriate permissions and are using secure methods to manage your credentials.

## Detailed Instructions

For a complete step-by-step guide with troubleshooting tips, see [STEP_BY_STEP.md](STEP_BY_STEP.md) 