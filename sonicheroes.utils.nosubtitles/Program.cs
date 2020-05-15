using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using CallingConventions = Reloaded.Hooks.Definitions.X86.CallingConventions;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;

namespace sonicheroes.utils.nosubtitles
{
    public unsafe class Program : IMod
    {
        private IModLoader _modLoader;
        private IHook<PrintSubtitles> _printSubtitles;
        
        public void Start(IModLoaderV1 loader)
        {
            _modLoader = (IModLoader)loader;

            /* Your mod code starts here. */
            _modLoader.GetController<IReloadedHooks>().TryGetTarget(out var hooks);
            _printSubtitles = hooks.CreateHook<PrintSubtitles>(PrintSubtitlesImpl, 0x00428560).Activate();
        }

        private int PrintSubtitlesImpl(char* font, string text, IntPtr posX, IntPtr posY, IntPtr e, float f, float g)
        {
            return _printSubtitles.OriginalFunction(font, "", posX, posY, e, f, g);
        }

        /* Mod loader actions. */
        public void Suspend() { }
        public void Resume() { }
        public void Unload() { }

        /*  If CanSuspend == false, suspend and resume button are disabled in Launcher and Suspend()/Resume() will never be called.
            If CanUnload == false, unload button is disabled in Launcher and Unload() will never be called. */
        public bool CanUnload()  => false;
        public bool CanSuspend() => false;

        /* Automatically called by the mod loader when the mod is about to be unloaded. */
        public Action Disposing { get; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [Function(CallingConventions.Stdcall)]
        public unsafe delegate int PrintSubtitles(char* font, [MarshalAs(UnmanagedType.LPStr)] string text, IntPtr posX, IntPtr posY, IntPtr e, float f, float g);
    }
}
