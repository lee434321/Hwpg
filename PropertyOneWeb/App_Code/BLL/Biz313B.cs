using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 接口3.13B-删除Notice信息处理类
/// </summary>
public class Biz313B:BizFactory
{
    Req313B req = null;
    public Biz313B()
    { }

    public Biz313B(string postStr)
    {
        this.BIZ_NAME = "接口3.13B";
        this.req = Common.Deserialize<Req313B>(postStr) as Req313B;
    }

    public override string InvokeRequest()
    {
        int cnt = 0;
        Resp313B resp = new Resp313B();

        DbHelper db = new DbHelper(Common.OracleConnStrLocal, true);
        try
        {
            Criteria c = new Criteria();
            c.userid = this.req.data.userid;
            c.noticeId = this.req.data.noticeid;
            List<M_T_Notice> etyList = BHelper.FetchNotice(c, db);

            List<string> parameters = new List<string>();
            parameters.Add("NoticeId");
            for (int i = 0; i < etyList.Count; i++)
            {
                cnt += Common.Delete<M_T_Notice>(etyList[i], db, parameters);
            }
            db.Commit();
            
            resp.data.result = "100";
            resp.data.message = "接口操作完成(处理了" + cnt.ToString() + "条数据)";
        }
        catch (Exception err)
        {
            db.Abort();
            resp.data.result = "200";
            resp.data.message = err.Message;
        }
        return Common.Serialize<Resp313B>(resp);
    }
}