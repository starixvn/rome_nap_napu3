using System.Collections.Generic;
using Fanex.BetList.Accounting.Core;
using Fanex.BetList.Accounting.Models;
using Fanex.BetList.Accounting.Resources;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Fanex.BetList.Accounting.Extensions
{
    public static class BetListExtension
    {
        public static string BuildStake(this TicketDetail betDetail, ref BetListHTMLBuilder builder)
        {
            string stakeBuilder = builder.BuildStake(betDetail.Ticket);

            if (betDetail.IsShowOtExRate)
            {
                stakeBuilder +=
                    "<br />" +
                    "<span class=\"r exrate2\">" + betDetail.VirtualRate + "</span>" +
                    "<br />" +
                    "<span class=\"r exrate2\">(" + Labels.Ex_Rate_2 + ")</span>";
            }

            return stakeBuilder;
        }

        public static IBetListExcelBuilder AddHeaderStatement(
            this IBetListExcelBuilder factory,
            bool isShowWinLost,
            bool isShowSuperPT,
            bool isShowMasterPT,
            bool isShowAgentPT,
            bool isShowMember)
        {
            ICellStyle headerStyle = factory.CreateCellStyle();
            headerStyle.CloneStyleFrom(factory.EvenCellStyle);
            headerStyle.FillForegroundColor = 44;
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.VerticalAlignment = VerticalAlignment.Center;
            headerStyle.SetFont(factory.RTFHelper.PosFont);

            CellAddress cellAddr;
            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.No), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 1200);

            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Trans_Time), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 5400);

            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Choice), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 15500);

            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Odds), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 2400);

            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Stake), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 2400);

            if (isShowWinLost)
            {
                cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Win_Loss), headerStyle);
                factory.SetColumnWidth(cellAddr.ColumnIndex, 3400);
            }

            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Status), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 3400);

            if (isShowSuperPT)
            {
                cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Super + " " + Labels.PT + " / " + Labels.WinLoss), headerStyle);
                factory.SetColumnWidth(cellAddr.ColumnIndex, 3400);
            }

            if (isShowMasterPT)
            {
                cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Master + " " + Labels.PT + " /  " + Labels.WinLoss), headerStyle);
                factory.SetColumnWidth(cellAddr.ColumnIndex, 3400);
            }

            if (isShowAgentPT)
            {
                cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Agent + " " + Labels.PT + " /  " + Labels.WinLoss), headerStyle);
                factory.SetColumnWidth(cellAddr.ColumnIndex, 3400);
            }

            if (isShowMember)
            {
                cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Operator), headerStyle);
                factory.SetColumnWidth(cellAddr.ColumnIndex, 4800);
            }

            return factory;
        }

        public static IBetListExcelBuilder AddCreator(
            this IBetListExcelBuilder factory,
            TicketDetail ticketDetail,
            ITicketHelper ticketHelper)
        {
            IRichTextString rtfCreator = new HSSFRichTextString(ticketDetail.Creator + "\n" + ticketDetail.Ticket.IP);
            factory.AddCell(rtfCreator, ticketHelper.Index % 2 == 0 ? factory.OddCellStyleCenterAligned : factory.EvenCellStyleCenterAligned);

            return factory;
        }

        public static IBetListExcelBuilder AddStatus(
            this IBetListExcelBuilder factory,
            TicketDetail ticketDetail,
            ITicketHelper ticketHelper)
        {
            IRichTextString rtfCreator = new HSSFRichTextString(ticketDetail.Ticket.Status);
            factory.AddCell(rtfCreator, ticketHelper.Index % 2 == 0 ? factory.OddCellStyleCenterAligned : factory.EvenCellStyleCenterAligned);

            return factory;
        }
    }
}
