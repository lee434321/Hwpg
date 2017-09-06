using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// 接口3.13A-修改Notice信息处理类
/// </summary>
/// {"userid":"ACBV00176","noticeid":"4","startdate":"2017-09-02","enddate":"2017-09-29","type":"","title":"test2","detail":"fsdfs","imgurl":"/Upload/1709061428560704.jpg"}
/// {"code":"100","msg":"请求成功","status":"true","time":"2017-09-06 02:29:42","data":{"message":"Operation is not valid due to the current state of the object.","result":"200"}}

public class Biz313A : BizFactory
{
    Req313A req = null;
    public Biz313A()
    { }

    public Biz313A(string postStr)
    {
        this.BIZ_NAME = "接口3.13A";
        this.req = Common.Deserialize<Req313A>(postStr) as Req313A;
    }

    public override string InvokeRequest()
    {
        Resp313A resp = new Resp313A();
        try
        {
            if (UpdateNotice())
            {
                resp.data.result = "100";
                resp.data.message = "接口操作成功";
            } 
        }
        catch (Exception err)
        {
            resp.data.result = "200";
            resp.data.message = err.Message;
        }
        return Common.Serialize<Resp313A>(resp);
    }

    private bool UpdateNotice()
    {
        DbHelper db = new DbHelper(Common.OracleConnStrLocal, true);
        try
        {
            Criteria c = new Criteria();
            c.noticeId = this.req.data.noticeid;
            List<M_T_Notice> etyList = BHelper.FetchNotice(c, db);

            if (etyList.Count > 0)
            {
                etyList[0].StartDate = DateTime.Parse(this.req.data.startdate);
                etyList[0].EndDate = DateTime.Parse(this.req.data.enddate);
                etyList[0].Detail = this.req.data.detail;
                etyList[0].Title = this.req.data.title;
                etyList[0].ImgUrlLarge = this.req.data.imgurl;
                etyList[0].LeaseNumber = this.req.data.userid;
                etyList[0].UpdateBy = this.req.data.userid;
                
                List<string> keys = new List<string>();
                keys.Add("NoticeId");
                if (Common.Update<M_T_Notice>(etyList[0], db, keys) > 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        catch (Exception err)
        {
            db.Abort();
            throw err;
        }
        finally 
        {
            db.Commit();
        }
    }
}