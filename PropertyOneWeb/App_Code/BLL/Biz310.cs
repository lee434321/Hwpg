using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// 接口3.10回复Feedback信息处理类
/// </summary>
public class Biz310 : BizFactory
{
    Req310 req = null;
    public Biz310()
    { }

    public Biz310(string postStr)
    {
        this.BIZ_NAME = "接口3.10";
        this.req = Common.Deserialize<Req310>(postStr) as Req310;
    }

    public override string InvokeRequest()
    {
        Resp310 resp = new Resp310();

        try
        {
            M_T_Feedback_Res ety = new M_T_Feedback_Res();
            ety.Approve = "A"; //审批状态 A-已审批 I-未审批
            ety.CreateBy = Common.CurrentUser;
            ety.CreateDate = DateTime.Now;
            ety.Detail = this.req.data.replydetail;
            ety.FeedbackId = this.req.data.feedbackid;
            ety.LeaseNum = this.req.data.leasenum;
            ety.Status = "A"; //状态 A-有效 I-无效
            this.Insert(ety);
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
        return Common.Serialize<Resp310>(resp);
    }

    private void Insert(M_T_Feedback_Res ety)
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, true);
        try
        {
            ety.Id = Common.DbNull2Int(Common.GetSingleValue("select max(id) from T_Feedback_Res", db)) + 1;
            Common.Insert<M_T_Feedback_Res>(ety, db);
            db.Commit();
        }
        catch (Exception err)
        {
            db.Abort();
            throw err;
        }
    }
}