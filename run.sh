#!/bin/bash

# Check if the executable exists
if [ ! -f "ConsoleKeyVaultComparer" ]; then
    echo "Error: ConsoleKeyVaultComparer executable not found!"
    echo "Please make sure you're in the correct directory and the executable exists."
    exit 1
fi

# Check if arguments are provided
if [ $# -lt 3 ]; then
    echo "Usage: ./run.sh <command> <source-vault-uri> <target-vault-uri>"
    echo "Commands: source, target, compare"
    echo "Example: ./run.sh compare https://source-vault.vault.azure.net/ https://target-vault.vault.azure.net/"
    echo ""
    echo "For detailed instructions, see STEP_BY_STEP.md"
    exit 1
fi

# Run the tool
./ConsoleKeyVaultComparer "$@" 