# Cryptie

Cryptie is a secure text messenger application featuring end-to-end encryption, digital signatures, and a robust three-layer architecture.

## Prerequisites

- [.NET 9 SDK (or later)](https://dotnet.microsoft.com/download)
- macOS (for the client build step, at least for now)
- Bash shell (for running the macOS build script, at least for now)

## Build and Run

### 1. Server & Shared Libraries

From the root of the repository, navigate to the release folder and build the Cryptie server and shared libraries:

```bash
cd path/to/repository
dotnet build --configuration Release ./src
```

This will compile the server and shared DTO/model libraries in Release mode.

### 2. Client (macOS)

To build the Avalonia-based macOS client, do the following:

```bash
cd path/to/repository/src/Cryptie.Client
chmod +x build-macos-app.sh
./build-macos-app.sh
```

After the script completes, you will find `Cryptie.app` in the same folder. This `.app` bundle is ready to run on macOS:

```bash
open Cryptie.app
```

## Usage

- **Server**: Once the server build completes, launch it with:
  
  ```bash
  dotnet ./src/Cryptie.Server/bin/Release/net6.0/Cryptie.Server.dll
  ```
  
- **Client (macOS)**: Double-click `Cryptie.app` in Finder or use:

  ```bash
  open Cryptie.app
  ```

## Project Structure

```
src/
├── Cryptie.ServerTests/      # Server-side unit and integration tests
├── Cryptie.Server/           # Server-side application (API, key management, SignalR hubs)
├── Cryptie.Common.Tests/     # Shared library tests
├── Cryptie.Common/           # DTOs and models shared by server and client
├── Cryptie.Client.Tests/     # Client-side tests
└── Cryptie.Client/           # Avalonia UI client
    ├── build-macos-app.sh    # macOS client build script
```

## License

This project is released under the MIT License.
