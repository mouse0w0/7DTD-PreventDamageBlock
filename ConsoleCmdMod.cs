using System.Collections.Generic;

namespace PreventDamageBlock;

public class ConsoleCmdMod : ConsoleCmdAbstract
{
    public override bool AllowedInMainMenu => true;
    public override string[] GetCommands() => new[]
    {
        "preventdamageblock",
        "pdb"
    };

    public override string GetDescription() => Localization.Get("PDB_ConsoleCmdDesc");

    public override string GetHelp() => Localization.Get("PDB_ConsoleCmdHelp");

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
        }
    }

    private static bool IsNoEnoughParam(List<string> _params, int _expectedCount)
    {
        if (_params.Count >= _expectedCount) return false;
        Log.Out(Localization.Get("PDB_NoEnoughParam"));
        return true;
    }
}