using HarmonyLib;

namespace PreventDamageBlock
{
    public class Main : IModApi
    {
        public static Mod Instance { get; private set; }
        
        public void InitMod(Mod _modInstance)
        {
            Instance = _modInstance;
            
            Config.Load();
            
            new Harmony("com.github.mouse0w0.preventblockdamage").PatchAll();
        }
    }
}