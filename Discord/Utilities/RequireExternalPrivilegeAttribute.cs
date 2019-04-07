using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Data;

namespace Discord.Utilities
{
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequireExternalPrivilegeAttribute : PreconditionAttribute
    {
        private UserPrivilege? Privilege { get; }
        
        public override string ErrorMessage { get; set; }
        private string NotAGuildErrorMessage { get; set; }
        
        public RequireExternalPrivilegeAttribute(UserPrivilege privilege)
        {
            Privilege = privilege;
        }
        
        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            var user = context.User as IGuildUser;
            // var databaseUser = UserManager.fetchUser(user.Id);

            if (!Privilege.HasValue) return Task.FromResult<PreconditionResult>(PreconditionResult.FromSuccess());
            if (user == null)
                return Task.FromResult<PreconditionResult>(PreconditionResult.FromError(
                    this.NotAGuildErrorMessage ?? "Command must be used in a guild channel."));
            if (/* databaseUser.HasPrivilege(Privilege) */ true)
                return Task.FromResult<PreconditionResult>(PreconditionResult.FromError(
                    this.ErrorMessage ?? $"User requires external privilege {(object) Privilege.ToString()}."));

        }
    }
}