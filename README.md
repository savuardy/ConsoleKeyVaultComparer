# Azure Key Vault Secret Comparer

A command-line tool for comparing secrets between Azure Key Vaults. This tool helps you identify differences between secrets stored in different Azure Key Vault environments.

## Prerequisites

- Azure CLI installed and configured with appropriate access to Key Vaults
- Azure Key Vault access permissions for the vaults you want to compare

## Installation

### For macOS (x64)

1. Download the latest release for macOS
2. Make the file executable:
   ```bash
   chmod +x keyvault-comparer
   ```
3. Move the file to a directory in your PATH (optional):
   ```bash
   sudo mv keyvault-comparer /usr/local/bin/
   ```

### For Windows

1. Download the latest release for Windows
2. Add the directory containing the executable to your PATH (optional)

### For Linux

1. Download the latest release for Linux
2. Make the file executable:
   ```bash
   chmod +x keyvault-comparer
   ```
3. Move the file to a directory in your PATH (optional):
   ```bash
   sudo mv keyvault-comparer /usr/local/bin/
   ```

## Usage

The tool supports three main commands:

1. List secrets from dev Key Vault:
   ```bash
   keyvault-comparer dev
   ```

2. List secrets from stage Key Vault:
   ```bash
   keyvault-comparer stage
   ```

3. Compare secrets between dev and stage Key Vaults:
   ```bash
   keyvault-comparer compare
   ```

## Building from Source

To build the application from source:

1. Clone the repository
2. Navigate to the project directory
3. Run the following command:
   ```bash
   dotnet publish -c Release -r <runtime-identifier>
   ```

Replace `<runtime-identifier>` with:
- `osx-x64` for macOS
- `win-x64` for Windows
- `linux-x64` for Linux

The executable will be created in the `bin/Release/net9.0/<runtime-identifier>/publish` directory.

## Security Note

This tool requires Azure credentials to access Key Vaults. Make sure you have the appropriate permissions and are using secure methods to manage your credentials. 