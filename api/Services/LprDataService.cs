using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Timers;
using api.Enums;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.LprData;
using api.Services.Gates;
using api.utils;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;

namespace api.Services
{
    public interface ILprDataService
    {
        void gateManagement(LprReceiveModel lprReceiveModel);
    }
    public class LprDataService : ILprDataService
    {

        private readonly IServiceScopeFactory serviceScopeFactory;

        public LprDataService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void gateManagement(LprReceiveModel lprReceiveModel)
        {
            bool success = Enum.TryParse(lprReceiveModel.gateType, out GateType gateType);
            if (!success)
            {
                Log.Error("GateType is not valid");
                return;
            }

            GateHandler gateHandler;

            switch (gateType)
            {
                case GateType.IN:
                    gateHandler = new MainEntryGatehandler(serviceScopeFactory);
                    gateHandler.HandleGateEvent(lprReceiveModel);
                    break;
                case GateType.IN_Electronic:
                    gateHandler = new ElectricEntryGatehandler(serviceScopeFactory);
                    gateHandler.HandleGateEvent(lprReceiveModel);
                    break;
            }
        }
    }
}




// TODO: a cron job that will run every hour to update the hourly available space by (- reservation count)

// A logic flow of a smart parking system about gate & record & payment

// the system will receive the license plate & gate type from the rpc

// there are 4 gate in the system
// MainEntry, ElectricEntry, ElectricExit, ElectricExit
// if a user need to get into electric area, the flow is in the order

// when a user get to the MainEntry, ElectricEntry, ElectricExit will record a record and a relate sessionID

// the record will save their In,out time, spaceType(Regular(for Main area), Electric)

// if a user leave at 30mins before the entryTime it will free
// if a user pay during parking, if the payTime is short than 20mins it is fine, but not if more than 20mins charge one hour

// if there is a resvation in the type of recrod it will give a discount