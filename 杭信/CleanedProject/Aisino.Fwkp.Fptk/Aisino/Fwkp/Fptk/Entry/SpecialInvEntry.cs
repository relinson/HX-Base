﻿namespace Aisino.Fwkp.Fptk.Entry
{
    using Aisino.Framework.MainForm.UpDown;
    using Aisino.Framework.Plugin.Core;
    using Aisino.Framework.Plugin.Core.Command;
    using Aisino.Framework.Plugin.Core.Util;
    using Aisino.FTaxBase;
    using Aisino.Fwkp.Fptk;
    using Aisino.Fwkp.Fptk.Form;
    using log4net;
    using System;
    using System.Windows.Forms;

    public class SpecialInvEntry : AbstractCommand
    {
        private ILog log = LogUtil.GetLogger<SpecialInvEntry>();
        internal static InvoiceForm_ZZS zyfpIfm;

        protected override void RunCommand()
        {
            try
            {
                if ((zyfpIfm == null) || !zyfpIfm.HasShow())
                {
                    SPFLService service = new SPFLService();
                    if (FLBM_lock.isFlbm() && (service.GetMaxBMBBBH() == "0.0"))
                    {
                        MessageManager.ShowMsgBox("INP-242133");
                    }
                    else if ((SNYInvEntry.snyIfm != null) && SNYInvEntry.snyIfm.HasShow())
                    {
                        MessageManager.ShowMsgBox("INP-242163");
                    }
                    else
                    {
                        IFpManager manager = new FpManager();
                        if (manager.CanInvoice(0))
                        {
                            string[] current = manager.GetCurrent(0);
                            if (current != null)
                            {
                                if (new StartConfirmForm(0, current).ShowDialog() == DialogResult.OK)
                                {
                                    zyfpIfm = new InvoiceForm_ZZS(0, 0, current[0], current[1]);
                                    if (zyfpIfm.InitSuccess)
                                    {
                                        zyfpIfm.ShowDialog();
                                    }
                                }
                            }
                            else
                            {
                                MessageManager.ShowMsgBox(manager.Code());
                            }
                        }
                        else
                        {
                            MessageManager.ShowMsgBox(manager.Code());
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.log.Error("专用发票开具功能加载异常：" + exception.ToString());
                string[] textArray1 = new string[] { exception.ToString() };
                MessageManager.ShowMsgBox("INP-242115", textArray1);
            }
        }

        protected override bool SetValid()
        {
            TaxCard card = TaxCardFactory.CreateTaxCard();
            string str = PropertyUtil.GetValue("HandMade", "0");
            return ((((int)card.TaxMode == 2) && card.QYLX.ISZYFP) && (str == "0"));
        }
    }
}

