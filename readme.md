# Corrupt Index Fixer for Umbraco

The 'Corrupt Index Fixer'-package is a simple solution designed for Umbraco applications to 
automatically handle and recover from corrupted Examine indexes. This middleware ensures that 
your site's search functionality remains intact by detecting issues and triggering a controlled 
index rebuild when necessary.

## Installation

You can install the package via NuGet Package Manager or using the .NET CLI.

### Using .NET CLI

```bash
dotnet add package Knowit.Umbraco.CorruptIndexFixer
```

## Usage
Once installed, the middleware will automatically integrate with your Umbraco application. 
The middleware is composed automatically, so no additional configuration is required.