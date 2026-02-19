# KN Poker - Texas Hold'em Equity Calculator

A Texas Hold'em poker equity calculator that compares hand ranges to determine win percentages. Built with .NET 9, featuring both a command-line interface and a Blazor web UI.

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)

## Features

- **Range vs Range Equity Calculation** - Compare two poker hand ranges against each other
- **Full Board Enumeration** - Calculates exact equity by enumerating all possible board runouts
- **Multiple Interfaces** - Use via command line or web browser
- **Detailed Breakdowns** - View equity for each hand combination within the ranges
- **Progress Tracking** - Web UI shows calculation progress with cancel support

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Project Structure

```
KNPoker/
??? KNPoker/           # Command-line application
??? KNPokerWeb/        # Blazor web application
??? KNPokerLib/        # Core library with equity calculation logic
??? KNPokerTests/      # Unit tests
```

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/fnovitsk/KNPoker.git
cd KNPoker
```

### Restore Dependencies

```bash
dotnet restore
```

### Build the Solution

```bash
dotnet build
```

## Running the Applications

### Web UI (Recommended for Interactive Use)

```bash
cd KNPokerWeb
dotnet run
```

Then open your browser to `https://localhost:5001` (or the URL shown in the console).

**Using the Web UI:**
1. Enter a range in the "Range 1" field (e.g., `AA-KK`)
2. Enter a range in the "Range 2" field (e.g., `QQ-JJ`)
3. Click "Calculate Equity"
4. View the results showing win percentages and detailed combo breakdowns

### Command Line Interface

**Calculate Equity:**
```bash
cd KNPoker
dotnet run -- equity "AA-KK" "QQ-JJ"
```

**Generate Hand Combos:**
```bash
dotnet run -- gencombos "AKs" "QQ"
```

**Create Cache File:**
```bash
dotnet run -- createCache ./cache.csv
```

## Range Notation

The calculator uses standard poker range notation:

| Notation | Description | Example |
|----------|-------------|---------|
| `AA` | Pocket pair | Pocket Aces |
| `AA-TT` | Pair range | Aces through Tens |
| `AKs` | Suited hand | Ace-King suited |
| `AKo` | Offsuit hand | Ace-King offsuit |
| `AKs-ATs` | Suited range | AKs, AQs, AJs, ATs |
| `AKo-ATo` | Offsuit range | AKo, AQo, AJo, ATo |
| `AA, KK, AKs` | Multiple ranges | Combine with commas |

### Examples

```bash
# Premium pairs vs premium pairs
dotnet run -- equity "AA-KK" "QQ-JJ"

# Pocket aces vs suited connectors
dotnet run -- equity "AA" "JTs"

# Wide range vs tight range
dotnet run -- equity "AA-TT, AKs-ATs, KQs" "AA-QQ, AKs"
```

## Running Tests

```bash
dotnet test
```

## Sample Output

**CLI Output:**
```
AcAd,0.821,QcQd,0.173,0.006,1
AcAd,0.821,QcQh,0.173,0.006,1
...
Range 1 Wins,0.819
Range 2 Wins,0.175
Ranges Tie,0.006
```

**Web UI:**
- Visual progress bar during calculation
- Summary cards showing win percentages
- Equity bar chart visualization
- Detailed table with per-combo results

## Technical Details

- **Equity Calculation**: Uses full enumeration of all possible 5-card boards (1,712,304 combinations per hand matchup)
- **Suit Isomorphism**: Reduces computation by unifying equivalent suit patterns
- **Parallel Processing**: Utilizes multiple CPU cores for faster calculations
- **Hand Evaluation**: Uses the HoldemPoker.Evaluator library for hand ranking

## Dependencies

- [HoldemPoker.Cards](https://www.nuget.org/packages/HoldemPoker.Cards) - Card representation
- [HoldemPoker.Evaluator](https://www.nuget.org/packages/HoldemPoker.Evaluator) - Hand evaluation
- [MoreLINQ](https://www.nuget.org/packages/morelinq) - Extended LINQ operations
- [Spectre.Console](https://www.nuget.org/packages/Spectre.Console) - CLI framework
