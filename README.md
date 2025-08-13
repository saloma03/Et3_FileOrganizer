# 📂 File Organizer


File Organizer is a desktop application that automatically organizes files in a specified folder based on their extensions. With its user-friendly interface, simulation mode, and undo functionality, it brings order to your digital chaos!

## 🔍 Table of Contents
- [🌟 Features](#-features)
- [🛠 Technologies Used](#-technologies-used)
- [🚀 Installation & Setup](#-installation--setup)
  - [System Requirements](#system-requirements)
  - [🖥️ Command Line Setup](#-command-line-setup)
  - [🛠️ Building from Source](#-building-from-source)
  - [▶️ Running the Application](#-running-the-application)
- [🧪 Testing](#-testing-the-application)
  - [Unit Tests](#unit-tests)
  - [Test Cheatsheet](#test-command-cheatsheet)
- [🛠 Troubleshooting](#-troubleshooting)
- [📂 Project Structure](#-project-structure)
- [📊 Diagrams](#-diagrams)
- [📝 Notes for Reviewers](#-notes-for-reviewers)
- [📬 Contact](#-contact)
## 🌟 Features

- 🗂 **Automatic File Organization** - Sorts files into folders by extension (e.g., `.jpg` → `Images`)
- 👀 **Simulation Mode** - Preview changes before applying them
- ↩️ **Undo Functionality** - Revert your last organization operation
- ⚙️ **Customizable Rules** - Define your own organization rules via JSON
- 🎨 **Modern UI** - Clean WPF interface with smooth animations
- ⚠️ **Conflict Handling** - Automatically resolves duplicate filenames
- 📝 **Activity Logging** - Keep track of all operations

## 🛠 Technologies Used

| Category       | Technologies |
|----------------|--------------|
| Language       | ![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white) |
| UI Framework   | ![WPF](https://img.shields.io/badge/WPF-5C2D91?logo=.net&logoColor=white) |
| Testing       | ![xUnit](https://img.shields.io/badge/xUnit-5C2D91?logo=.net&logoColor=white) ![FluentAssertions](https://img.shields.io/badge/Fluent_Assertions-512BD4) |
| Tools         | ![Visual Studio](https://img.shields.io/badge/Visual_Studio-5C2D91?logo=visual-studio&logoColor=white) ![Git](https://img.shields.io/badge/Git-F05032?logo=git&logoColor=white) |

## 🚀 How to Run

### Prerequisites
- [.NET Framework 8.0+](https://dotnet.microsoft.com/download/dotnet-framework)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended)
- Windows

## 🚀 Installation & Setup

### System Requirements
- Windows 10/11 (64-bit)
- .NET 8.0 Runtime
- 2GB RAM minimum
- 200MB disk space

### 🖥️ Command Line Setup
```bash
# Clone repository (HTTPS)
git clone https://github.com/saloma03/Et3_FileOrganizer.git

# Change directory
cd Et3_FileOrganizer

# Alternative SSH clone
# git clone git@github.com:saloma03/Et3_FileOrganizer.git
```
### 🛠️ Building from Source

```bash
# Restore NuGet packages
dotnet restore FileOrganizer.sln

# Build solution (Debug mode)
dotnet build FileOrganizer.sln --configuration Debug

# Build solution (Release mode)
dotnet build FileOrganizer.sln --configuration Release
```
### ▶️ Running the Application
```bash 
# Run in Debug mode
dotnet run --project FileOrganizer/FileOrganizer.csproj

# Run in Release mode
dotnet run --project FileOrganizer/FileOrganizer.csproj --configuration Release
``` 
### 🧪 Testing the Application
### Unit Tests
```bash 
# Run all tests
dotnet test FileOrganizer.Tests/FileOrganizer.Tests.csproj

# Run specific test class
dotnet test --filter "FullyQualifiedName=FileOrganizer.Tests.FileManagerTests"

# Generate test coverage report (requires coverlet)
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

```
### Test Command Cheatsheet

- **`dotnet test`**  
  Run all tests.

- **`dotnet test --filter "Category=Integration"`**  
  Run only the integration tests.

- **`dotnet test --logger:"console;verbosity=detailed"`**  
  Get detailed output during the test execution.

### 🛠️ Troubleshooting

#### Common Issues
```bash 
# Clean and rebuild solution
dotnet clean FileOrganizer.sln
dotnet build FileOrganizer.sln

# Reset NuGet cache
dotnet nuget locals all --clear

# Verify .NET installation
dotnet --list-sdks
dotnet --list-runtimes
```
### 📂 Project Structure
```bash 
Et3_FileOrganizer/
├── 📁 FileOrganizer/
│   ├── 📁 Core/
│   │   ├── � FileManager.cs        # Handles file scanning and moving
│   │   ├── �FilesOrganizer.cs     # Core logic for organizing files
│   │   ├── 🔧SettingManager.cs     # Manages JSON-based rules
│   │   ├── ↩️UndoManager.cs        # Handles undo operations
│   │   ├── Rule.cs               # Model for organization rules
│   │   ├── 📁 Logging/
│   │   │   ├── ActionLogger.cs   # Logs operations
│   │   ├── 📁 Interfaces/
│   │   │   ├── ICommand.cs       # Command Pattern interface
│   │   ├── 📁 Commands/
│   │   │   ├── OrganizeCommand.cs # Command for file moves
│   ├── 📁 Models/
│   │   ├── FileModel.cs          # File metadata model
│   ├── MainWindow.xaml           # Main UI
│   ├── MainWindow.xaml.cs        # Main UI code-behind
│   ├── PreviewWindow.xaml        # Simulation preview UI
│   ├── PreviewWindow.xaml.cs     # Preview UI code-behind
├── 📁 FileOrganizer.Tests/
│   ├── 📁 UnitTests/
│   │   ├── FileManagerTests.cs   # Tests for FileManager
│   │   ├── SettingManagerTests.cs # Tests for SettingManager
├── 📄 settings.json                 # Rule configuration
├── FileOrganizer.sln             # Solution file
├── 📄 README.md                     # This file
```
### 📊 Diagrams 
#### UseCase Diagrams
<img width="712" height="472" alt="FileOrganizerUseCase drawio" src="https://github.com/user-attachments/assets/2a4cc1d3-1455-4cbb-884b-1da7b9fd6de1" />

#### Class Diagram 
<img width="1151" height="808" alt="image" src="https://github.com/user-attachments/assets/c906fbf8-6272-48c3-af8e-3880227f9cc7" />

#### Sequence Diagram
<img width="1151" height="808" alt="image" src="https://github.com/user-attachments/assets/3ccad3fb-e6ab-4c57-ab69-dcaa250ad8ec" />

### 📝 Notes for Reviewers
### `Getting Started: `





- Start with MainWindow.xaml.cs for UI flow and FilesOrganizer.cs for core logic.



- Check settings.json for rule configuration.



- Run unit tests in FileOrganizer.Tests using dotnet test or Visual Studio Test Explorer.



### ` Key Features:`

- Simulation Mode: Preview file moves in PreviewWindow using StartOrganization(simulate: true).



- Undo: Uses Command Pattern (OrganizeCommand and UndoManager) for reversible operations.



- Testing: Unit tests use MockFileSystem for isolated testing, ensuring no real file system changes.



### ` Design Choices:`

- Uses System.IO.Abstractions for testability and dependency injection.



- Implements clean code principles (SOLID) for maintainability.

- Command design pattern
- Dependency Injection (DI):  

### ` Future Improvements:`






- Add recursive folder scanning for subdirectories.



- Implement a UI for editing rules in settings.json.



- Introduce multithreading for better performance with large folders.



- Enhance logging with a structured library like Serilog.

### 📬 Contact
For questions or feedback, please open an issue on GitHub or reach out via [salmaom3r@gmail.com](mailto:salmaom3r@gmail.com).
