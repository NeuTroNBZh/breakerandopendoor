using RetakePlugin.Config;

namespace RetakePlugin.Core;

public sealed class RoundStartCoordinator
{
    private readonly EntityScanner _scanner;
    private readonly EntityClassifier _classifier;
    private readonly ActionExecutor _executor;
    private readonly PluginConfig _config;

    public RoundStartCoordinator(
        EntityScanner scanner,
        EntityClassifier classifier,
        ActionExecutor executor,
        PluginConfig config)
    {
        _scanner = scanner;
        _classifier = classifier;
        _executor = executor;
        _config = config;
    }

    public RoundStartReport HandleRoundStart()
    {
        var report = new RoundStartReport();
        var entities = _scanner.ScanEntities(_config.MaxEntitiesPerRoundStart);
        report.Scanned = entities.Count;
        var probeAttempts = 0;

        foreach (var entity in entities)
        {
            try
            {
                var kind = _classifier.Classify(entity);
                switch (kind)
                {
                    case EntityKind.Door:
                        report.DoorsDetected++;
                        if (!_config.EnableOpenDoors)
                        {
                            report.Ignored++;
                            break;
                        }

                        if (_executor.OpenDoor(entity))
                        {
                            report.DoorsOpened++;
                        }
                        else
                        {
                            report.Errors++;
                        }
                        break;

                    case EntityKind.BreakableWindow:
                        report.BreakablesDetected++;
                        if (!_config.EnableBreakWindows)
                        {
                            report.Ignored++;
                            break;
                        }

                        if (_executor.BreakEntity(entity))
                        {
                            report.BreakablesBroken++;
                        }
                        else
                        {
                            report.Errors++;
                        }
                        break;

                    case EntityKind.BreakableVent:
                        report.BreakablesDetected++;
                        if (!_config.EnableBreakVents)
                        {
                            report.Ignored++;
                            break;
                        }

                        if (_executor.BreakEntity(entity))
                        {
                            report.BreakablesBroken++;
                        }
                        else
                        {
                            report.Errors++;
                        }
                        break;

                    case EntityKind.BreakableOther:
                        report.BreakablesDetected++;
                        if (!_config.EnableBreakOtherBreakables)
                        {
                            report.Ignored++;
                            break;
                        }

                        if (_executor.BreakEntity(entity))
                        {
                            report.BreakablesBroken++;
                        }
                        else
                        {
                            report.Errors++;
                        }
                        break;

                    case EntityKind.Excluded:
                        report.ExcludedDetected++;
                        report.Ignored++;
                        break;

                    case EntityKind.Unknown:
                    default:
                        report.UnknownDetected++;
                        report.Ignored++;

                        if (_config.ProbeUnknownEntitiesForBreakInput
                            && probeAttempts < _config.UnknownProbeMaxPerPass
                            && ShouldProbeUnknown(entity.ClassName))
                        {
                            probeAttempts++;
                            report.UnknownProbeAttempts++;

                            if (_executor.BreakEntity(entity))
                            {
                                report.UnknownProbeBroken++;
                                report.BreakablesBroken++;
                            }
                        }
                        break;
                }
            }
            catch
            {
                // Erreur locale: on continue le batch pour ne pas impacter le serveur.
                report.Errors++;
            }
        }

        return report;
    }

    private bool ShouldProbeUnknown(string className)
    {
        if (className.StartsWith("func_", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        foreach (var token in _config.UnknownProbeClassNameTokens)
        {
            if (className.Contains(token, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}

public sealed class RoundStartReport
{
    public int Scanned { get; set; }
    public int DoorsDetected { get; set; }
    public int BreakablesDetected { get; set; }
    public int ExcludedDetected { get; set; }
    public int UnknownDetected { get; set; }
    public int UnknownProbeAttempts { get; set; }
    public int UnknownProbeBroken { get; set; }
    public int DoorsOpened { get; set; }
    public int BreakablesBroken { get; set; }
    public int Ignored { get; set; }
    public int Errors { get; set; }
}
