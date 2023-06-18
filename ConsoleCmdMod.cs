using System.Collections.Generic;

namespace PreventDamageBlock;

public class ConsoleCmdMod : ConsoleCmdAbstract
{
    public override bool AllowedInMainMenu => true;

    protected override string[] getCommands() => new[]
    {
        "preventdamageblock",
        "pdb"
    };

    protected override string getDescription() => Localization.Get("PDB_ConsoleCmdDesc");

    protected override string getHelp() => Localization.Get("PDB_ConsoleCmdHelp");

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        if (IsNoEnoughParam(_params, 1)) return;

        switch (_params[0])
        {
            case "reload":
            {
                Config.Load();
                Log.Out(Localization.Get("PDB_ReloadConfig"));
                return;
            }
            case "debug":
            {
                Main.Debug = !Main.Debug;
                Log.Out($"[PreventDamageBlock] Debug {(Main.Debug ? "Enabled" : "Disabled")}");
                return;
            }
        }
    }

    private static bool IsNoEnoughParam(List<string> _params, int _expectedCount)
    {
        if (_params.Count >= _expectedCount) return false;
        Log.Out(Localization.Get("PDB_NoEnoughParam"));
        return true;
    }
}