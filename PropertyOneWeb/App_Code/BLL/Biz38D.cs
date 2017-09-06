using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 接口3.8D-系统管理员获取Feedback回复信息
/// </summary>
public class Biz38D : BizFactory
{
    Req38D req = null;
    public Biz38D()
    { }

    public Biz38D(string postStr)
    {
        this.BIZ_NAME = "接口3.8D";
        this.req = Common.Deserialize<Req38D>(postStr) as Req38D;
    }

    public override string InvokeRequest()
    {
        Resp38B resp = new Resp38B();
        try
        {
            Criteria c = new Criteria();
            c.leaseNumber = this.req.data.userid; //注意这里的userid
            c.feedbackId = this.req.data.feedbackid;
            List<M_T_Feedback_Res> etyList = this.Select(c);

            resp.data.result = "100";
            resp.data.message = "接口操作成功";

            Resp38B2 r38b2 = null;
            for (int i = 0; i < etyList.Count; i++)
            {
                r38b2 = new Resp38B2();
                r38b2.date = etyList[i].CreateDate.ToString();
                r38b2.detail = etyList[i].Detail;
                r38b2.id = etyList[i].Id;
                r38b2.replytype = etyList[i].ReplyType;
                r38b2.replyperson = etyList[i].ReplyPerson;
                resp.data.replyinfo.Add(r38b2);
            }
        }
        catch (Exception err)
        {
            resp.code = "200";
            resp.msg = "接口操作失败";
            resp.time = Common.Today;
            resp.status = "false";

            resp.data.result = "200";
            resp.data.message = err.Message;
        }
        return Common.Serialize<Resp38B>(resp);
    }

    private List<M_T_Feedback_Res> Select(Criteria c)
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, false);
        try
        {
            string sql = "select * from t_feedback_res where 1=1 ";

            if (c.leaseNumber!="")
            {
                sql += " and LeaseNum='" + c.leaseNumber + "' ";
            }
            if (c.feedbackId!=0)
            {
                sql += " and feedbackId=" + c.feedbackId.ToString();
            }

            List<M_T_Feedback_Res> etyList= db.ExecuteList<M_T_Feedback_Res>(sql, null);
            db.Commit();
            return etyList;
        }
        catch (Exception err)
        {
            db.Abort();
            throw err;
        }
    }
}