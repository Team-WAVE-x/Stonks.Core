using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Stonks.Core.Rewrite.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Precondition
{
    public class Developer : PreconditionAttribute
    {
        private readonly string _errorMessage;

        public Developer(string errorMessage = "❌ 본 명령어는 개발자만 사용할 수 있습니다.")
        {
            _errorMessage = errorMessage;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var setting = services.GetRequiredService<Setting>();

            if (context.User.Id == setting.Config.DeveloperId)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
            {
                context.Channel.SendMessageAsync(_errorMessage);
                return Task.FromResult(PreconditionResult.FromError(_errorMessage));
            }
        }
    }
}
