﻿using System;
using NMSD.Cronus.Commanding;
using NMSD.Cronus.Eventing;
using NMSD.Cronus.Messaging;
using NMSD.Cronus.Sample.Collaboration.Collaborators;
using NMSD.Cronus.Sample.Collaboration.Collaborators.Commands;
using NMSD.Cronus.Sample.IdentityAndAccess.Users.Events;

namespace NMSD.Cronus.Sample.Collaboration.Ports
{
    public class CollaboratorPorts : IPort, IMessageHandler,
        IMessageHandler<NewUserRegistered>
    {
        public IPublisher<ICommand> CommandPublisher { get; set; }

        public void Handle(NewUserRegistered message)
        {
            CommandPublisher.Publish(new CreateNewCollaborator(new CollaboratorId(Guid.NewGuid()), message.Email));
        }

    }
}
