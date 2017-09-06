using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// 接口3.8C获取Feedback信息
/// </summary>
/// getfeedbacksystem
public class Biz38C:BizFactory
{
    Req38C req = null;
	public Biz38C()
	{
	}

    public Biz38C(string postStr)
    {
        this.BIZ_NAME = "接口3.8C";
        this.req = Common.Deserialize<Req38C>(postStr) as Req38C;
    }

    public override string InvokeRequest()
    {
        Resp38A resp = new Resp38A();
        try
        {
            Criteria c = new Criteria();
            c.endDate = this.req.data.enddate;
            c.startDate = this.req.data.startdate;
            c.leaseNumber = this.req.data.userid; //这里的userid表示是管理员登录
            c.type = this.req.data.type;
            c.status = this.req.data.status;

            List<M_T_Feedback> etyList = this.Select(c);

            resp.data.feedbacknum = etyList.Count;
            for (int i = 0; i < etyList.Count; i++)
            {
                Resp38A2 dtl = new Resp38A2();
                dtl.date = Common.DbNull2Str(etyList[i].CreateDate);
                dtl.detail = Common.DbNull2Str(etyList[i].Detail);
                dtl.feedbackid = Common.DbNull2Int(etyList[i].FeedbackId);
                dtl.status = Common.DbNull2Str(etyList[i].Status);
                dtl.title = Common.DbNull2Str(etyList[i].Title);
                dtl.type = Common.DbNull2Str(etyList[i].Type);
                resp.data.feedbackinfo.Add(dtl);
            }
        }
        catch (Exception err)
        {
            resp.code = "200";
            resp.msg = "接口调用失败";
            resp.status = "false";
            resp.time = Common.Today;

            resp.data.result = "200";
            resp.data.message = err.Message;
        }

        return Common.Serialize<Resp38A>(resp);
    }

    private List<M_T_Feedback> Select(Criteria c)
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, false);

        string sql = "";
        try
        {
            sql = "select * from t_feedback where 1=1";

            if (c.startDate != "")
            {
                sql += " and CreateDate>= to_date('" + c.startDate + "','yyyy-mm-dd') ";
            }
            if (c.endDate != "")
            {
                sql += " and CreateDate<=to_date('" + c.endDate + "','yyyy-mm-dd') ";
            }
            if (c.leaseNumber != "")
            {
                sql += " and LeaseNumber='" + c.leaseNumber + "' ";
            }
            if (c.status != "")
            {
                sql += " and status='" + c.status + "'";
            }
            if (c.type != "")
            {
                sql += " and type=" + c.type;
            }
            return db.ExecuteList<M_T_Feedback>(sql, null);
        }
        catch (Exception err)
        {
            throw err;
        }
    }
}