# Changelog

All notable changes to this project are documented in this file.

## [0.1.0] - 2026-03-14

### Added
- Initial CounterStrikeSharp plugin architecture for round-start entity handling.
- Configurable behavior toggles:
  - EnableOpenDoors
  - EnableBreakWindows
  - EnableBreakVents
  - EnableBreakOtherBreakables
- Multi-pass processing strategy for round_start/freeze_end timing.
- Diagnostic command css_bod_dump_break_candidates.
- Release packaging script and deployable zip output.

### Changed
- Plugin identity renamed to breakerandopendoor.
- Break handling hardened for stubborn vent/window entities.
