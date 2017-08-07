using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Fanex.BetList.Accounting.Constants;
using Fanex.BetList.Accounting.Resources;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Builder.StatusBuilder;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.Factories;
using Fanex.BetList.Core.Templates;

namespace Fanex.BetList.Accounting.Core
{
    public static class BetListHelper
    {
        public static readonly Regex IsNumericRegex = new Regex("^(" +
            /*Hex*/ @"0x[0-9a-f]+" + "|" +
            /*Bin*/ @"0b[01]+" + "|" +
            /*Oct*/ @"0[0-7]+" + "|" +
            /*Dec*/ @"((?!0)|[-+])(\d*\.)?\d+(e\d+)?" +
                                                    ")$");

        public static bool IsNumeric(string value)
        {
            return IsNumericRegex.IsMatch(value);
        }

        public static bool IsHRWinPlaceBettype(int betType)
        {
            return betType == BetTypeContants.WinPlace ||
                betType == BetTypeContants.WinPlaceUKTote ||
                betType == BetTypeContants.VirtualSportWinPlace;
        }

        public static string IsNegative(decimal input)
        {
            return input < 0 ? "negative" : "positive";
        }

        public static string PercentFormat(decimal input)
        {
            var isInputInterger = (input * 100) % 1 == 0;

            return isInputInterger ?
                Math.Round(input * 100, 0).ToString() + Labels.Symbol_Percent :
                Math.Round(input * 100, 2).ToString() + Labels.Symbol_Percent;
        }

        public static string BuildStatus(
            ITicket ticket,
            ITicketHelper ticketHelper,
            List<ITicketData> ticketData,
            BetListHTMLFactory htmlFactory = null,
            BetListHTMLBuilder builder = null)
        {
            htmlFactory = htmlFactory ?? new BetListHTMLFactory();
            builder = builder ?? htmlFactory.CreateBuilder();

            IStatus statusBuilder = builder.CreateStatusBuilder(ticket.BetTypeId);
            Status_Template statusBlock = statusBuilder.Render(ticket, ticketHelper, ticketData, true);
            statusBlock.ShowIP = null;

            return statusBlock.ToString();
        }
    }
}
