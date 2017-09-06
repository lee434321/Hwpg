using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 接口3.12新建Notice信息处理类
/// </summary>
public class Biz312 :BizFactory
{
    Req312 req = null;
    public Biz312()
    { }

    public Biz312(string postStr)
    {
        this.BIZ_NAME = "接口3.12";
        req = Common.Deserialize<Req312>(postStr) as Req312;
    }

    public override string InvokeRequest()
    {
        Resp312 resp = null;
        try
        {
            resp = new Resp312();
            if (NewNotice())
            {
                resp.data.result = "100";
                resp.data.message = "接口操作完成";
            }
            else
            {
                resp.data.result = "200";
                resp.data.message = "接口操作失败";
            }   
        }
        catch (Exception err)
        {
            resp.data.result = "200";
            resp.data.message = err.Message;
        }
        return Common.Serialize<Resp312>(resp);
    }

    private bool NewNotice()
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, true);
        try
        {
            M_T_Notice ety = new M_T_Notice();
            
            ety.LeaseNumber = this.req.data.userid;
            ety.ImgUrlLarge = this.req.data.imgurl;
            ety.Detail = this.req.data.detail;
            ety.Title = this.req.data.title;
            ety.Type = this.req.data.type;
            ety.NoticeId = Common.DbNull2Int(Common.GetSingleValue("select max(noticeid) from t_notice", db)) + 1;
            ety.Approve = "A";
            ety.CreateDate = DateTime.Now;
            ety.EndDate = DateTime.Parse(this.req.data.enddate);
            ety.StartDate = DateTime.Parse(this.req.data.startdate);
            ety.Status = "A";
            ety.CreateBy = Common.CurrentUser;
            ety.UpdateBy = Common.CurrentUser;
            ety.UpdateDate = DateTime.Now;

            Common.Insert<M_T_Notice>(ety, db);
            db.Commit();
            return true;
        }
        catch (Exception err)
        {
            db.Abort();
            throw err;
        }
    }
}