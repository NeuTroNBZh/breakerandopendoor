# Architecture - CS2 Retake Plugin (round_start)

## Objectives
- Break all non-door breakable objects at the start of each round.
- Open all doors at the start of each round.
- Never break a door (absolute gameplay rule).
- Provide a testable, extensible, and server-robust architecture.

## round_start Sequence
1. Receive round_start event via plugin entry point.
2. Start a short processing window (configurable time budget).
3. Scan map entities through a dedicated scanner.
4. Classify each entity:
   - Door
   - Non-door breakable
   - Excluded
   - Unknown
5. Apply decision pipeline:
   - If door: Open action.
   - If non-door breakable: Break action.
   - Otherwise: Ignore.
6. Log summary (opened doors, broken breakables, errors).

## Entity Classification
### Doors
Possible classification signals:
- Explicit door classnames (example: prop_door_rotating, func_door, func_door_rotating).
- Presence of opening inputs (Open, Unlock + Open).

Priority rule:
- Any entity recognized as a door is excluded from all break logic.

### Non-door Breakables
Possible classification signals:
- Historically breakable entities (example: func_breakable, func_breakable_surf, prop_physics* depending on engine flags).
- Presence of destruction inputs or breakability state.

Rule:
- A breakable entity is a break target only if it is not classified as a door.

### Exclusions
Examples:
- Critical gameplay/server entities (spawns, map logic, system triggers).
- Entities explicitly excluded by config.

## Decision Pipeline
Strict order to avoid gameplay regressions:
1. Validate entity (exists, valid handle, not already removed).
2. Check explicit exclusions (config list).
3. Check door (priority).
4. Check non-door breakable.
5. Ignore fallback.

Pseudo-rule:
- If entity is a door => OpenDoor(entity)
- Else if entity is breakable => BreakEntity(entity)
- Else => NoOp

## Error Handling and Performance
### Errors
- Each action is wrapped in local try/catch to avoid stopping global processing.
- Errors are counted and logged with classname and entity id.
- If OpenDoor fails, no break attempt is allowed for that entity.

### Performance
- Iterative scanning without unnecessary allocations (pre-allocated lists when possible).
- Configurable maximum entities processed per pass to avoid CPU spikes.
- Concise production logging (optional verbose mode).

## Test Plan
### Unit Tests
- Classifier: door has priority over breakable.
- Classifier: exclusions are respected.
- Coordinator: doors route to OpenDoor only.
- Coordinator: non-door breakables route to BreakEntity.
- Coordinator: local errors do not interrupt batch processing.

### Integration Tests (local server)
- Map with rotating doors: verify doors are open after round_start.
- Map with mixed breakables: verify only non-doors are broken.
- Load test: dense-entity map to validate time budget and stability.

### Gameplay Validation
- Visual checks and logs: no broken doors across multiple consecutive rounds.

## Planned Extensions
- Rule-based per-map classification strategy (json/yaml).
- Additional metrics (processing time, success rate per action type).
- Optional delayed post-round_start hook for maps requiring extra delay.
