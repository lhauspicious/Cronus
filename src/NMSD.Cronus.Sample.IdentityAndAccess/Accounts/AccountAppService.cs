﻿using NMSD.Cronus.DomainModelling;
using NMSD.Cronus.Sample.IdentityAndAccess.Accounts.Commands;

namespace NMSD.Cronus.Sample.IdentityAndAccess.Accounts
{
    public class AccountAppService : AggregateRootApplicationService<Account>,
        IMessageHandler<RegisterAccount>,
        IMessageHandler<ChangeAccountEmail>
    {

        public void Handle(RegisterAccount message)
        {
            Repository.Save(new Account(message.Id, message.Email), message);
        }

        public void Handle(ChangeAccountEmail message)
        {
            Repository.Update<Account>(message.Id, message, user => user.ChangeEmail(message.OldEmail, message.NewEmail));
        }
    }
}
