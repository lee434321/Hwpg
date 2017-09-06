using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// 接口 3.14 获取租户信息
/// act=getleaseinfo
/// </summary>
public class Biz314 : BizFactory
{
    Req314 req = null;

    public Biz314()
    { }

    public Biz314(string postStr)
    {
        this.BIZ_NAME = "接口3.14";
        req = Common.Deserialize<Req314>(postStr) as Req314;
    }

    public override string InvokeRequest()
    {
        Resp314 resp = new Resp314();
        try
        {
            /// 1.执行DAL查询，获取数据
            DataTable dt = DAL_Select_LeaseInfo(req.data.leasenum);
            if (dt.Rows.Count > 0)
            {
                /// 2.组织接口数据，返回
                List<Resp3142> list3142 = (List<Resp3142>)Common.DT2List<Resp3142>(dt);
                resp.data.result = "100";
                resp.data.message = "获取数据成功!";
                resp.data.leaseInfo = list3142;
            }
            else
            {
                resp.data.result = "200";
                resp.data.message = "获取数据失败";
            }
        }
        catch (Exception err)
        {
            resp.code = "200";
            resp.status = "false";
            resp.msg = err.Message;
            resp.time = Common.Today;
        }
        return Common.Serialize<Resp314>(resp);
    }

    private DataTable DAL_Select_LeaseInfo(string leaseNum)
    {
        DbHelper db = new DbHelper(false);
        string sql = "";
        sql += "select l.lease_number as leasenum";
        sql += ",l.customer_number as custnum";
        sql += ",c.customer_name as custname";
        sql += ",acs.billing_address_1||','||acs.billing_address_2 as billingaddress";
        sql += ",l.lease_term_from as leasestartdate";
        sql += ",l.lease_term_to as leaseenddate";
        sql += ",l.tenant_trade_name1 as tradename";
        sql += ",l.premise_name1 as premises";
        sql += ",acp.email as email";
        sql += ",acp.display_name as contactperson";
        sql += ",acp.office_main_phone_number as contactnum";

        sql += " from lm_lease l,ar_customer c,ar_customer_site acs ,ad_contact_point acp";
        sql += " where 1=1";
        sql += " and l.status='A' and l.active='A'";
        sql += " and l.customer_number=c.customer_number";
        sql += " and c.customer_number=acs.customer_number";
        sql += " and trim(c.customer_number)=trim(acp.addressee_code(+))";
        sql += " and c.active='A'";
        sql += " and l.lease_number ='" + leaseNum + "'";

        DataTable dt = db.ExecuteDT(sql, null);
        db.Commit();
        dt.TableName = "LeaseInfo";
        return dt;
    }
}