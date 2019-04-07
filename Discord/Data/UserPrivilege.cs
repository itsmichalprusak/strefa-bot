using System;

namespace Discord.Data
{
    [Flags]
    public enum UserPrivilege
    {
        None = 0,
        
        // Community
        CanAnnounce = 100,
        
        // FiveM
        CanRestartServers = 200,
        
        // IPS
        CanFindForumMembers = 300,
        
        // Moderation
        CanPruneMessages = 400,
        
        // Music
        CanUseVoiceTest = 500,
        
    }
}