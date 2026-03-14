# Contributing

Thanks for your interest in contributing.

## Prerequisites

- .NET SDK 8+
- PowerShell 5.1+

## Local Build

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build-release.ps1
```

## Coding Rules

- Keep changes focused and minimal.
- Preserve server stability and deterministic gameplay.
- Never break doors; doors should be opened only when enabled.
- Add logs for behavior changes that affect gameplay debugging.

## Pull Requests

- Describe the gameplay impact.
- Include steps to reproduce and validate.
- Mention tested maps (for example de_mirage, de_nuke).
