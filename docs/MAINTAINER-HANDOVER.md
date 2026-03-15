# Maintainer Handover Guide

This document is for maintainers who need to continue development and release management of BreakerAndOpenDoor.

## Project Goals
- Open doors at round start.
- Break non-door breakables.
- Never break doors.

## Tech Stack
- .NET 8
- CounterStrikeSharp plugin model
- GitHub Actions for CI and release

## Important Runtime Rule
Door entities must always be treated as open-only targets. Any behavior that can break doors is a regression.

## Repository Map
- src/RetakePlugin: plugin source
- addons/counterstrikesharp/configs/plugins/breakerandopendoor: default server config
- scripts/build-release.ps1: local release bundle script
- .github/workflows/ci.yml: CI build validation
- .github/workflows/release.yml: release creation from tags

## Release Process (Professional)
1. Merge tested changes to main.
2. Ensure CI is green.
3. Create a semantic tag: vMAJOR.MINOR.PATCH.
4. Push tag to origin.
5. Release workflow builds bundle and publishes GitHub Release with artifact.

Example:
```powershell
git tag -a v1.0.2 -m "Release v1.0.2"
git push origin v1.0.2
```

## Versioning Policy
- MAJOR: breaking behavior or architecture changes.
- MINOR: new features, non-breaking.
- PATCH: bug fixes and packaging/docs/CI fixes.

## Failure Recovery
If a tag workflow fails:
1. Fix workflow on main.
2. Create a new patch tag (do not reuse failed tag): vX.Y.(Z+1).
3. Push new tag.

## New Maintainer Checklist
- Verify plugin behavior on de_mirage and de_nuke.
- Confirm no door break behavior.
- Verify generated release zip contains:
  - addons/counterstrikesharp/plugins/breakerandopendoor/breakerandopendoor.dll
  - addons/counterstrikesharp/configs/plugins/breakerandopendoor/breakerandopendoor.json
- Confirm release notes are in English.

## Support Notes
- If users report missing release: check Actions run result for the tag.
- If users report map-specific breakables not breaking: inspect classnames via css_bod_dump_break_candidates and update config/classifier rules.
