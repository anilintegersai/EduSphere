# Academic Structure Depth

## Workspace

Open **Academic > Academic Planning** as a SuperAdmin or TenantAdmin. The workspace contains calendars, structured curriculum, lesson planning, teacher constraints, timetable generation, rollover and bulk-data tabs.

## Timetable Generation

Create branch time slots, rooms and teacher-subject assignments before generating. A template defines each subject's weekly period requirement and daily maximum. The generator creates a draft timetable and respects:

- teacher-subject and optional section assignments;
- unavailable and preferred teacher windows;
- section, teacher and room occupancy;
- room capacity and preferred rooms;
- effective-date overlap with existing non-archived timetables;
- subject periods per week and maximum periods per day.

Incomplete solutions are retained as drafts. `TimetableGenerationRuns` records scheduled and unscheduled counts with diagnostics so an administrator can resolve constraints and regenerate.

## Academic Rollover

Rollover copies terms into a target academic year, shifts dates relative to the two academic-year start dates, remaps copied calendar events to their new terms, and optionally clones reusable timetable templates. Existing term sequences and template names are skipped to keep reruns safe.

## Bulk Masters

The CSV workflow supports `Department`, `Course` and `Subject` rows. Parent rows must precede dependent rows in the file. Use **Validate preview** before import; invalid type, missing values, duplicate codes and unresolved parents are shown per row. Export produces the same four-column format: `Type,Code,Name,ParentCode`.

## Database Promotion

Apply migration `20260919110117_AcademicStructureDepth` or run `DatabaseScripts/20260919_AcademicStructureDepth_Promotion.sql`. The script is idempotent and starts after `20260919101602_TenantSettingsAdministration`.
