namespace Fika_Installer.Models
{
    public enum SptValidationResult
    {
        OK,
        INVALID_SPT_FOLDER,
        MODS_DETECTED,
        ASSEMBLY_CSHARP_NOT_DEOBFUSCATED,
    }
}
