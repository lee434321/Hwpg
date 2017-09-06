using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 接口3.8B-租约用户获取Feedback的回复信息处理类
/// </summary>
public class Biz38B : BizFactory
{
    Req38B req = null;
    public Biz38B()
    { }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="postStr"></param>
    public Biz38B(string postStr)
    {
        this.BIZ_NAME = "接口3.8B";
        this.req = Common.Deserialize<Req38B>(postStr) as Req38B;
    }

    public override string InvokeRequest()
    {
        Resp38B resp = new Resp38B();
        
        Criteria c = new Criteria();
        c.leaseNumber = this.req.data.leasenum;
        c.feedbackId = this.req.data.feedbackid;
        List<M_T_Feedback_Res> etyList = this.Select(c);

        resp.code = "100";
        resp.msg = "接口操作成功";
        for (int i = 0; i < etyList.Count; i++)
        {
            Resp38B2 r38b2 = new Resp38B2();
            r38b2.date = etyList[i].CreateDate.ToString();
            r38b2.detail = etyList[i].Detail;
            r38b2.id = etyList[i].Id;
            r38b2.replytype = etyList[i].ReplyType;
            r38b2.replyperson = etyList[i].ReplyPerson;
            resp.data.replyinfo.Add(r38b2);
        }
        return Common.Serialize<Resp38B>(resp);
    }

    private List<M_T_Feedback_Res> Select(Criteria c)
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, false);
        try
        {
            string sql = "select * from " + M_T_Feedback_Res.TableName;
            if (c.leaseNumber != "")
            {
                sql += " and LeaseNum='" + c.leaseNumber + "' ";
            }
            if (c.feedbackId != 0)
            {
                sql += " and feedbackId=" + c.feedbackId.ToString();
            }
            return db.ExecuteList<M_T_Feedback_Res>(sql, null);
        }
        catch(Exception err)
        {
            throw err;
        }    
    }
}