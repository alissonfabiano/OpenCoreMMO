﻿using System;
using Autofac;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Creatures.Services;
using NeoServer.Server.Commands.Player.UseItem;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Standalone.IoC.Modules
{
    public static class ServiceInjection
    {
        public static ContainerBuilder AddServices(this ContainerBuilder builder)
        {
            //Game services
            builder.RegisterType<DealTransaction>().As<IDealTransaction>().SingleInstance();
            builder.RegisterType<CoinTransaction>().As<ICoinTransaction>().SingleInstance();
            builder.RegisterType<SharedExperienceConfiguration>().As<ISharedExperienceConfiguration>().SingleInstance();
            builder.RegisterType<PartyInviteService>().As<IPartyInviteService>().SingleInstance();
            builder.RegisterType<SummonService>().As<ISummonService>().SingleInstance();
            builder.RegisterType<ToMapMovementService>().As<IToMapMovementService>().SingleInstance();
            
            
            //game builders
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).As<IInspectionTextBuilder>()
                .SingleInstance();

            //application services
            builder.RegisterType<HotkeyService>().SingleInstance();

            return builder;
        }
    }
}