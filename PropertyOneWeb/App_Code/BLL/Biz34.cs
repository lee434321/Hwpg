using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// 3.4	获取待支付账单信息处理类
/// act=getonlinepayinfo
/// </summary>
public class Biz34 : BizFactory
{
    Req34 req = null;

    public Biz34()
    {
    }

    public Biz34(string postStr)
    {
        this.BIZ_NAME = "接口3.4";
        this.req = Common.Deserialize<Req34>(postStr) as Req34;
    }

    public override string InvokeRequest()
    {
        Resp34 resp = new Resp34();
        try
        {
            if (req.data.leasenum != "")
            {
                DataTable dt = DAL_Select_PayInfo(req.data.leasenum);
                Common.AttachRowId(dt);
                resp.code = "100";
                resp.status = "true";
                resp.msg = "请求成功!";
                resp.time = Common.Today;

                resp.data.result = "100";
                resp.data.message = "获取成功!";
                resp.data.customername = Common.DbNull2Str(dt.Rows[0]["customername"]);
                resp.data.payinfonum = Common.DbNull2Int(dt.Rows.Count);
                resp.data.premisename = Common.DbNull2Str(dt.Rows[0]["premisename"]);
                resp.data.shoparea = Common.DbNull2Str(dt.Rows[0]["shoparea"]);
                resp.data.shopname = Common.DbNull2Str(dt.Rows[0]["shopname"]);

                resp.data.totalamount = Common.DbNull2Dec(dt.Compute("sum(outstanding)", ""));
                resp.data.payinfo = (List<Resp34_2>)Common.DT2List<Resp34_2>(dt);
            }
            else
            {
                resp.code = "100";
                resp.status = "true";
                resp.msg = "请求成功!";
                resp.time = Common.Today;

                resp.data.result = "200";
                resp.data.message = "租约号为空,无法查询";
            }
        }
        catch (Exception err)
        {
            resp.code = "200";
            resp.status = "false";
            resp.msg = err.Message;
            resp.time = Common.Today;
        }
        return Common.Serialize<Resp34>(resp);
    }

    public string GetOutstanding(string leaseNum)
    {
        Resp34 resp = new Resp34();
        try
        {
            if (leaseNum != "")
            {
                DataTable dt = DAL_Select_PayInfo(leaseNum);
                Common.AttachRowId(dt);
                resp.code = "100";
                resp.status = "true";
                resp.msg = "请求成功!";
                resp.time = Common.Today;

                resp.data.result = "100";
                resp.data.message = "获取成功!";
                resp.data.customername = Common.DbNull2Str(dt.Rows[0]["customername"]);
                resp.data.payinfonum = Common.DbNull2Int(dt.Rows.Count);
                resp.data.premisename = Common.DbNull2Str(dt.Rows[0]["premisename"]);
                resp.data.shoparea = Common.DbNull2Str(dt.Rows[0]["shoparea"]);
                resp.data.shopname = Common.DbNull2Str(dt.Rows[0]["shopname"]);
                resp.data.totalamount = Common.DbNull2Dec(dt.Compute("sum(outstanding)", ""));
                resp.data.payinfo = (List<Resp34_2>)Common.DT2List<Resp34_2>(dt);
            }
            else
            {
                resp.code = "100";
                resp.status = "true";
                resp.msg = "请求成功!";
                resp.time = Common.Today;

                resp.data.result = "200";
                resp.data.message = "租约号为空,无法查询";
            }
        }
        catch (Exception err)
        {
            resp.code = "200";
            resp.status = "false";
            resp.msg = err.Message;
            resp.time = Common.Today;
        }
        return Common.Serialize<Resp34>(resp);
    }

    /// <summary>
    /// 获取待支付Invoice
    /// </summary>
    /// <param name="leaseNum"></param>
    /// <returns></returns>
    private DataTable DAL_Select_PayInfo(string leaseNum)
    {
        DbHelper db = new DbHelper(false);
        string sql = "";
        sql += " select ai.customer_number,";
        sql += "ac.customer_name as customername,";
        sql += "'' as shopname, ";
        sql += "l.premise_name1 as premisename, ";
        sql += "'' as shoparea, ";
        sql += "0 as totalamount, ";
        sql += " 0 as payinfonum, ";
        //sql += "---- start payinfo detail ";
        sql += "ai.invoice_number as transno, ";
        sql += "ail.invoice_line_number as invoicelinenum,";
        sql += "ail.charge_code as chargeitem, ";
        sql += "ail.charge_description as descr, ";
        sql += "ail.invoice_amount as amount, ";
        sql += " ail.outstanding_amount as outstanding, ";
        sql += " ai.payment_due_date as duedate ";
        //sql += " ---- end payinfo detail ";
        sql += "from ar_invoice ai, ar_invoice_line ail, ar_customer ac,lm_lease l,ar_customer_site acs ";
        sql += " where 1 = 1 ";

        sql += " and ai.invoice_number = ail.invoice_number ";
        sql += " and Upper(SUBSTR(ail.PAY_STATUS, 1, 1)) in ('U', 'P') and ail.outstanding_amount > 0 ";
        sql += " and ai.lease_number = trim(l.lease_number) and l.status = 'A' and l.active = 'A' ";
        sql += " and ai.customer_number = ac.customer_number and ac.customer_number = acs.customer_number ";
        sql += " and trim(l.site_number) = trim(acs.site_number) ";
        sql += " and ai.lease_number ='" + leaseNum + "'";
        

        DataTable dt = db.ExecuteDT(sql, null);
        db.Commit();
        dt.TableName = "PayInfo";
        return dt;
    }
}