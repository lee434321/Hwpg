using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// 接口3.9新建Feedback信息处理类
/// </summary>
public class Biz39 : BizFactory
{
    Req39 req = null;

    public Biz39()
    { }

    public Biz39(string postStr)
    {
        this.BIZ_NAME = "接口3.9";
        this.req = Common.Deserialize<Req39>(postStr) as Req39;
    }

    public override string InvokeRequest()
    {
        Resp39 resp = new Resp39();

        DbHelper db = new DbHelper(Common.OracleConnStrLocal, true);
        try
        {
            M_T_Feedback ety = new M_T_Feedback();
            ety.CreateBy = Common.CurrentUser;
            ety.CreateDate = DateTime.Now;
            ety.Title = this.req.data.title;
            ety.Type = Common.DbNull2Int(this.req.data.type);
            ety.Detail = this.req.data.detail;
            ety.FeedbackId = Common.DbNull2Int(Common.GetSingleValue("select max(feedbackid) from t_feedback", db)) + 1;
            ety.LeaseNumber = this.req.data.leasenum;
            ety.Status = "100";
            ety.PropertyCode = "<None>";
            Common.Insert<M_T_Feedback>(ety, db);
            db.Commit();
        }
        catch (Exception err)
        {
            db.Abort();
            resp.code = "200";
            resp.msg = "接口调用失败";
            resp.status = "false";
            resp.time = Common.Today;

            resp.data.result = "200";
            resp.data.message = err.Message;
        }

        return Common.Serialize<Resp39>(resp);
    }
}