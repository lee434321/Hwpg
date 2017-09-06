using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// 接口3.6获取已支付账单历史记录处理类
/// </summary>
public class Biz36:BizFactory
{
    Req36 req = null;

	public Biz36()
	{
	}

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="postStr"></param>
    public Biz36(string postStr)
    {
        this.BIZ_NAME = "接口3.6";
        req = Common.Deserialize<Req36>(postStr) as Req36;
    }

    /// <summary>
    /// 请求处理函数
    /// </summary>
    /// <returns></returns>
    public override string InvokeRequest()
    {
        Resp36 resp = new Resp36();
        try
        {
            Criteria c = new Criteria();
            c.leaseNumber = this.req.data.leasenum;
            c.startDate = this.req.data.startdate;
            c.endDate = this.req.data.enddate;
            DataTable dt = this.DAL_Select(c);

            resp.data = Rig(dt);
            resp.data.result = "100";
            resp.data.message = "接口操作完成";
        }
        catch(Exception err)
        {
            resp.code = "200";
            resp.msg = "接口操作失败!";
            resp.status = "false";
            resp.time = Common.Today;
            resp.data.result = "200";
            resp.data.message = err.Message;
        }

        return Common.Serialize<Resp36>(resp);
    }

    private Resp36_1 Rig(DataTable dt)
    {
        Resp36_1 data = new Resp36_1();
        data.paynum = dt.Rows.Count;

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            Resp36_2 r362 = new Resp36_2();
            r362.payamount = Common.DbNull2Doub(dt.Rows[i]["amount"]);
            r362.paydate = Common.DbNull2Str(dt.Rows[0]["paydate"]);

            //有争议，这里的支付结果应该是receipt或者是分录的创建结果。因为接口总是在用户支付成功的前提下操作
            r362.payresult = Common.DbNull2Str(dt.Rows[0]["status"]); 

            data.payinfo.Add(r362);
        }

        return data;
    }

    private DataTable DAL_Select(Criteria c)
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, false);
        try
        {
            string sql = "";
            sql += "select * from t_payment where 1=1 ";
            if (c.startDate != "") 
                sql += " and paydate>=to_date('" + c.startDate + "','yyyy-mm-dd')";
            
            if (c.endDate != "")
                sql += " and paydate<=to_date('" + c.endDate + "','yyyy-mm-dd')";

            if (c.leaseNumber != "")
                sql += " and leasenumber='" + c.leaseNumber + "'";

            DataTable dt= db.ExecuteDT(sql, null);
            db.Commit();
            return dt;
        }
        catch(Exception err)
        {
            db.Abort();
            throw err;
        }
    }
}